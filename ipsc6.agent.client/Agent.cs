using org.pjsip.pjsua2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ipsc6.agent.client
{
    public class Agent : IDisposable
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Agent));

        public Agent(IEnumerable<CtiServer> ctiServers, ushort localPort = 0, string localAddress = "")
        {
            foreach (var ctiServer in ctiServers)
            {
                var conn = new Connection(localPort, localAddress);
                conn.OnConnectionStateChanged += Conn_OnConnectionStateChanged;
                conn.OnServerSentEvent += Conn_OnServerSend;
                connections.Add(conn);
                this.ctiServers.Add(ctiServer);
            }
        }

        public Agent(IEnumerable<string> addresses, ushort localPort = 0, string localAddress = "")
        {
            foreach (var s in addresses)
            {
                var ctiServer = new CtiServer(s);
                var conn = new Connection(localPort, localAddress);
                conn.OnConnectionStateChanged += Conn_OnConnectionStateChanged;
                conn.OnServerSentEvent += Conn_OnServerSend;
                connections.Add(conn);
                ctiServers.Add(ctiServer);
            }
        }

        // 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~Agent()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
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
                foreach (var sipAcc in sipAccountCollection)
                {
                    logger.DebugFormat("Dispose {0}", sipAcc);
                    sipAcc.Dispose();
                }
                sipAccountCollection.Clear();
                //
                foreach (var conn in connections)
                {
                    logger.DebugFormat("Dispose {0}", conn);
                    conn.Dispose();
                }
                connections.Clear();
                //
                disposedValue = true;
            }
        }

        internal static Endpoint SipEndpoint;
        internal static TaskFactory SyncFactory;

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

        private readonly RequestGuard requestGuard = new();

        public AgentRunningState RunningState { get; private set; } = AgentRunningState.Stopped;

        public string WorkerNum { get; private set; }

        private string password;

        private readonly List<Connection> connections = new();
        private readonly List<CtiServer> ctiServers = new();
        public IReadOnlyList<CtiServer> CtiServers => ctiServers;
        public int GetConnetionIndex(CtiServer connectionInfo) => ctiServers.IndexOf(connectionInfo);
        private Connection GetConnection(int index) => connections[index];
        private Connection GetConnection(CtiServer connectionInfo) => connections[GetConnetionIndex(connectionInfo)];
        public ConnectionState GetConnectionState(int index) => GetConnection(index).State;
        public ConnectionState GetConnectionState(CtiServer ctiServer)
        {
            var index = ctiServers.IndexOf(ctiServer);
            return connections[index].State;
        }

        public int MainConnectionIndex { get; private set; } = -1;
        private Connection mainConnection => connections[MainConnectionIndex];
        public CtiServer MainConnectionInfo => (MainConnectionIndex < 0) ? null : ctiServers[MainConnectionIndex];

        public event EventHandler OnMainConnectionChanged;
        public int AgentId => mainConnection.AgentId;

        public string DisplayName { get; private set; }
        public int AgentChannel { get; private set; } = -1;
        public AgentState AgentState { get; private set; } = AgentState.NotExist;
        public WorkType WorkType { get; private set; } = WorkType.Unknown;
        public AgentStateWorkType AgentStateWorkType
        {
            get => new(AgentState, WorkType);
            private set
            {
                AgentState = value.AgentState;
                WorkType = value.WorkType;
            }
        }
        public event EventHandler<AgentStateChangedEventArgs> OnAgentStateChanged;

        public TeleState TeleState { get; private set; } = TeleState.OnHook;
        public event EventHandler<TeleStateChangedEventArgs> OnTeleStateChanged;

        readonly HashSet<QueueInfo> queueInfos = new();
        public IReadOnlyCollection<QueueInfo> QueueInfos => queueInfos;
        public event EventHandler<QueueInfoEventArgs> OnQueueInfoReceived;

        public event EventHandler<HoldInfoEventArgs> OnHoldInfoReceived;

        readonly HashSet<CallInfo> calls = new();
        public IReadOnlyCollection<CallInfo> Calls => calls;

        public IReadOnlyCollection<CallInfo> HeldCalls
        {
            get
            {
                lock (this)
                {
                    return (
                        from m in calls
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
                var index = connections.IndexOf(conn);
                var ctiServer = ctiServers[index];
                switch (msg.Type)
                {
                    case MessageType.REMOTE_MSG_SETSTATE:
                        /// 状态改变
                        ProcessStateChangedMessage(ctiServer, msg);
                        break;
                    case MessageType.REMOTE_MSG_SETTELESTATE:
                        /// 电话状态改变
                        ProcessTeleStateChangedMessage(ctiServer, msg);
                        break;
                    case MessageType.REMOTE_MSG_QUEUEINFO:
                        /// 排队信息
                        ProcessQueueInfoMessage(ctiServer, msg);
                        break;
                    case MessageType.REMOTE_MSG_HOLDINFO:
                        /// 保持信息
                        ProcessHoldInfoMessage(ctiServer, msg);
                        break;
                    case MessageType.REMOTE_MSG_SENDDATA:
                        /// 其他各种数据
                        ProcessDataMessage(ctiServer, msg);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exce)
            {
                logger.ErrorFormat("OnServerSend - {0}: {1}\r\n{2}", sender, e.Message, exce);
                throw;
            }
        }

        private AgentStateWorkType cachedStateWorkType;

        void ProcessStateChangedMessage(CtiServer ctiServer, ServerSentMessage msg)
        {
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
            var currIndex = ctiServers.FindIndex(ci => ci == ctiServer);
            AgentStateWorkType oldState = null;
            Connection oldMainConnObj = null;
            lock (this)
            {
                if (AgentStateWorkType != newState)
                {
                    oldState = AgentStateWorkType.Clone() as AgentStateWorkType;
                    AgentStateWorkType = newState;
                    // 缓存状态
                    if (!isEverLostAllConnections)
                    {
                        if (AgentState is AgentState.Idle or AgentState.Pause
                            && availableSetBusyWorkTypes.Contains(WorkType))
                        {
                            cachedStateWorkType = newState;
                        }
                    }
                }
                // 如果是工作状态且主连接不是当前的，需要切换！
                if (newState.AgentState is AgentState.Ring or AgentState.Work
                    && currIndex != MainConnectionIndex)
                {
                    oldMainConnObj = mainConnection;
                    // 先修改 index
                    MainConnectionIndex = currIndex;
                    logger.InfoFormat("切换主服务节点到 {0}", mainConnection);
                }
            }

            if (oldMainConnObj != null)
            {
                // 通知原来的主，无论能否通知成功
                Task.Run(async () =>
                {
                    var req = new AgentRequestMessage(MessageType.REMOTE_MSG_TAKENAWAY);
                    await oldMainConnObj.RequestAsync(req);
                });
                // fire the event
                OnMainConnectionChanged?.Invoke(this, EventArgs.Empty);
            }
            if (oldState != null)
            {
                var ev = new AgentStateChangedEventArgs(oldState, newState);
                OnAgentStateChanged?.Invoke(this, ev);
            }
        }

        void ProcessTeleStateChangedMessage(CtiServer _, ServerSentMessage msg)
        {
            TeleState oldState;
            TeleState newState = (TeleState)msg.N1;
            logger.DebugFormat("TeleStateChanged - {0} --> {1}", TeleState, newState);
            TeleStateChangedEventArgs ev = null;
            lock (this)
            {
                oldState = TeleState;
                if (oldState != newState)
                {
                    TeleState = newState;
                    ev = new TeleStateChangedEventArgs(oldState, newState);
                }
                // 如果挂机了， Call List 必须清空
                if (newState == TeleState.OnHook)
                {
                    calls.Clear();
                }
                // Offhook 请求的对应的自动摘机
                if (IsOffHooking)
                {
                    switch (newState)
                    {
                        case TeleState.OffHook:
                            logger.Debug("TeleStateChangedMessage - Server side Offhooking Succees");
                            offHookServerTcs.SetResult(null);
                            break;
                        default:
                            logger.Debug("TeleStateChangedMessage - Server side Offhooking Fail");
                            offHookServerTcs.SetCanceled();
                            break;
                    }
                }
            }
            if (ev != null)
            {
                OnTeleStateChanged?.Invoke(this, ev);
            }
        }

        readonly static QueueEventType[] aliveQueueEventTypes = { QueueEventType.Join, QueueEventType.Wait };

        void ProcessQueueInfoMessage(CtiServer ctiServer, ServerSentMessage msg)
        {
            var queueInfo = new QueueInfo(ctiServer, msg, groups);
            bool isAlive = aliveQueueEventTypes.Contains(queueInfo.EventType);
            logger.DebugFormat("QueueInfoMessage - {0}", queueInfo);
            // 记录 QueueInfo
            lock (this)
            {
                // 修改之前,一律先删除
                queueInfos.Remove(queueInfo);
                if (isAlive)
                    queueInfos.Add(queueInfo);
            }
            OnQueueInfoReceived?.Invoke(this, new QueueInfoEventArgs(ctiServer, queueInfo));
        }

        void ProcessHoldInfoMessage(CtiServer ctiServer, ServerSentMessage msg)
        {
            var channel = msg.N1;
            var holdEventType = (HoldEventType)msg.N2;
            var callInfo = new CallInfo(ctiServer, channel, msg.S)
            {
                IsHeld = holdEventType != HoldEventType.Cancel,
                HoldType = holdEventType,
            };
            logger.DebugFormat("HoldInfoMessage - {0}: {1}", callInfo.IsHeld ? "UnHold" : "Hold", callInfo);
            // 改写 Call Collection(remove then add)
            lock (this)
            {
                calls.Remove(callInfo);
                calls.Add(callInfo);
            }
            OnHoldInfoReceived?.Invoke(this, new HoldInfoEventArgs(ctiServer, callInfo));
        }

        void ProcessDataMessage(CtiServer ctiServer, ServerSentMessage msg)
        {
            switch ((ServerSentMessageSubType)msg.N1)
            {
                case ServerSentMessageSubType.AgentId:
                    DoOnAgentId(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.Channel:
                    DoOnChannel(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.SipRegistrarList:
                    DoOnSipRegistrarList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.GroupIdList:
                    DoOnGroupIdList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.GroupNameList:
                    DoOnGroupNameList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.PrivilegeList:
                    DoOnPrivilegeList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.PrivilegeExternList:
                    DoOnPrivilegeExternList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.SignedGroupIdList:
                    DoOnSignedGroupIdList(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.WorkingChannel:
                    DoOnWorkingChannel(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.Ring:
                    DoOnRing(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.IvrData:
                    DoOnIvrData(ctiServer, msg);
                    break;
                case ServerSentMessageSubType.CustomString:
                    DoOnCustomString(ctiServer, msg);
                    break;
                default:
                    break;
            }
        }

        public event EventHandler OnSignedGroupsChanged;
        private void DoOnSignedGroupIdList(CtiServer _, ServerSentMessage msg)
        {
            var signed = Convert.ToBoolean(msg.N2);
            var ids = msg.S.Split(Constants.VerticalBarDelimiter);
            lock (this)
            {
                foreach (var id in ids)
                {
                    var groupObj = groups.FirstOrDefault(m => m.Id == id);
                    if (groupObj == null)
                    {
                        logger.ErrorFormat("DoOnSignedGroupIdList - 技能组 <id=\"{0}\"> 不存在", id);
                    }
                    else
                    {
                        logger.DebugFormat("DoOnSignedGroupIdList - 技能组 <id=\"{0}\" signed={1}>", id, signed);
                        groupObj.IsSigned = signed;
                    }
                }
            }
            OnSignedGroupsChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<SipRegistrarListReceivedEventArgs> OnSipRegistrarListReceived;
        public event EventHandler OnSipRegisterStateChanged;
        public event EventHandler<SipCallEventArgs> OnSipCallStateChanged;
        private void DoOnSipRegistrarList(CtiServer connectionInfo, ServerSentMessage msg)
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
                        var uri = $"sip:{WorkerNum}@{addr}";
                        Sip.Account acc;
                        acc = sipAccountCollection.FirstOrDefault(x => x.getInfo().uri == uri);
                        if (acc != null)
                        {
                            logger.DebugFormat("SipAccount 释放已存在帐户 {0} ...", uri);
                            sipAccountCollection.Remove(acc);
                            acc.Dispose();
                        }
                        logger.DebugFormat("SipAccount 新建帐户 {0} ...", uri);
                        using var sipAuthCred = new AuthCredInfo("digest", "*", WorkerNum, 0, "hesong");
                        using var cfg = new AccountConfig { idUri = uri };
                        cfg.regConfig.timeoutSec = 60;
                        cfg.regConfig.retryIntervalSec = 30;
                        cfg.regConfig.randomRetryIntervalSec = 10;
                        cfg.regConfig.firstRetryIntervalSec = 15;
                        cfg.regConfig.registrarUri = $"sip:{addr}";
                        cfg.sipConfig.authCreds.Add(sipAuthCred);
                        acc = new Sip.Account(connectionIndex);
                        acc.OnRegisterStateChanged += Acc_OnRegisterStateChanged;
                        acc.OnIncomingCall += Acc_OnIncomingCall;
                        acc.OnCallDisconnected += Acc_OnCallDisconnected;
                        acc.OnCallStateChanged += Acc_OnCallStateChanged;
                        acc.create(cfg);
                        sipAccountCollection.Add(acc);
                    }
                    ReloadSipAccountCollection();
                }
            }).Wait();

            OnSipRegistrarListReceived?.Invoke(this, evt);
        }

        private void ReloadSipAccountCollection()
        {
            sipAccounts.Clear();
            sipAccounts.UnionWith(
                from m in sipAccountCollection
                select new SipAccount(m)
            );
        }

        private void Acc_OnRegisterStateChanged(object sender, EventArgs e)
        {
            lock (this)
            {
                ReloadSipAccountCollection();
            }
            OnSipRegisterStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Acc_OnIncomingCall(object sender, Sip.CallEventArgs e)
        {
            SipCall callObj = new(e.Call);
            lock (this)
            {
                if (IsOffHooking)
                {
                    logger.Debug("TeleStateChangedMessage - Client side Offhooking: Answer ...");
                    try
                    {
                        using (var cop = new CallOpParam { statusCode = pjsip_status_code.PJSIP_SC_OK })
                        {
                            e.Call.answer(cop);
                        }
                        logger.Debug("TeleStateChangedMessage - Client side Offhooking: Answer ok");
                        offHookClientTcs.SetResult(null);
                    }
                    catch (Exception exce)
                    {
                        offHookClientTcs.SetException(exce);
                        throw;
                    }
                }
                ReloadSipAccountCollection();
            }
            OnSipCallStateChanged?.Invoke(this, new SipCallEventArgs(callObj));
        }

        private void Acc_OnCallStateChanged(object sender, Sip.CallEventArgs e)
        {
            SipCall callObj = new(e.Call);
            lock (this)
            {
                ReloadSipAccountCollection();
            }
            OnSipCallStateChanged?.Invoke(this, new SipCallEventArgs(callObj));
        }

        private void Acc_OnCallDisconnected(object sender, Sip.CallEventArgs e)
        {
            SipCall callObj = new(e.Call);
            lock (this)
            {
                ReloadSipAccountCollection();
            }
            OnSipCallStateChanged?.Invoke(this, new SipCallEventArgs(callObj));
        }

        public event EventHandler<CustomStringReceivedEventArgs> OnCustomStringReceived;
        private void DoOnCustomString(CtiServer ctiServer, ServerSentMessage msg)
        {
            var val = new ServerSentCustomString(ctiServer, msg.N2, msg.S);
            var evt = new CustomStringReceivedEventArgs(ctiServer, val);
            OnCustomStringReceived?.Invoke(this, evt);
        }

        public event EventHandler<IvrDataReceivedEventArgs> OnIvrDataReceived;
        private void DoOnIvrData(CtiServer ctiServer, ServerSentMessage msg)
        {
            var val = new IvrData(ctiServer, msg.N2, msg.S);
            var evt = new IvrDataReceivedEventArgs(ctiServer, val);
            OnIvrDataReceived?.Invoke(this, evt);
        }

        public event EventHandler<RingInfoReceivedEventArgs> OnRingInfoReceived;
        private void DoOnRing(CtiServer ctiServer, ServerSentMessage msg)
        {
            var workingChannelInfo = new WorkingChannelInfo(msg.N2);
            var callInfo = new CallInfo(ctiServer, workingChannelInfo.Channel, msg.S);
            logger.DebugFormat("OnRing - {0}", callInfo);
            lock (this)
            {
                WorkingChannelInfo = workingChannelInfo;
                calls.Remove(callInfo);
                calls.Add(callInfo);
            }
            OnWorkingChannelInfoReceived?.Invoke(this,
                new WorkingChannelInfoReceivedEventArgs(ctiServer, workingChannelInfo));
            OnRingInfoReceived?.Invoke(this,
                new RingInfoReceivedEventArgs(ctiServer, callInfo));
        }

        public WorkingChannelInfo WorkingChannelInfo { get; private set; }
        public event EventHandler<WorkingChannelInfoReceivedEventArgs> OnWorkingChannelInfoReceived;
        private void DoOnWorkingChannel(CtiServer ctiServer, ServerSentMessage msg)
        {
            var workingChannelInfo = new WorkingChannelInfo(msg.N2, msg.S);
            logger.DebugFormat("OnWorkingChannel - {0}: {1}", ctiServer, workingChannelInfo.Channel);
            lock (this)
            {
                WorkingChannelInfo = workingChannelInfo;
            }
            OnWorkingChannelInfoReceived?.Invoke(this,
                new WorkingChannelInfoReceivedEventArgs(ctiServer, workingChannelInfo));
        }

        private readonly HashSet<Privilege> privilegeCollection = new();
        public IReadOnlyCollection<Privilege> PrivilegeCollection => privilegeCollection;
        public event EventHandler OnPrivilegeCollectionReceived;
        private void DoOnPrivilegeList(CtiServer _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var values = from s in msg.S.Split(Constants.VerticalBarDelimiter)
                         select (Privilege)Enum.Parse(typeof(Privilege), s);
            lock (this)
            {
                privilegeCollection.UnionWith(values);
            }
            OnPrivilegeCollectionReceived?.Invoke(this, EventArgs.Empty);
        }

        private readonly HashSet<int> privilegeExternCollection = new();
        public IReadOnlyCollection<int> PrivilegeExternCollection => privilegeExternCollection;
        public event EventHandler OnPrivilegeExternCollectionReceived;
        private void DoOnPrivilegeExternList(CtiServer _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var values = from s in msg.S.Split(Constants.VerticalBarDelimiter)
                         select int.Parse(s);
            lock (this)
            {
                privilegeExternCollection.UnionWith(values);
            }
            OnPrivilegeExternCollectionReceived?.Invoke(this, EventArgs.Empty);
        }

        readonly List<Group> groups = new();
        public IReadOnlyList<Group> Groups => groups;
        public event EventHandler OnGroupReceived;

        List<Group> cachedGroups = new();

        void DoOnGroupIdList(CtiServer _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var itGroups = from s in msg.S.Split(Constants.VerticalBarDelimiter)
                           select new Group(s);
            lock (this)
            {
                groups.Clear();
                groups.AddRange(itGroups);
            }
        }

        void DoOnGroupNameList(CtiServer _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var names = msg.S.Split(Constants.VerticalBarDelimiter);
            bool isRestore = false;
            lock (this)
            {
                foreach (var pair in groups.Zip(names, (first, second) => (first, second)))
                {
                    pair.first.Name = pair.second;
                }
                if (isEverLostAllConnections)
                {
                    isRestore = true;
                    isEverLostAllConnections = false;
                }
            }
            OnGroupReceived?.Invoke(this, EventArgs.Empty);
            if (isRestore)
            {
                var groupIdList = (
                    from m in cachedGroups
                    where (
                        m.IsSigned
                        && groups.Any(n => n.Id == m.Id && !n.IsSigned)
                    )
                    select m.Id
                ).ToList();
                if (groupIdList.Count > 0)
                {
                    logger.Info("DoOnGroupNameList - 从丢失全部 CTI 节点的连接的情况中恢复，还原之前的技能与状态");
                    AgentStateWorkType stateWorkType = null;
                    if (cachedStateWorkType != null)
                        stateWorkType = cachedStateWorkType.Clone() as AgentStateWorkType;
                    Task.Run(async () =>
                    {
                        logger.Debug("还原技能");
                        await SignInAsync(groupIdList);
                        if (stateWorkType != null)
                        {
                            if (stateWorkType.AgentState == AgentState.Idle)
                            {
                                logger.Debug("还原示闲");
                                await SetIdleAsync();
                            }
                            else if (stateWorkType.AgentState == AgentState.Pause)
                            {
                                logger.DebugFormat("还原示忙 {0}", stateWorkType.WorkType);
                                await SetBusyAsync(stateWorkType.WorkType);
                            }
                        }
                    });
                }
            }
            else
            {
                cachedGroups = groups.ToList();
            }
        }

        public event EventHandler<ChannelAssignedEventArgs> OnChannelAssigned;
        void DoOnChannel(CtiServer ctiServer, ServerSentMessage msg)
        {
            var evt = new ChannelAssignedEventArgs(ctiServer, msg.N2);
            lock (this)
            {
                AgentChannel = evt.Value;
            }
            OnChannelAssigned?.Invoke(this, evt);
        }

        public event EventHandler<AgentDisplayNameReceivedEventArgs> OnAgentDisplayNameReceived;
        void DoOnAgentId(CtiServer ctiServer, ServerSentMessage msg)
        {
            var evt = new AgentDisplayNameReceivedEventArgs(ctiServer, msg.S);
            lock (this)
            {
                DisplayName = evt.Value;
            }
            OnAgentDisplayNameReceived?.Invoke(this, evt);
        }

        public event EventHandler<ConnectionInfoStateChangedEventArgs> OnConnectionStateChanged;

        static readonly ConnectionState[] disconntedStates = { ConnectionState.Lost, ConnectionState.Failed, ConnectionState.Closed };
        static readonly AgentState[] workingStates = { AgentState.Ring, AgentState.Work };

        private bool isEverLostAllConnections = false;
        void Conn_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            var conn = sender as Connection;
            var connIdx = connections.IndexOf(conn);
            var ctiServer = ctiServers[connIdx];
            var rand = new Random();
            var evtStateChanged = new ConnectionInfoStateChangedEventArgs(ctiServer, e.OldState, e.NewState);
            EventArgs evtMainConnChanged = null;
            Action action = null;
            logger.DebugFormat("{0}: {1} --> {2}", conn, e.OldState, e.NewState);
            lock (this)
            {
                if (RunningState == AgentRunningState.Started)
                {
                    if (!isEverLostAllConnections)
                    {
                        if (connections.All(x => !x.Connected))
                        {
                            isEverLostAllConnections = true;
                            logger.Warn("丢失了所有 CTI 服务节点的连接!");
                        }
                    }
                }
                //////////
                // 断线处理
                if (disconntedStates.Contains(e.NewState))
                {
                    var isMain = connIdx == MainConnectionIndex;
                    if (isMain)
                    {
                        if (RunningState != AgentRunningState.Started)
                        {
                            logger.WarnFormat(
                                "主服务节点 [{0}]({1}) 连接断开 (NewState={2} RunningState={3}) 放弃重连.",
                                connIdx, ctiServers[connIdx], e.NewState, RunningState
                            );
                        }
                        else
                        {
                            // 其它连接还有连接上了的吗?
                            var indices = (
                                from vi in connections.Select((value, index) => (value, index))
                                where vi.index != MainConnectionIndex && vi.value.State == ConnectionState.Ok
                                select vi.index
                            ).ToList();
                            if (indices.Count > 0)
                            {
                                // 有的选,但是如果在工作状态,不能变!
                                if (workingStates.Contains(AgentState))
                                {
                                    logger.WarnFormat("主服务节点 [{0}]({1}) 连接断开. 由于处于工作状态, 将继续使用该主节点并发起重连", connIdx, ctiServer);
                                }
                                else
                                {
                                    MainConnectionIndex = indices[rand.Next(indices.Count)];
                                    logger.WarnFormat(
                                        "主服务节点 [{0}]({1}) 连接断开. 切换主节点到 [{2}]({3})",
                                        connIdx, ctiServer, MainConnectionIndex, MainConnectionInfo
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
                                                from vi in connections.Select((value, index) => (value, index))
                                                where vi.index != MainConnectionIndex && exceptStats.All(m => m != vi.value.State)
                                                select vi.index
                                            ).ToList();
                                            if (indices2.Count > 0)
                                            {
                                                MainConnectionIndex = indices2[rand.Next(indices2.Count)];
                                                logger.WarnFormat(
                                                    "主服务节点 [{0}]({1}) 连接被主动关闭. 虽然找不到其它可用的连接, 仍切换主节点到 [{2}]({3})",
                                                    connIdx, ctiServer, MainConnectionIndex, MainConnectionInfo
                                                );
                                            }
                                            else
                                            {
                                                logger.WarnFormat(
                                                    "主服务节点 [{0}]({1}) 连接被主动关闭. 且找不到其它未被主动关闭的连接, 将继续使用该主节点并发起重连",
                                                    connIdx, ctiServer
                                                );
                                            }
                                        }
                                        break;
                                    default:
                                        logger.WarnFormat(
                                            "主服务节点 [{0}]({1}) 连接断开. 但是由于找不到其它可用服务节点连接, 将继续使用该主节点并发起重连",
                                            connIdx, ctiServer
                                        );
                                        break;
                                }
                            }
                            ///
                            if (MainConnectionIndex != connIdx)
                            {
                                action = new Action(async () =>
                                {
                                    logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接 ... ", connIdx, ctiServer);
                                    try
                                    {
                                        await conn.OpenAsync(ctiServer.Host, ctiServer.Port, WorkerNum, password, flag: 0);
                                        logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接成功", connIdx, ctiServer);
                                    }
                                    catch (ConnectionException ex)
                                    {
                                        logger.ErrorFormat("从服务节点 [{0}]({1}) 重新连接异常: {2}", connIdx, ctiServer, $"{ex.GetType()} {ex.Message}");
                                    };
                                });
                                // 需要抛出切换新的主服务节事件
                                evtMainConnChanged = EventArgs.Empty;
                            }
                            else
                            {
                                action = new Action(async () =>
                                {
                                    logger.InfoFormat("主服务节点 [{0}]({1}) 重新连接 ... ", connIdx, ctiServer);
                                    try
                                    {
                                        await conn.OpenAsync(ctiServer.Host, ctiServer.Port, WorkerNum, password, flag: 1);
                                        logger.InfoFormat("主服务节点 [{0}]({1}) 重新连接成功", connIdx, ctiServer);
                                    }
                                    catch (ConnectionException ex)
                                    {
                                        logger.ErrorFormat("主服务节点 [{0}]({1}) 重新连接异常: {2}", connIdx, ctiServer, $"{ex.GetType()} {ex.Message}");
                                    };
                                });
                            }
                        }
                    }
                    else
                    {
                        if (RunningState != AgentRunningState.Started)
                        {
                            logger.WarnFormat(
                                "从服务节点 [{0}]({1}) 连接断开 ({2} {3}). 放弃重连.",
                                connIdx, ctiServer, e.NewState, RunningState
                            );
                        }
                        else
                        {
                            logger.WarnFormat("从服务节点 [{0}]({1}) 连接丢失. 将发起重连", connIdx, ctiServer);
                            action = new Action(async () =>
                            {
                                logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接 ... ", connIdx, ctiServer);
                                try
                                {
                                    await conn.OpenAsync(ctiServer.Host, ctiServer.Port, WorkerNum, password, flag: 0);
                                    logger.InfoFormat("从服务节点 [{0}]({1}) 重新连接成功", connIdx, ctiServer);
                                }
                                catch (ConnectionException ex)
                                {
                                    logger.ErrorFormat("从服务节点 [{0}]({1}) 重新连接异常: {2}", connIdx, ctiServer, $"{ex.GetType()} {ex.Message}");
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

        public async Task StartUpAsync(string workerNum, string password)
        {
            using (requestGuard.TryEnter())
            {
                AgentRunningState savedRunningState;
                IEnumerable<int> minorIndices;
                var rand = new Random();
                lock (this)
                {
                    if (RunningState != AgentRunningState.Stopped)
                    {
                        throw new InvalidOperationException($"Invalid state: {RunningState}");
                    }
                    savedRunningState = RunningState;
                    RunningState = AgentRunningState.Starting;
                }
                // 首先，主节点
                try
                {
                    MainConnectionIndex = rand.Next(0, ctiServers.Count);
                    minorIndices = from i in Enumerable.Range(0, ctiServers.Count)
                                   where i != MainConnectionIndex
                                   select i;
                    WorkerNum = workerNum;
                    this.password = password;
                    logger.InfoFormat("主服务节点 [{0}]({1}|{2}) 首次连接 ... ", MainConnectionIndex, MainConnectionInfo.Host, MainConnectionInfo.Port);
                    await mainConnection.OpenAsync(
                        MainConnectionInfo.Host, MainConnectionInfo.Port,
                        workerNum, password,
                        flag: 1
                    );
                    lock (this)
                    {
                        RunningState = AgentRunningState.Started;
                    }
                }
                catch
                {
                    lock (this)
                    {
                        RunningState = savedRunningState;
                    }
                    throw;
                }
                // 然后其他节点
                var tasks = from i in minorIndices
                            select connections[i].OpenAsync(
                                ctiServers[i].Host, ctiServers[i].Port,
                                workerNum, password,
                                flag: 0
                            );
                await Task.WhenAll(tasks);
            }
        }

        public async Task ShutDownAsync(bool force = false)
        {
            using (requestGuard.TryEnter())
            {
                AgentRunningState savedRunningState;
                var graceful = !force;
                bool isMainNotConnected;
                lock (this)
                {
                    if (RunningState != AgentRunningState.Started)
                    {
                        throw new InvalidOperationException($"Invalid state: {RunningState}");
                    }
                    savedRunningState = RunningState;
                    RunningState = AgentRunningState.Stopping;
                    isMainNotConnected = !mainConnection.Connected;
                }
                try
                {
                    // 首先，主节点
                    if (isMainNotConnected)
                    {
                        logger.WarnFormat("主节点连接 {0} 已经关闭！", mainConnection, graceful);
                    }
                    else
                    {
                        logger.DebugFormat("关闭主节点连接 {0} graceful={1}...", mainConnection, graceful);
                        await mainConnection.CloseAsync(graceful, flag: 1);
                        logger.DebugFormat("关闭主节点连接 {0} graceful={1} 完毕.", mainConnection, graceful);
                    }

                    // 然后其他节点
                    var itConnObj =
                        from x in connections.Select((value, index) => (value, index))
                        where x.index != MainConnectionIndex
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
                                    await conn.CloseAsync(graceful, flag: 0);
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
                                            await conn.CloseAsync(graceful: false, flag: 0);
                                            break;
                                        default:
                                            throw;
                                    }
                                }
                            }
                            else
                            {
                                await conn.CloseAsync(graceful: false, flag: 0);
                            }
                        })
                    );
                    lock (this)
                    {
                        RunningState = AgentRunningState.Stopped;
                    }
                }
                catch
                {
                    lock (this)
                    {
                        RunningState = savedRunningState;
                    }
                    throw;
                }

                // release Sip Accounts
                lock (this)
                {
                    foreach (var sipAcc in sipAccountCollection)
                    {
                        logger.DebugFormat("Dispose {0}", sipAcc);
                        sipAcc.Dispose();
                    }
                    sipAccountCollection.Clear();
                }
            }
        }

        public bool IsRequesting => requestGuard.IsEntered;

        public async Task<ServerSentMessage> RequestAsync(int connectionIndex, AgentRequestMessage args, int timeout = Connection.DefaultRequestTimeoutMilliseconds)
        {
            using (requestGuard.TryEnter())
            {
                var conn = connections[connectionIndex];
                return await conn.RequestAsync(args, timeout);
            }
        }

        public async Task<ServerSentMessage> RequestAsync(AgentRequestMessage args, int timeout = Connection.DefaultRequestTimeoutMilliseconds)
        {
            using (requestGuard.TryEnter())
            {
                return await mainConnection.RequestAsync(args, timeout);
            }
        }

        public async Task SignInAsync()
        {
            await SignInAsync(Array.Empty<string>());
        }

        public async Task SignInAsync(string id)
        {
            await SignInAsync(new string[] { id });
        }

        public async Task SignInAsync(IEnumerable<string> ids)
        {
            using (requestGuard.TryEnter())
            {
                var s = string.Join("|", ids);
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_SIGNON, s);
                await mainConnection.RequestAsync(req);
            }
        }

        public async Task SignOutAsync()
        {
            await SignOutAsync(Array.Empty<string>());
        }

        public async Task SignOutAsync(string id)
        {
            await SignOutAsync(new string[] { id });
        }

        public async Task SignOutAsync(IEnumerable<string> ids)
        {
            using (requestGuard.TryEnter())
            {
                var s = string.Join("|", ids);
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_SIGNOFF, s);
                await mainConnection.RequestAsync(req);
            }
        }

        public async Task SetIdleAsync()
        {
            using (requestGuard.TryEnter())
            {
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_CONTINUE);
                await mainConnection.RequestAsync(req);
            }
        }

        static readonly WorkType[] availableSetBusyWorkTypes = {
            WorkType.PauseBusy,
            WorkType.PauseLeave,
            WorkType.PauseTyping,
            WorkType.PauseForce,
            WorkType.PauseDisconnect,
            WorkType.PauseSnooze,
            WorkType.PauseDinner,
            WorkType.PauseTrain,
        };

        public async Task SetBusyAsync(WorkType workType = WorkType.PauseBusy)
        {
            if (!availableSetBusyWorkTypes.Contains(workType))
                throw new ArgumentOutOfRangeException(nameof(workType), $"Invalid work type {workType}");
            using (requestGuard.TryEnter())
            {
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_PAUSE, (int)workType);
                await mainConnection.RequestAsync(req);
            }
        }

        public async Task DialAsync(string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = mainConnection;
                var s = $"{calledTelnum}|{callingTelnum}|{channelGroup}|{option}";
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_DIAL, 0, s);
                await OffHookAsync(conn);
                await conn.RequestAsync(req);
            }
        }

        public async Task XferAsync(CtiServer connectionInfo, int channel, string groupId, string workerNum = "", string customString = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var s = $"{workerNum}|{groupId}|{customString}";
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_TRANSFER, channel, s);
                await conn.RequestAsync(req);
            }
        }

        public async Task XferAsync(int connectionIndex, int channel, string groupId, string workerNum = "", string customString = "")
        {
            var connectionInfo = ctiServers[connectionIndex];
            await XferAsync(connectionInfo, channel, groupId, workerNum, customString);
        }

        public async Task Xfer(CallInfo callInfo, string groupId, string workerNum = "", string customString = "")
        {
            await XferAsync(callInfo.CtiServer, callInfo.Channel, groupId, workerNum, customString);
        }

        public async Task XferAsync(string groupId, string workerNum = "", string customString = "")
        {
            CallInfo callInfo = HeldCalls.First();
            await Xfer(callInfo, groupId, workerNum, customString);
        }

        public async Task XferConsultAsync(string groupId, string workerNum = "", string customString = "")
        {
            using (requestGuard.TryEnter())
            {
                CallInfo callInfo = HeldCalls.FirstOrDefault();
                var conn = (callInfo == null) ? mainConnection : GetConnection(callInfo.CtiServer);
                var s = $"{workerNum}|{groupId}|{customString}";
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_CONSULT, -1, s);
                await conn.RequestAsync(req);
            }
        }

        public async Task XferExtAsync(CtiServer connectionInfo, int channel, string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var s = $"{calledTelnum}|{callingTelnum}|{channelGroup}|{option}";
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_TRANSFER_EX, channel, s);
                await conn.RequestAsync(req);
            }
        }

        public async Task XferExtAsync(int connectionIndex, int channel, string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            var connectionInfo = ctiServers[connectionIndex];
            await XferExtAsync(connectionInfo, channel, calledTelnum, callingTelnum, channelGroup, option);
        }

        public async Task XferExtAsync(CallInfo callInfo, string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            var connectionInfo = callInfo.CtiServer;
            var channel = callInfo.Channel;
            await XferExtAsync(connectionInfo, channel, calledTelnum, callingTelnum, channelGroup, option);
        }

        public async Task XferExtAsync(string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            CallInfo callInfo = HeldCalls.First();
            await XferExtAsync(callInfo, calledTelnum, callingTelnum, channelGroup, option);
        }

        public async Task XferExtConsultAsync(string calledTelnum, string callingTelnum = "", string channelGroup = "", string option = "")
        {
            using (requestGuard.TryEnter())
            {
                CallInfo callInfo = HeldCalls.First();
                var conn = GetConnection(callInfo.CtiServer);
                var s = $"{calledTelnum}|{callingTelnum}|{channelGroup}|{option}";
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_CONSULT_EX, -1, s);
                await conn.RequestAsync(req);
            }
        }

        public async Task CallIvrAsync(CtiServer connectionInfo, int channel, string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var s = $"{ivrId}|{(int)invokeType}|{customString}";
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_CALLSUBFLOW, channel, s);
                await conn.RequestAsync(req);
            }
        }

        public async Task CallIvrAsync(int connectionIndex, int channel, string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            var connectionInfo = ctiServers[connectionIndex];
            await CallIvrAsync(connectionInfo, channel, ivrId, invokeType, customString);
        }

        public async Task CallIvrAsync(CallInfo callInfo, string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            var connectionInfo = callInfo.CtiServer;
            var channel = callInfo.Channel;
            await CallIvrAsync(connectionInfo, channel, ivrId, invokeType, customString);
        }

        public async Task CallIvrAsync(string ivrId, IvrInvokeType invokeType = IvrInvokeType.Keep, string customString = "")
        {
            CallInfo callInfo;
            lock (this)
            {
                callInfo = (
                    from item in calls
                    where !item.IsHeld
                    select item
                ).FirstOrDefault();
            }
            if (callInfo == null)
            {
                await CallIvrAsync(MainConnectionInfo, AgentChannel, ivrId, invokeType, customString);
            }
            else
            {
                await CallIvrAsync(callInfo, ivrId, invokeType, customString);
            }
        }

        public async Task HoldAsync()
        {
            using (requestGuard.TryEnter())
            {
                CallInfo callInfo;
                lock (this)
                {
                    callInfo = (
                        from item in calls
                        where !item.IsHeld
                        select item
                    ).First();
                }
                var conn = GetConnection(callInfo.CtiServer);
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_HOLD);
                await conn.RequestAsync(req);
            }
        }

        public async Task UnHoldAsync(CtiServer connectionInfo, int channel)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_RETRIEVE, channel);
                await conn.RequestAsync(req);
            }
        }

        public async Task UnHoldAsync(int connectionIndex, int channel)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await UnHoldAsync(connectionInfo, channel);
        }

        public async Task UnHoldAsync(CallInfo callInfo)
        {
            await UnHoldAsync(callInfo.CtiServer, callInfo.Channel);
        }

        public async Task UnHoldAsync()
        {
            var callInfo = HeldCalls.First();
            await UnHoldAsync(callInfo);
        }

        public async Task BreakAsync(int channel = -1, string customString = "")
        {
            using (requestGuard.TryEnter())
            {
                if (channel < 0)
                {
                    channel = WorkingChannelInfo.Channel;
                }
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_BREAK_SESS, channel, customString);
                await mainConnection.RequestAsync(req);
            }
        }

        static readonly SemaphoreSlim sipSemaphore = new(1);

        public async Task AnswerAsync()
        {
            logger.Debug("Answer");
            await sipSemaphore.WaitAsync();
            try
            {
                await SyncFactory.StartNew(() =>
                {
                    lock (this)
                    {
                        var call = (
                            from c in sipAccountCollection.SelectMany(x => x.Calls)
                            let ci = c.getInfo()
                            where ci.state == pjsip_inv_state.PJSIP_INV_STATE_INCOMING
                            select c
                        ).First();
                        using var cop = new CallOpParam { statusCode = pjsip_status_code.PJSIP_SC_OK };
                        call.answer(cop);
                    }
                });
            }
            finally
            {
                sipSemaphore.Release();
            }
        }

        public async Task HangupAsync()
        {
            logger.Debug("Hangup");
            await sipSemaphore.WaitAsync();
            try
            {
                await SyncFactory.StartNew(() =>
                {
                    lock (this)
                    {
                        SipEndpoint.hangupAllCalls();
                        // 主动挂机了， Call List 也来一个清空动作
                        calls.Clear();
                    }
                });
            }
            finally
            {
                sipSemaphore.Release();
            }
        }

        TaskCompletionSource<object> offHookServerTcs;
        TaskCompletionSource<object> offHookClientTcs;
        public bool IsOffHooking { get; private set; }

        public const int DefaultOffHookTimeoutMilliSeconds = 5000;

        internal async Task OffHookAsync(Connection connection, int millisecondsTimeout = DefaultOffHookTimeoutMilliSeconds)
        {
            logger.DebugFormat("OffHook - 服务节点 [{0}] 回呼请求开始", connection);
            lock (this)
            {
                if (IsOffHooking)
                    throw new InvalidOperationException();
                IsOffHooking = true;
                offHookServerTcs = new TaskCompletionSource<object>();
                offHookClientTcs = new TaskCompletionSource<object>();
            }
            try
            {
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_OFFHOOK);
                await connection.RequestAsync(req);
                var timeoutTask = Task.Delay(millisecondsTimeout);
                Task[] waitingTasks = { offHookServerTcs.Task, offHookClientTcs.Task };
                var completeTask = Task.WhenAll(offHookServerTcs.Task, offHookClientTcs.Task);
                var task = await Task.WhenAny(completeTask, timeoutTask);
                if (task == timeoutTask)
                {
                    logger.ErrorFormat("OffHook - 服务节点 [{0}] 回呼超时", connection);
                    offHookServerTcs.TrySetCanceled();
                    offHookClientTcs.TrySetCanceled();
                    throw new TimeoutException();
                }
                await task;
                if (waitingTasks.Any(x => x.Status != TaskStatus.RanToCompletion))
                {
                    logger.ErrorFormat("OffHook - 服务节点 [{0}] 回呼失败", connection);
                    throw new InvalidOperationException();
                }
                logger.DebugFormat("OffHook - 服务节点 [{0}] 回呼成功", connection);
            }
            finally
            {
                IsOffHooking = false;
            }
        }

        internal async Task OffHookAsync(CtiServer connectionInfo, int millisecondsTimeout = DefaultOffHookTimeoutMilliSeconds)
        {
            var connection = GetConnection(connectionInfo);
            await OffHookAsync(connection, millisecondsTimeout);
        }

        internal async Task OffHookAsync(int connectionIndex, int millisecondsTimeout = DefaultOffHookTimeoutMilliSeconds)
        {
            var connection = GetConnection(connectionIndex);
            await OffHookAsync(connection, millisecondsTimeout);
        }

        internal async Task OffHookAsync(int millisecondsTimeout = DefaultOffHookTimeoutMilliSeconds)
        {
            var connectionInfo = MainConnectionInfo;
            await OffHookAsync(connectionInfo, millisecondsTimeout);
        }

        public async Task DequeueAsync(QueueInfo queueInfo)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(queueInfo.CtiServer);
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_GETQUEUE, queueInfo.Channel);
                await conn.RequestAsync(req);
            }
        }

        public async Task MonitorAsync(CtiServer connectionInfo, string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_LISTEN, -1, workerNum);
                await OffHookAsync(conn);
                await conn.RequestAsync(req);
            }
        }


        public async Task MonitorAsync(int connectionIndex, string workerNum)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await MonitorAsync(connectionInfo, workerNum);
        }

        public async Task InterceptAsync(CtiServer connectionInfo, string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_INTERCEPT, -1, workerNum);
                await conn.RequestAsync(req);
            }
        }

        public async Task InterceptAsync(int connectionIndex, string workerNum)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await InterceptAsync(connectionInfo, workerNum);
        }

        public async Task UnMonitorAsync(CtiServer connectionInfo, string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_STOPLISTEN, workerNum);
                await conn.RequestAsync(req);
            }
        }

        public async Task UnMonitorAsync(int connectionIndex, string workerNum)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await UnMonitorAsync(connectionInfo, workerNum);
        }

        public async Task InterruptAsync(CtiServer connectionInfo, string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_FORCEINSERT, -1, workerNum);
                await OffHookAsync(conn);
                await conn.RequestAsync(req);
            }
        }

        public async Task InterruptAsync(int connectionIndex, string workerNum)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await InterruptAsync(connectionInfo, workerNum);
        }

        public async Task HangupAsync(CtiServer connectionInfo, string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var conn = GetConnection(connectionInfo);
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_FORCEHANGUP, workerNum);
                await conn.RequestAsync(req);
            }
        }

        public async Task HangupAsync(int connectionIndex, string workerNum)
        {
            var connectionInfo = ctiServers[connectionIndex];
            await HangupAsync(connectionInfo, workerNum);
        }

        public async Task SignOutAsync(string workerNum, string groupId)
        {
            using (requestGuard.TryEnter())
            {
                var s = $"{workerNum}|{groupId}";
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_FORCESIGNOFF, -1, s);
                await mainConnection.RequestAsync(req);
            }
        }

        public async Task SetIdleAsync(string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_FORCEIDLE, -1, workerNum);
                await mainConnection.RequestAsync(req);
            }
        }

        public async Task SetBusyAsync(string workerNum, WorkType workType = WorkType.PauseBusy)
        {
            using (requestGuard.TryEnter())
            {
                var s = $"{workerNum}|{(int)workType}";
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_FORCEPAUSE, -1, s);
                await mainConnection.RequestAsync(req);
            }
        }

        public async Task KickOutAsync(string workerNum)
        {
            using (requestGuard.TryEnter())
            {
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_KICKOUT, -1, workerNum);
                await mainConnection.RequestAsync(req);
            }
        }

        public async Task TakenAwayAsync(int index)
        {
            using (requestGuard.TryEnter())
            {
                Connection connObj;
                // 先修改 index
                lock (this)
                {
                    if (MainConnectionIndex < 0)
                    {
                        throw new InvalidOperationException();
                    }
                    if (index < 0 || index >= connections.Count || index == MainConnectionIndex)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, "");
                    }
                    if (connections[index].State != ConnectionState.Ok)
                    {
                        throw new InvalidOperationException($"Invalid state: {connections[index].State}");
                    }
                    connObj = mainConnection;
                    MainConnectionIndex = index;
                    logger.InfoFormat("切换主服务节点到 {0}", mainConnection);
                }
                OnMainConnectionChanged?.Invoke(this, EventArgs.Empty);
                // 再通知原来的主，无论能否通知成功
                var req = new AgentRequestMessage(MessageType.REMOTE_MSG_TAKENAWAY);
                await connObj.RequestAsync(req);
            }
        }

        readonly HashSet<Sip.Account> sipAccountCollection = new();
        readonly HashSet<SipAccount> sipAccounts = new();
        public IReadOnlyCollection<SipAccount> SipAccounts => sipAccounts;

    }
}
