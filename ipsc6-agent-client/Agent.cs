using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using org.pjsip.pjsua2;

namespace ipsc6.agent.client
{
    public class Agent : IDisposable
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Agent));

        public Agent(IEnumerable<ConnectionInfo> connectionInfoCollection, ushort localPort = 0, string localAddress = "")
        {
            foreach (var connInfo in connectionInfoCollection)
            {
                var conn = new Connection(localPort, localAddress);
                conn.OnConnectionStateChanged += Conn_OnConnectionStateChanged;
                conn.OnServerSentEvent += Conn_OnServerSend;
                internalConnections.Add(conn);
                connectionList.Add(connInfo);
            }
        }

        public Agent(IEnumerable<string> addresses, ushort localPort = 0, string localAddress = "")
        {
            foreach (var s in addresses)
            {
                var connInfo = new ConnectionInfo(s);
                var conn = new Connection(localPort, localAddress);
                conn.OnConnectionStateChanged += Conn_OnConnectionStateChanged;
                conn.OnServerSentEvent += Conn_OnServerSend;
                internalConnections.Add(conn);
                connectionList.Add(connInfo);
            }
        }

        ~Agent()
        {
            Dispose(disposing: false);
        }

        // Flag: Has Dispose already been called?
        private bool disposedValue = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                }
                // 释放未托管的资源(未托管的对象)并重写终结器
                // 将大型字段设置为 null
                foreach (var sipAcc in sipAccounts)
                {
                    logger.DebugFormat("Dispose {0}", sipAcc);
                    sipAcc.Dispose();
                }
                sipAccounts.Clear();
                //
                foreach (var conn in internalConnections)
                {
                    logger.DebugFormat("Dispose {0}", conn);
                    conn.Dispose();
                }
                internalConnections.Clear();
                //
                disposedValue = true;
            }
        }

        internal static Endpoint SipEndpoint;
        internal static TaskFactory SyncFactory;

        internal Sip.Call currentCall;

        public static void Initial()
        {
            logger.Info("Initial");

            logger.Debug("network.Connector.Initial()");
            network.Connector.Initial();

            SyncFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

            logger.Debug("create pjsua2 endpoint");
            SipEndpoint = new Endpoint();
            SipEndpoint.libCreate();
            using (var epCfg = new EpConfig())
            using (var sipTpConfig = new TransportConfig { port = 5060 })
            {
                //epCfg.logConfig.level = 3;
                //epCfg.logConfig.msgLogging = 0;
                //epCfg.logConfig.writer = SipLogWriter.Instance;
                SipEndpoint.libInit(epCfg);
                //if (!SipEndpoint.libIsThreadRegistered())
                //{
                //    logger.Debug("pjsua2 endpoint registers thread");
                //    SipEndpoint.libRegisterThread(Thread.CurrentThread.Name);
                //}
                logger.Debug("pjsua2 endpoint creates transport");
                SipEndpoint.transportCreate(pjsip_transport_type_e.PJSIP_TRANSPORT_UDP, sipTpConfig);
            }
            logger.Debug("pjsua2 endpoint starts");
            SipEndpoint.libStart();
        }

        public static void Release()
        {
            logger.Info("Release");

            logger.Debug("network.Connector.Release()");
            network.Connector.Release();

            logger.Debug("destory pjsua2 Endpoint");
            SipEndpoint.libDestroy();
            SipEndpoint.Dispose();
        }

        private AgentRunningState runningState = AgentRunningState.Stopped;
        public AgentRunningState RunningState => runningState;

        string workerNumber;
        public string WorkerNumber => workerNumber;
        string password;

        readonly List<ConnectionInfo> connectionList = new List<ConnectionInfo>();
        public IReadOnlyCollection<ConnectionInfo> ConnectionList => connectionList;
        public int GetConnetionIndex(ConnectionInfo connectionInfo) => connectionList.IndexOf(connectionInfo);
        private Connection GetConnection(int index) => internalConnections[index];
        private Connection GetConnection(ConnectionInfo connectionInfo) => internalConnections[GetConnetionIndex(connectionInfo)];
        public ConnectionState GetConnectionState(int index) => GetConnection(index).State;
        public ConnectionState GetConnectionState(ConnectionInfo connectionInfo)
        {
            var index = connectionList.IndexOf(connectionInfo);
            return internalConnections[index].State;
        }

        readonly List<Connection> internalConnections = new List<Connection>();

        int mainConnectionIndex = -1;
        public int MainConnectionIndex => mainConnectionIndex;

        public event EventHandler OnMainConnectionChanged;

        public ConnectionInfo MainConnectionInfo => (mainConnectionIndex < 0) ? null : connectionList[mainConnectionIndex];

        Connection MainConnection => internalConnections[mainConnectionIndex];
        public int AgentId => MainConnection.AgentId;

        string displayName;
        public string DisplayName => displayName;

        int agentChannel = -1;
        public int AgentChannel => agentChannel;

        AgentState agentState = AgentState.NotExist;
        WorkType workType = WorkType.Unknown;
        public AgentState AgentState => agentState;
        public WorkType WorkType => workType;
        public AgentStateWorkType AgentStateWorkType
        {
            get => new AgentStateWorkType(agentState, workType);
            private set
            {
                agentState = value.AgentState;
                workType = value.WorkType;
            }
        }
        public event AgentStateChangedEventHandler OnAgentStateChanged;

        TeleState teleState = TeleState.OnHook;
        public TeleState TeleState => teleState;
        public event TeleStateChangedEventHandler OnTeleStateChanged;

        readonly HashSet<QueueInfo> queueInfoCollection = new HashSet<QueueInfo>();
        public IReadOnlyCollection<QueueInfo> QueueInfoCollection => queueInfoCollection;
        public event QueueInfoEventHandler OnQueueInfo;

        public event HoldInfoEventHandler OnHoldInfo;

        readonly HashSet<CallInfo> callCollection = new HashSet<CallInfo>();
        public IReadOnlyCollection<CallInfo> CallCollection => callCollection;

        public IReadOnlyCollection<CallInfo> HeldCallCollection
        {
            get
            {
                lock (this)
                {
                    return (
                        from m in callCollection
                        where m.IsHeld
                        select m
                    ).ToList();
                }
            }
        }

        void Conn_OnServerSend(object sender, ServerSentEventArgs e)
        {
            try
            {
                var conn = sender as Connection;
                var msg = e.Message;
                var index = internalConnections.IndexOf(conn);
                var connInfo = connectionList[index];
                switch (msg.Type)
                {
                    case MessageType.REMOTE_MSG_SETSTATE:
                        /// 状态改变
                        ProcessStateChangedMessage(connInfo, msg);
                        break;
                    case MessageType.REMOTE_MSG_SETTELESTATE:
                        /// 电话状态改变
                        ProcessTeleStateChangedMessage(connInfo, msg);
                        break;
                    case MessageType.REMOTE_MSG_QUEUEINFO:
                        /// 排队信息
                        ProcessQueueInfoMessage(connInfo, msg);
                        break;
                    case MessageType.REMOTE_MSG_HOLDINFO:
                        /// 保持信息
                        ProcessHoldInfoMessage(connInfo, msg);
                        break;
                    case MessageType.REMOTE_MSG_SENDDATA:
                        /// 其他各种数据
                        ProcessDataMessage(connInfo, msg);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exce)
            {
                logger.ErrorFormat("OnServerSend - {0}: {1}", sender, exce);
                throw;
            }
        }

        void ProcessStateChangedMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            AgentState[] workingState = { AgentState.Ring, AgentState.Work };
            AgentStateWorkType newState;
            var newWorkType = (WorkType)msg.N2;
            if (newWorkType == WorkType.OffHooked)
            {
                // 特殊处理，不改变 WorkType!
                newState = new AgentStateWorkType((AgentState)msg.N1, AgentStateWorkType.WorkType);
            }
            else
            {
                newState = new AgentStateWorkType((AgentState)msg.N1, (WorkType)msg.N2);
            }
            logger.DebugFormat("StateChanged - {0}", newState);
            var currIndex = connectionList.FindIndex(ci => ci == connInfo);
            AgentStateWorkType oldState = null;
            Connection oldMainConnObj = null;
            lock (this)
            {
                isOffHookRequesting = false;  // 请求服务器摘机的过程结束
                if (AgentStateWorkType != newState)
                {
                    oldState = AgentStateWorkType.Clone() as AgentStateWorkType;
                    AgentStateWorkType = newState;
                }
                // 如果是工作状态且主连接不是当前的，需要切换！
                if (workingState.Any(x => x == newState.AgentState) && currIndex != mainConnectionIndex)
                {
                    oldMainConnObj = MainConnection;
                    // 先修改 index
                    mainConnectionIndex = currIndex;
                    logger.InfoFormat("切换主服务节点到 {0}", MainConnection);
                }
            }
            if (oldMainConnObj != null)
            {
                // 通知原来的主，无论能否通知成功
                Task.Run(async () =>
                {
                    var req = new AgentRequestMessage(MessageType.REMOTE_MSG_TAKENAWAY);
                    await oldMainConnObj.Request(req);
                });
                // fire the event
                OnMainConnectionChanged?.Invoke(this, new EventArgs());
            }
            if (oldState != null)
            {
                var ev = new AgentStateChangedEventArgs(oldState, newState);
                OnAgentStateChanged?.Invoke(this, ev);
            }
        }

        void ProcessTeleStateChangedMessage(ConnectionInfo _, ServerSentMessage msg)
        {
            TeleState oldState;
            TeleState newState = (TeleState)msg.N1;
            logger.DebugFormat("TeleStateChanged - {0} --> {1}", teleState, newState);
            TeleStateChangedEventArgs ev = null;
            lock (this)
            {
                oldState = teleState;
                if (oldState != newState)
                {
                    teleState = newState;
                    ev = new TeleStateChangedEventArgs(oldState, newState);
                }
                // 如果挂机了， Call List 必须清空
                if (newState == TeleState.OnHook)
                {
                    callCollection.Clear();
                }
            }
            if (ev != null)
            {
                OnTeleStateChanged?.Invoke(this, ev);
            }
        }

        readonly static QueueEventType[] aliveQueueEventTypes = { QueueEventType.Join, QueueEventType.Wait };

        void ProcessQueueInfoMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var queueInfo = new QueueInfo(connInfo, msg, groupCollection);
            bool isAlive = aliveQueueEventTypes.Any(x => x == queueInfo.EventType);
            logger.DebugFormat("QueueInfoMessage - {0}", queueInfo);
            // 记录 QueueInfo
            lock (this)
            {
                // 修改之前,一律先删除
                queueInfoCollection.RemoveWhere(x => x == queueInfo);
                if (isAlive)
                    queueInfoCollection.Add(queueInfo);
            }
            OnQueueInfo?.Invoke(this, new QueueInfoEventArgs(connInfo, queueInfo));
        }

        void ProcessHoldInfoMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var channel = msg.N1;
            var holdEventType = (HoldEventType)msg.N2;
            var callInfo = new CallInfo(connInfo, channel, msg.S)
            {
                IsHeld = holdEventType != HoldEventType.Cancel,
                HoldType = holdEventType,
            };
            logger.DebugFormat("HoldInfoMessage - {0}: {1}", callInfo.IsHeld ? "UnHold" : "Hold", callInfo);
            // 改写 Call Collection
            lock (this)
            {
                callCollection.RemoveWhere(x => x == callInfo);
                callCollection.Add(callInfo);
            }
            OnHoldInfo?.Invoke(this, new HoldInfoEventArgs(connInfo, callInfo));
        }

        void ProcessDataMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            switch ((ServerSentMessageSubType)msg.N1)
            {
                case ServerSentMessageSubType.AgentId:
                    DoOnAgentId(connInfo, msg);
                    break;
                case ServerSentMessageSubType.Channel:
                    DoOnChannel(connInfo, msg);
                    break;
                case ServerSentMessageSubType.SipRegistrarList:
                    DoOnSipRegistrarList(connInfo, msg);
                    break;
                case ServerSentMessageSubType.GroupIdList:
                    DoOnGroupIdList(connInfo, msg);
                    break;
                case ServerSentMessageSubType.GroupNameList:
                    DoOnGroupNameList(connInfo, msg);
                    break;
                case ServerSentMessageSubType.PrivilegeList:
                    DoOnPrivilegeList(connInfo, msg);
                    break;
                case ServerSentMessageSubType.PrivilegeExternList:
                    DoOnPrivilegeExternList(connInfo, msg);
                    break;
                case ServerSentMessageSubType.SignedGroupIdList:
                    DoOnSignedGroupIdList(connInfo, msg);
                    break;
                case ServerSentMessageSubType.WorkingChannel:
                    DoOnWorkingChannel(connInfo, msg);
                    break;
                case ServerSentMessageSubType.Ring:
                    DoOnRing(connInfo, msg);
                    break;
                case ServerSentMessageSubType.IvrData:
                    DoOnIvrData(connInfo, msg);
                    break;
                case ServerSentMessageSubType.CustomString:
                    DoOnCustomString(connInfo, msg);
                    break;
                default:
                    break;
            }
        }

        public event EventHandler OnSignedGroupsChanged;
        private void DoOnSignedGroupIdList(ConnectionInfo _, ServerSentMessage msg)
        {
            var signed = Convert.ToBoolean(msg.N2);
            var ids = msg.S.Split(Constants.VerticalBarDelimiter);
            lock (this)
            {
                foreach (var id in ids)
                {
                    var groupObj = groupCollection.FirstOrDefault(m => m.Id == id);
                    if (groupObj == null)
                    {
                        logger.ErrorFormat("DoOnSignedGroupIdList - 技能组 <id=\"{0}\"> 不存在", id);
                    }
                    else
                    {
                        logger.DebugFormat("DoOnSignedGroupIdList - 技能组 <id=\"{0}\" signed={1}>", id, signed);
                        groupObj.Signed = signed;
                    }
                }
            }
            OnSignedGroupsChanged?.Invoke(this, new EventArgs());
        }

        public event SipRegistrarListReceivedEventHandler OnSipRegistrarListReceived;
        public event SipRegisterStateChangedEventHandler OnSipRegisterStateChanged;
        private void DoOnSipRegistrarList(ConnectionInfo connectionInfo, ServerSentMessage msg)
        {
            var connectionIndex = GetConnetionIndex(connectionInfo);
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var val = msg.S.Split(Constants.VerticalBarDelimiter);
            var evt = new SipRegistrarListReceivedEventArgs(val);

            SyncFactory.StartNew(() =>
            {
                lock (this)
                {
                    foreach (var addr in evt.Value)
                    {
                        logger.DebugFormat("处理 SipAccount 地址 {0} ...", addr);
                        var uri = $"sip:{workerNumber}@{addr}";
                        // 这个地址是不是已经注册了? 如果是的，目前暂定不要重新注册?
                        var existedAcc = (
                            from acc in sipAccounts
                            where acc.isValid()
                            let accInfo = acc.getInfo()
                            where accInfo.regIsConfigured && accInfo.uri == uri
                            select acc
                        ).FirstOrDefault();
                        if (existedAcc == null)
                        {
                            logger.DebugFormat("SipAccount 帐户 {0} 尚不存在，新建 ...", uri);
                            using (var sipAuthCred = new AuthCredInfo("digest", "*", workerNumber, 0, "hesong"))
                            using (var cfg = new AccountConfig { idUri = uri })
                            {
                                cfg.regConfig.timeoutSec = 60;
                                cfg.regConfig.retryIntervalSec = 30;
                                cfg.regConfig.randomRetryIntervalSec = 10;
                                cfg.regConfig.firstRetryIntervalSec = 15;
                                cfg.regConfig.registrarUri = $"sip:{addr}";
                                cfg.sipConfig.authCreds.Add(sipAuthCred);
                                var acc = new Sip.Account(connectionIndex);
                                acc.OnRegisterStateChanged += Acc_OnRegisterStateChanged;
                                acc.OnIncomingCall += Acc_OnIncomingCall;
                                acc.OnCallDisconnected += Acc_OnCallDisconnected;
                                acc.create(cfg);
                                sipAccounts.Add(acc);
                            }
                        }
                        else
                        {
                            logger.DebugFormat("SipAccount 帐户 {0} 已经存在，重注册 ...", uri);
                            try
                            {
                                existedAcc.setRegistration(true);
                            }
                            catch (Exception exce)
                            {
                                logger.ErrorFormat("SipAccount 帐户 {0} 重注册错误:\r\n{1}", uri, exce);
                            }
                        }
                    }
                    ReloadSipAccountCollection();
                }
            }).Wait();

            OnSipRegistrarListReceived?.Invoke(this, evt);
        }

        private void ReloadSipAccountCollection()
        {
            sipAccountCollection.Clear();
            sipAccountCollection.UnionWith(
                from m in sipAccounts
                select new SipAccountInfo(m)
            );
        }


        private void Acc_OnRegisterStateChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ReloadSipAccountCollection();
            }
            OnSipRegisterStateChanged?.Invoke(this, new EventArgs());
        }

        private void Acc_OnIncomingCall(object sender, Sip.IncomingCallEventArgs e)
        {
            var call = e.Call;
            if (currentCall != null)
            {
                logger.WarnFormat("新来的呼叫 - 因为当前呼叫已经存在，所以拒绝新来的呼叫 {0}", call);
                using (var cop = new CallOpParam { statusCode = pjsip_status_code.PJSIP_SC_BUSY_HERE })
                {
                    call.hangup(cop);
                }
                return;
            }
            logger.DebugFormat("新来的呼叫 - 设置当前呼叫为 {0}", call);
            currentCall = e.Call;
            bool isInRequest = false;
            lock (this)
            {
                if (isOffHookRequesting)
                {
                    isOffHookRequesting = false;
                    isInRequest = true;
                }
                if (isInRequest)
                {
                    isOffHookRequesting = false;
                    logger.DebugFormat("新来的呼叫 - IsOffHookRequesting is true, 主动应答 {0}", call);
                    using (var cop = new CallOpParam { statusCode = pjsip_status_code.PJSIP_SC_OK })
                    {
                        currentCall.answer(cop);
                    }
                }
                ReloadSipAccountCollection();
            }
        }

        private void Acc_OnCallDisconnected(object sender, EventArgs e)
        {
            logger.Debug("呼叫断开，清空 “当前呼叫” 变量");
            currentCall = null;
            lock (this)
            {
                isOffHookRequesting = false;
                ReloadSipAccountCollection();
            }
        }

        public event CustomStringReceivedEventArgsReceivedEventHandler OnCustomStringReceived;
        private void DoOnCustomString(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var val = new ServerSentCustomString(connInfo, msg.N2, msg.S);
            var evt = new CustomStringReceivedEventArgs(connInfo, val);
            OnCustomStringReceived?.Invoke(this, evt);
        }

        public event IvrDataReceivedEventHandler OnIvrDataReceived;
        private void DoOnIvrData(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var val = new IvrData(connInfo, msg.N2, msg.S);
            var evt = new IvrDataReceivedEventArgs(connInfo, val);
            OnIvrDataReceived?.Invoke(this, evt);
        }

        public event RingInfoReceivedEventHandler OnRingInfoReceived;
        private void DoOnRing(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var workChInfo = new WorkingChannelInfo(msg.N2);
            var callInfo = new CallInfo(connInfo, workChInfo.Channel, msg.S);
            logger.DebugFormat("OnRing - {0}", callInfo);
            lock (this)
            {
                workingChannelInfo = workChInfo;
                callCollection.Add(callInfo);
            }
            OnWorkingChannelInfoReceived?.Invoke(this,
                new WorkingChannelInfoReceivedEventArgs(connInfo, workChInfo));
            OnRingInfoReceived?.Invoke(this,
                new RingInfoReceivedEventArgs(connInfo, callInfo));
        }

        WorkingChannelInfo workingChannelInfo;
        public WorkingChannelInfo WorkingChannel => workingChannelInfo;
        public event WorkingChannelInfoReceivedEventHandler OnWorkingChannelInfoReceived;
        private void DoOnWorkingChannel(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var info = new WorkingChannelInfo(msg.N2, msg.S);
            logger.DebugFormat("OnWorkingChannel - {0}: {1}", connInfo, info.Channel);
            lock (this)
            {
                workingChannelInfo = info;
            }
            OnWorkingChannelInfoReceived?.Invoke(this,
                new WorkingChannelInfoReceivedEventArgs(connInfo, info));
        }

        private readonly HashSet<Privilege> privilegeCollection = new HashSet<Privilege>();
        public IReadOnlyCollection<Privilege> PrivilegeCollection => privilegeCollection;
        public event EventHandler OnPrivilegeCollectionReceived;
        private void DoOnPrivilegeList(ConnectionInfo _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var values = from s in msg.S.Split(Constants.VerticalBarDelimiter)
                         select (Privilege)Enum.Parse(typeof(Privilege), s);
            lock (this)
            {
                privilegeCollection.UnionWith(values);
            }
            OnPrivilegeCollectionReceived?.Invoke(this, new EventArgs());
        }

        private readonly HashSet<int> privilegeExternCollection = new HashSet<int>();
        public IReadOnlyCollection<int> PrivilegeExternCollection => privilegeExternCollection;
        public event EventHandler OnPrivilegeExternCollectionReceived;
        private void DoOnPrivilegeExternList(ConnectionInfo _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var values = from s in msg.S.Split(Constants.VerticalBarDelimiter)
                         select int.Parse(s);
            lock (this)
            {
                privilegeExternCollection.UnionWith(values);
            }
            OnPrivilegeExternCollectionReceived?.Invoke(this, new EventArgs());
        }

        readonly List<AgentGroup> groupCollection = new List<AgentGroup>();
        public IReadOnlyCollection<AgentGroup> GroupCollection => groupCollection;
        public event EventHandler OnGroupCollectionReceived;
        void DoOnGroupIdList(ConnectionInfo _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var groups = from s in msg.S.Split(Constants.VerticalBarDelimiter)
                         select new AgentGroup(s);
            lock (this)
            {
                groupCollection.Clear();
                groupCollection.AddRange(groups);
            }
        }

        void DoOnGroupNameList(ConnectionInfo _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var names = msg.S.Split(Constants.VerticalBarDelimiter);
            lock (this)
            {
                foreach (var pair in groupCollection.Zip(names, (first, second) => (first, second)))
                {
                    pair.first.Name = pair.second;
                }
            }
            OnGroupCollectionReceived?.Invoke(this, new EventArgs());
        }

        public event ChannelAssignedEventHandler OnChannelAssigned;
        void DoOnChannel(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var evt = new ChannelAssignedEventArgs(connInfo, msg.N2);
            lock (this)
            {
                agentChannel = evt.Value;
            }
            OnChannelAssigned?.Invoke(this, evt);
        }

        public event AgentDisplayNameReceivedEventHandler OnAgentDisplayNameReceived;
        void DoOnAgentId(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var evt = new AgentDisplayNameReceivedEventArgs(connInfo, msg.S);
            lock (this)
            {
                displayName = evt.Value;
            }
            OnAgentDisplayNameReceived?.Invoke(this, evt);
        }

        public event ConnectionInfoStateChangedEventHandler OnConnectionStateChanged;
        void Conn_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            ConnectionState[] disconntedStates = { ConnectionState.Lost, ConnectionState.Failed, ConnectionState.Closed };
            AgentState[] workingState = { AgentState.Ring, AgentState.Work, AgentState.WorkPause };
            var conn = sender as Connection;
            var connIdx = internalConnections.IndexOf(conn);
            var connInfo = connectionList[connIdx];
            var rand = new Random();
            var evtStateChanged = new ConnectionInfoStateChangedEventArgs(connInfo, e.OldState, e.NewState);
            EventArgs evtMainConnChanged = null;
            Action action = null;
            logger.DebugFormat("{0}: {1} --> {2}", conn, e.OldState, e.NewState);
            lock (this)
            {
                //////////
                // 断线处理
                if (disconntedStates.Any(x => x == e.NewState))
                {
                    var isMain = connIdx == mainConnectionIndex;
                    if (isMain)
                    {
                        if (runningState != AgentRunningState.Started)
                        {
                            logger.WarnFormat(
                                "主服务节点 [{0}]({1}) 连接断开 (NewState={2} RunningState={3}) 放弃重连.",
                                connIdx, connectionList[connIdx], e.NewState, runningState
                            );
                        }
                        else
                        {
                            // 其它连接还有连接上了的吗?
                            var indices = (
                                from vi in internalConnections.Select((value, index) => (value, index))
                                where vi.index != mainConnectionIndex && vi.value.State == ConnectionState.Ok
                                select vi.index
                            ).ToList();
                            if (indices.Count > 0)
                            {
                                // 有的选,但是如果在工作状态,不能变!
                                if (workingState.Any(x => x == AgentState))
                                {
                                    logger.WarnFormat("主服务节点 [{0}]({1}) 连接断开. 由于处于工作状态, 将继续使用该主节点并发起重连", connIdx, connInfo);
                                }
                                else
                                {
                                    mainConnectionIndex = indices[rand.Next(indices.Count)];
                                    logger.WarnFormat(
                                        "主服务节点 [{0}]({1}) 连接断开. 切换主节点到 [{2}]({3})",
                                        connIdx, connInfo, mainConnectionIndex, MainConnectionInfo
                                    );
                                }
                            }
                            else
                            {
                                // 没得选
                                switch (e.NewState)
                                {
                                    case ConnectionState.Closed:
                                        {
                                            // 被主动关闭
                                            ConnectionState[] exceptStats = { ConnectionState.Closed, ConnectionState.Closing };
                                            var indices2 = (
                                                from vi in internalConnections.Select((value, index) => (value, index))
                                                where vi.index != mainConnectionIndex && exceptStats.All(m => m != vi.value.State)
                                                select vi.index
                                            ).ToList();
                                            if (indices2.Count > 0)
                                            {
                                                mainConnectionIndex = indices2[rand.Next(indices2.Count)];
                                                logger.WarnFormat(
                                                    "主服务节点 [{0}]({1}) 连接被主动关闭. 虽然找不到其它可用的连接, 仍切换主节点到 [{2}]({3})",
                                                    connIdx, connInfo, mainConnectionIndex, MainConnectionInfo
                                                );
                                            }
                                            else
                                            {
                                                logger.WarnFormat(
                                                    "主服务节点 [{0}]({1}) 连接被主动关闭. 且找不到其它未被主动关闭的连接, 将继续使用该主节点并发起重连",
                                                    connIdx, connInfo
                                                );
                                            }
                                        }
                                        break;
                                    default:
                                        logger.WarnFormat(
                                            "主服务节点 [{0}]({1}) 连接断开. 但是由于找不到其它可用服务节点连接, 将继续使用该主节点并发起重连",
                                            connIdx, connInfo
                                        );
                                        break;
                                }
                            }
                            ///
                            if (mainConnectionIndex != connIdx)
                            {
                                action = new Action(async () =>
                                {
                                    logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接 ... ", connIdx, connInfo);
                                    try
                                    {
                                        await conn.Open(connInfo.Host, connInfo.Port, workerNumber, password, flag: 0);
                                        logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接成功", connIdx, connInfo);
                                    }
                                    catch (ConnectionException ex)
                                    {
                                        logger.ErrorFormat("从服务节点 [{0}]({1}) 重新连接异常: {2}", connIdx, connInfo, $"{ex.GetType()} {ex.Message}");
                                    };
                                });
                                // 需要抛出切换新的主服务节事件
                                evtMainConnChanged = new EventArgs();
                            }
                            else
                            {
                                action = new Action(async () =>
                                {
                                    logger.InfoFormat("主服务节点 [{0}]({1}) 重新连接 ... ", connIdx, connInfo);
                                    try
                                    {
                                        await conn.Open(connInfo.Host, connInfo.Port, workerNumber, password, flag: 1);
                                        logger.InfoFormat("主服务节点 [{0}]({1}) 重新连接成功", connIdx, connInfo);
                                    }
                                    catch (ConnectionException ex)
                                    {
                                        logger.ErrorFormat("主服务节点 [{0}]({1}) 重新连接异常: {2}", connIdx, connInfo, $"{ex.GetType()} {ex.Message}");
                                    };
                                });
                            }
                        }
                    }
                    else
                    {
                        if (runningState != AgentRunningState.Started)
                        {
                            logger.WarnFormat(
                                "从服务节点 [{0}]({1}) 连接断开 ({2} {3}). 放弃重连.",
                                connIdx, connInfo, e.NewState, runningState
                            );
                        }
                        else
                        {
                            logger.WarnFormat("从服务节点 [{0}]({1}) 连接丢失. 将发起重连", connIdx, connInfo);
                            action = new Action(async () =>
                            {
                                logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接 ... ", connIdx, connInfo);
                                try
                                {
                                    await conn.Open(connInfo.Host, connInfo.Port, workerNumber, password, flag: 0);
                                    logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接成功", connIdx, connInfo);
                                }
                                catch (ConnectionException ex)
                                {
                                    logger.ErrorFormat("从服务节点 [{0}]({1}) 重新连接异常: {2}", connIdx, connInfo, $"{ex.GetType()} {ex.Message}");
                                }
                            });
                        }
                    }
                }
            }
            // 抛出连接状态变化事件
            OnConnectionStateChanged?.Invoke(this, evtStateChanged);
            // 抛出切换新的主服务节事件
            if (evtMainConnChanged != null)
            {
                OnMainConnectionChanged?.Invoke(this, evtMainConnChanged);
            }
            // 执行重连
            if (action != null)
            {
                Task.Run(action);
            }
        }

        public async Task StartUp(string workerNumber, string password)
        {
            AgentRunningState savedRunningState;
            IEnumerable<int> minorIndices;
            var rand = new Random();
            lock (this)
            {
                if (runningState != AgentRunningState.Stopped)
                {
                    throw new InvalidOperationException($"Invalid state: {runningState}");
                }
                savedRunningState = runningState;
                runningState = AgentRunningState.Starting;
            }
            // 首先，主节点
            try
            {
                mainConnectionIndex = rand.Next(0, connectionList.Count);
                minorIndices = from i in Enumerable.Range(0, connectionList.Count)
                               where i != mainConnectionIndex
                               select i;
                this.workerNumber = workerNumber;
                this.password = password;
                logger.InfoFormat("主服务节点 [{0}]({1}|{2}) 首次连接 ... ", mainConnectionIndex, MainConnectionInfo.Host, MainConnectionInfo.Port);
                await MainConnection.Open(
                    MainConnectionInfo.Host, MainConnectionInfo.Port,
                    workerNumber, password,
                    flag: 1
                );
                lock (this)
                {
                    runningState = AgentRunningState.Started;
                }
            }
            catch
            {
                lock (this)
                {
                    runningState = savedRunningState;
                }
                throw;
            }
            // 然后其他节点
            var tasks = from i in minorIndices
                        select internalConnections[i].Open(
                            connectionList[i].Host, connectionList[i].Port,
                            workerNumber, password,
                            flag: 0
                        );
            await Task.WhenAll(tasks);
        }

        public async Task ShutDown(bool force = false)
        {
            AgentRunningState savedRunningState;
            var graceful = !force;
            bool isMainNotConnected;
            lock (this)
            {
                if (runningState != AgentRunningState.Started)
                {
                    throw new InvalidOperationException($"Invalid state: {runningState}");
                }
                savedRunningState = runningState;
                runningState = AgentRunningState.Stopping;
                isMainNotConnected = !MainConnection.Connected;
            }
            try
            {
                // 首先，主节点
                if (isMainNotConnected)
                {
                    logger.WarnFormat("主节点连接 {0} 已经关闭！", MainConnection, graceful);
                }
                else
                {
                    logger.DebugFormat("关闭主节点连接 {0} graceful={1}...", MainConnection, graceful);
                    await MainConnection.Close(graceful, flag: 1);
                    logger.DebugFormat("关闭主节点连接 {0} graceful={1} 完毕.", MainConnection, graceful);
                }

                // 然后其他节点
                var itConnObj =
                    from x in internalConnections.Select((value, index) => (value, index))
                    where x.index != mainConnectionIndex
                    select x.value;
                await Task.WhenAll(
                    from conn in itConnObj
                    select Task.Run(async () =>
                    {
                        if (graceful)
                        {
                            try
                            {
                                logger.DebugFormat("关闭从节点连接 {0} graceful={1}...", conn, graceful);
                                await conn.Close(graceful, flag: 0);
                                logger.DebugFormat("关闭从节点连接 {0} graceful={1} 完毕.", conn, graceful);
                            }
                            catch (Exception exce)
                            {
                                switch (exce)
                                {
                                    case DisconnectionTimeoutException _:
                                    case ErrorResponse _:
                                    case InvalidOperationException _:
                                        logger.DebugFormat("关闭从节点连接 {0} 失败, 将强行关闭: {1}", conn, exce);
                                        await conn.Close(graceful: false, flag: 0);
                                        break;
                                    default:
                                        throw;
                                }
                            }
                        }
                        else
                        {
                            await conn.Close(graceful: false, flag: 0);
                        }
                    })
                );
                lock (this)
                {
                    runningState = AgentRunningState.Stopped;
                }
            }
            catch
            {
                lock (this)
                {
                    runningState = savedRunningState;
                }
                throw;
            }

            // release Sip Accounts
            lock (this)
            {
                foreach (var sipAcc in sipAccounts)
                {
                    logger.DebugFormat("Dispose {0}", sipAcc);
                    sipAcc.Dispose();
                }
                sipAccounts.Clear();
            }

        }

        public bool HasPendingRequest => internalConnections.Any(x => x.HasPendingRequest);

        public async Task<ServerSentMessage> Request(int connectionIndex, AgentRequestMessage args, int timeout = Connection.DefaultRequestTimeoutMilliseconds)
        {
            var conn = internalConnections[connectionIndex];
            return await conn.Request(args, timeout);
        }

        public async Task<ServerSentMessage> Request(AgentRequestMessage args, int timeout = Connection.DefaultRequestTimeoutMilliseconds)
        {
            return await MainConnection.Request(args, timeout);
        }

        public async Task SignIn()
        {
            await SignIn(Array.Empty<string>());
        }
        public async Task SignIn(string id)
        {
            var ids = new string[] { id };
            await SignIn(ids);
        }
        public async Task SignIn(IEnumerable<string> ids)
        {
            var s = string.Join("|", ids);
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_SIGNON, s);
            await MainConnection.Request(req);
        }

        public async Task SignOut()
        {
            await SignOut(Array.Empty<string>());
        }
        public async Task SignOut(string id)
        {
            var ids = new string[] { id };
            await SignOut(ids);
        }
        public async Task SignOut(IEnumerable<string> ids)
        {
            var s = string.Join("|", ids);
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_SIGNOFF, s);
            await MainConnection.Request(req);
        }

        public async Task SetIdle()
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_CONTINUE);
            await MainConnection.Request(req);
        }

        public async Task SetBusy(WorkType workType = WorkType.PauseBusy)
        {
            if (workType < WorkType.PauseBusy)
            {
                throw new ArgumentOutOfRangeException(nameof(workType), $"Invalid work type {workType}");
            }
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_PAUSE, (int)workType);
            await MainConnection.Request(req);
        }

        public async Task Intercept(ConnectionInfo _, int agentId)
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_INTERCEPT, agentId);
            await MainConnection.Request(req);
            throw new NotImplementedException();
        }

        public async Task Dial(string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            var s = $"{calledTelnum}|{callingTelnum}|{channelGroup}|{option}";
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_DIAL, 0, s);
            await MainConnection.Request(req);
        }

        public async Task Xfer(ConnectionInfo connectionInfo, int channel, string groupId, string workerNum = "", string customString = "")
        {
            var conn = GetConnection(connectionInfo);
            var s = $"{workerNum}|{groupId}|{customString}";
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_TRANSFER, channel, s);
            await conn.Request(req);
        }

        public async Task Xfer(int connectionIndex, int channel, string groupId, string workerNum = "", string customString = "")
        {
            var connectionInfo = connectionList[connectionIndex];
            await Xfer(connectionInfo, channel, groupId, workerNum, customString);
        }

        public async Task Xfer(CallInfo callInfo, string groupId, string workerNum = "", string customString = "")
        {
            await Xfer(callInfo.ConnectionInfo, callInfo.Channel, groupId, workerNum, customString);
        }

        public async Task Xfer(string groupId, string workerNum = "", string customString = "")
        {
            CallInfo callInfo = HeldCallCollection.First();
            await Xfer(callInfo, groupId, workerNum, customString);
        }

        public async Task XferConsult(string groupId, string workerNum = "", string customString = "")
        {
            CallInfo callInfo = HeldCallCollection.FirstOrDefault();
            var conn = (callInfo == null) ? MainConnection : GetConnection(callInfo.ConnectionInfo);
            var s = $"{workerNum}|{groupId}|{customString}";
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_CONSULT, -1, s);
            await conn.Request(req);
        }

        public async Task XferExt(ConnectionInfo connectionInfo, int channel, string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            var conn = GetConnection(connectionInfo);
            var s = $"{calledTelnum}|{callingTelnum}|{channelGroup}|{option}";
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_TRANSFER_EX, channel, s);
            await conn.Request(req);
        }
        public async Task XferExt(int connectionIndex, int channel, string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            var connectionInfo = connectionList[connectionIndex];
            await XferExt(connectionInfo, channel, calledTelnum, callingTelnum, channelGroup, option);
        }

        public async Task XferExt(CallInfo callInfo, string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            var connectionInfo = callInfo.ConnectionInfo;
            var channel = callInfo.Channel;
            await XferExt(connectionInfo, channel, calledTelnum, callingTelnum, channelGroup, option);
        }

        public async Task XferExt(string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            CallInfo callInfo = HeldCallCollection.First();
            await XferExt(callInfo, calledTelnum, callingTelnum, channelGroup, option);
        }

        public async Task XferExtConsult(string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            CallInfo callInfo = HeldCallCollection.First();
            var conn = GetConnection(callInfo.ConnectionInfo);
            var s = $"{calledTelnum}|{callingTelnum}|{channelGroup}|{option}";
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_CONSULT_EX, -1, s);
            await conn.Request(req);
        }

        public async Task CallIvr(ConnectionInfo connectionInfo, int channel, string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            var conn = GetConnection(connectionInfo);
            var s = $"{ivrId}|{(int)invokeType}|{customString}";
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_CALLSUBFLOW, channel, s);
            await conn.Request(req);
        }

        public async Task CallIvr(int connectionIndex, int channel, string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            var connectionInfo = connectionList[connectionIndex];
            await CallIvr(connectionInfo, channel, ivrId, invokeType, customString);
        }

        public async Task CallIvr(CallInfo callInfo, string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            var connectionInfo = callInfo.ConnectionInfo;
            var channel = callInfo.Channel;
            await CallIvr(connectionInfo, channel, ivrId, invokeType, customString);
        }

        public async Task CallIvr(string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            CallInfo callInfo;
            lock (this)
            {
                callInfo = (
                    from item in callCollection
                    where !item.IsHeld
                    select item
                ).FirstOrDefault();
            }
            if (callInfo == null)
            {
                await CallIvr(MainConnectionInfo, agentChannel, ivrId, invokeType, customString);
            }
            else
            {
                await CallIvr(callInfo, ivrId, invokeType, customString);
            }
        }

        public async Task Hold()
        {
            CallInfo callInfo;
            lock (this)
            {
                callInfo = (
                    from item in callCollection
                    where !item.IsHeld
                    select item
                ).First();
            }
            var conn = GetConnection(callInfo.ConnectionInfo);
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_HOLD);
            await conn.Request(req);
        }

        public async Task UnHold(ConnectionInfo connectionInfo, int channel)
        {
            var conn = GetConnection(connectionInfo);
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_RETRIEVE, channel);
            await conn.Request(req);
        }

        public async Task UnHold(int connectionIndex, int channel)
        {
            var connectionInfo = connectionList[connectionIndex];
            await UnHold(connectionInfo, channel);
        }

        public async Task UnHold(CallInfo callInfo)
        {
            var connectionInfo = callInfo.ConnectionInfo;
            var channel = callInfo.Channel;
            await UnHold(connectionInfo, channel);
        }

        public async Task UnHold()
        {
            var callInfo = HeldCallCollection.First();
            var connectionInfo = callInfo.ConnectionInfo;
            var channel = callInfo.Channel;
            await UnHold(connectionInfo, channel);
        }

        public async Task Break(int channel = -1, string customString = "")
        {
            if (channel < 0)
            {
                channel = workingChannelInfo.Channel;
            }
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_BREAK_SESS, channel, customString);
            await MainConnection.Request(req);
        }

        public async Task OnHook()
        {
            logger.Debug("OnHook - 本地呼叫拆线");
            await SyncFactory.StartNew(SipEndpoint.hangupAllCalls);
            // 主动挂机了， Call List 也来一个清空动作
            lock (this)
            {
                callCollection.Clear();
            }
        }

        private bool isOffHookRequesting = false;
        public bool IsOffHookRequesting => isOffHookRequesting;

        public async Task OffHook()
        {
            logger.Debug("OffHook - 请求服务端回呼");
            if (currentCall == null)
            {
                lock (this)
                {
                    if (isOffHookRequesting)
                        throw new InvalidOperationException();
                    isOffHookRequesting = true;
                }
                try
                {
                    var req = new AgentRequestMessage(MessageType.REMOTE_MSG_OFFHOOK);
                    await MainConnection.Request(req);
                }
                catch
                {
                    lock (this)
                    {
                        isOffHookRequesting = false;
                    }
                    throw;
                }
            }
            else
            {
                logger.Debug("OffHook - 本地呼叫应答");
                await SyncFactory.StartNew(() =>
                {
                    using (var cop = new CallOpParam { statusCode = pjsip_status_code.PJSIP_SC_OK })
                    {
                        currentCall.answer(cop);
                    }
                });
            }
        }

        public async Task Dequeue(QueueInfo queueInfo)
        {
            var conn = GetConnection(queueInfo.ConnectionInfo);
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_GETQUEUE, queueInfo.Channel);
            await conn.Request(req);
        }

        public async Task Interrupt(int agentId)
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_FORCEINSERT, agentId);
            await MainConnection.Request(req);
        }

        public async Task Monitor(int agentId)
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_LISTEN, agentId);
            await MainConnection.Request(req);
        }

        public async Task UnMonitor(int agentId)
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_STOPLISTEN, agentId);
            await MainConnection.Request(req);
        }


        public async Task Block(int agentId)
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_BLOCK, agentId);
            await MainConnection.Request(req);
        }

        public async Task UnBlock(int agentId)
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_UNBLOCK, agentId);
            await MainConnection.Request(req);
        }

        public async Task Kick(int agentId)
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_KICKOUT, agentId);
            await MainConnection.Request(req);
        }

        public async Task SignOut(int agentId, string groupId)
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_FORCESIGNOFF, agentId, groupId);
            await MainConnection.Request(req);
        }

        public async Task TakenAway(int index)
        {
            Connection connObj;
            // 先修改 index
            lock (this)
            {
                if (mainConnectionIndex < 0)
                {
                    throw new InvalidOperationException();
                }
                if (index < 0 || index >= internalConnections.Count || index == mainConnectionIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, "");
                }
                if (internalConnections[index].State != ConnectionState.Ok)
                {
                    throw new InvalidOperationException($"Invalid state: {internalConnections[index].State}");
                }
                connObj = MainConnection;
                mainConnectionIndex = index;
                logger.InfoFormat("切换主服务节点到 {0}", MainConnection);
            }
            OnMainConnectionChanged?.Invoke(this, new EventArgs());
            // 再通知原来的主，无论能否通知成功
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_TAKENAWAY);
            await connObj.Request(req);
        }

        readonly HashSet<Sip.Account> sipAccounts = new HashSet<Sip.Account>();
        readonly HashSet<SipAccountInfo> sipAccountCollection = new HashSet<SipAccountInfo>();
        public IReadOnlyCollection<SipAccountInfo> SipAccountCollection => sipAccountCollection;

    }
}
