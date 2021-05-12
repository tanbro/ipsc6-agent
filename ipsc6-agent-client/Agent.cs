using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ipsc6.agent.client
{
    public class Agent : IDisposable
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Agent));

        public Agent(IEnumerable<ConnectionInfo> connections)
        {
            foreach (var m in connections)
            {
                var conn = new Connection();
                conn.OnConnectionStateChanged += Conn_OnConnectionStateChanged;
                conn.OnServerSentEvent += Conn_OnServerSend;
                internalConnections.Add(conn);
                connectionList.Add(m);
            }
        }

        public Agent(IEnumerable<string> addresses)
        {
            foreach (var s in addresses)
            {
                var connInfo = new ConnectionInfo(s);
                var conn = new Connection();
                conn.OnConnectionStateChanged += Conn_OnConnectionStateChanged;
                conn.OnServerSentEvent += Conn_OnServerSend;
                internalConnections.Add(conn);
                connectionList.Add(connInfo);
            }
        }

        ~Agent()
        {
            Dispose(false);
        }

        // Flag: Has Dispose already been called?
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Check to see if Dispose has already been called.
                if (disposed)
                {
                    logger.Error("Dispose has already been called.");
                }
                else
                {
                    foreach (var conn in internalConnections)
                    {
                        logger.DebugFormat("Dispose {0}", conn);
                        conn.Dispose();
                    }
                    // Note disposing has been done.
                    disposed = true;
                }
            }
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

        int channel = -1;
        public int Channel => channel;

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

        TeleState teleState = TeleState.HangUp;
        public TeleState TeleState => teleState;
        public event TeleStateChangedEventHandler OnTeleStateChanged;

        readonly HashSet<QueueInfo> queueInfoCollection = new HashSet<QueueInfo>();
        public IReadOnlyCollection<QueueInfo> QueueInfoCollection => queueInfoCollection;
        public event QueueInfoEventHandler OnQueueInfo;

        readonly HashSet<HoldInfo> holdInfoCollection = new HashSet<HoldInfo>();
        public IReadOnlyCollection<HoldInfo> HoldInfoCollection => holdInfoCollection;
        public event HoldInfoEventHandler OnHoldInfo;

        void Conn_OnServerSend(object sender, ServerSentEventArgs e)
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

        void ProcessStateChangedMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            AgentState[] workingState = { AgentState.Ring, AgentState.Work };
            var currIndex = connectionList.FindIndex(ci => ci == connInfo);
            var newState = new AgentStateWorkType((AgentState)msg.N1, (WorkType)msg.N2);
            AgentStateWorkType oldState = null;
            Connection oldMainConnObj = null;
            lock (this)
            {
                if (AgentStateWorkType != newState)
                {
                    oldState = AgentStateWorkType.Clone() as AgentStateWorkType;
                    AgentStateWorkType = newState;
                }
                // 如果是工作状态且主连接不是当前的，需要切换！
                if (workingState.Any(m => m == newState.AgentState) && currIndex != mainConnectionIndex)
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
            TeleStateChangedEventArgs ev = null;
            lock (this)
            {
                oldState = teleState;
                if (oldState != newState)
                {
                    teleState = newState;
                    ev = new TeleStateChangedEventArgs(oldState, newState);
                }
            }
            if (ev != null)
            {
                OnTeleStateChanged?.Invoke(this, ev);
            }
        }

        void ProcessQueueInfoMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var queueInfo = new QueueInfo(connInfo, msg, groupCollection);
            // 记录 QueueInfo
            lock (this)
            {
                // 修改之前,一律先删除
                queueInfoCollection.RemoveWhere(m => m == queueInfo);
                if (queueInfo.EventType != QueueEventType.Cancel)
                {
                    queueInfoCollection.Add(queueInfo);
                }
            }
            var ev = new QueueInfoEventArgs(connInfo, queueInfo);
            OnQueueInfo?.Invoke(this, ev);
        }

        void ProcessHoldInfoMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var holdInfo = new HoldInfo(connInfo, msg);
            // 记录 HoldInfo
            lock (this)
            {
                // 修改之前,一律先删除
                holdInfoCollection.RemoveWhere(m => m == holdInfo);
                if (holdInfo.EventType != HoldEventType.Cancel)
                {
                    holdInfoCollection.Add(holdInfo);
                }
            }
            var ev = new HoldInfoEventArgs(connInfo, holdInfo);
            OnHoldInfo?.Invoke(this, ev);
        }

        void ProcessDataMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var type_ = (ServerSentMessageSubType)(msg.N1);
            switch (type_)
            {
                case ServerSentMessageSubType.AgentId:
                    DoOnAgentId(connInfo, msg);
                    break;
                case ServerSentMessageSubType.SipRegistrarList:
                    DoOnSipRegistrarList(connInfo, msg);
                    break;
                case ServerSentMessageSubType.Channel:
                    DoOnChannel(connInfo, msg);
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
                        logger.ErrorFormat("DoOnSignedGroupIdList - 技能组 <id=\"{0})\"> 不存在", id);
                    }
                    else
                    {
                        groupObj.Signed = signed;
                    }
                }
            }
            OnSignedGroupsChanged?.Invoke(this, new EventArgs());
        }

        public event SipRegistrarListReceivedEventHandler OnSipRegistrarListReceived;
        private void DoOnSipRegistrarList(ConnectionInfo _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var val = msg.S.Split(Constants.VerticalBarDelimiter);
            var evt = new SipRegistrarListReceivedEventArgs(val);
            OnSipRegistrarListReceived?.Invoke(this, evt);
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

        RingInfo ringInfo;
        public RingInfo RingInfo => ringInfo;
        public event RingInfoReceivedEventHandler OnRingInfoReceived;
        private void DoOnRing(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var _ringInfo = new RingInfo(connInfo, msg.N2, msg.S);
            var _workChInfo = new WorkingChannelInfo(_ringInfo.WorkingChannel);
            var e1 = new WorkingChannelInfoReceivedEventArgs(connInfo, _workChInfo);
            var e2 = new RingInfoReceivedEventArgs(connInfo, _ringInfo);
            lock (this)
            {
                workingChannelInfo = _workChInfo;
                ringInfo = _ringInfo;
                OnWorkingChannelInfoReceived?.Invoke(this, e1);
                OnRingInfoReceived?.Invoke(this, e2);
            }
        }

        WorkingChannelInfo workingChannelInfo;
        public WorkingChannelInfo WorkingChannel => workingChannelInfo;
        public event WorkingChannelInfoReceivedEventHandler OnWorkingChannelInfoReceived;
        private void DoOnWorkingChannel(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var info = new WorkingChannelInfo(msg.N2, msg.S);
            lock (this)
            {
                workingChannelInfo = info;
            }
            var evt = new WorkingChannelInfoReceivedEventArgs(connInfo, info);
            OnWorkingChannelInfoReceived?.Invoke(this, evt);
        }

        private readonly HashSet<Privilege> privilegeCollection = new HashSet<Privilege>();
        public IReadOnlyCollection<Privilege> PrivilegeCollection => privilegeCollection;
        public event EventHandler OnPrivilegeCollectionReceived;
        private void DoOnPrivilegeList(ConnectionInfo _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var values = from s in msg.S.Split(Constants.VerticalBarDelimiter)
                         select (Privilege)Convert.ToInt32(s);
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
                         select Convert.ToInt32(s);
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
                foreach (var pair in groupCollection.Zip(names, (first, second) => new { first, second }))
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
                channel = evt.Value;
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
            var evtStateChanged = new ConnectionInfoStateChangedEventArgs(connInfo, e.OldState, e.NewState);
            EventArgs evtMainConnChanged = null;
            Action action = null;
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
                                "主服务节点 [{0}]({1}|{2}) 连接断开. {3} {4}. 放弃重连.",
                                connIdx, connectionList[connIdx].Host, connectionList[connIdx].Port,
                                e.NewState, runningState
                            );
                        }
                        else
                        {
                            bool isChangeMain = false;
                            // 其它连接还有连接上了的吗?
                            var indices = (
                                from vi in internalConnections.Select((value, index) => new { value, index })
                                where vi.index != mainConnectionIndex && vi.value.State == ConnectionState.Ok
                                select vi.index
                            ).ToArray();
                            if (indices.Length == 0)
                            {
                                // 没得选
                                switch (e.NewState)
                                {
                                    case ConnectionState.Closed:
                                        logger.WarnFormat("主服务节点 [{0}]({1}|{2}) 连接被对端关闭. 虽然找不到其它可用服务节点连接, 仍将切换主节点", connIdx, connectionList[connIdx].Host, connectionList[connIdx].Port);
                                        isChangeMain = true;
                                        break;
                                    case ConnectionState.Lost:
                                        logger.WarnFormat("主服务节点 [{0}]({1}|{2}) 连接丢失. 但是由于找不到其它可用服务节点连接, 将继续使用该主节点并发起重连", connIdx, connectionList[connIdx].Host, connectionList[connIdx].Port);
                                        break;
                                    case ConnectionState.Failed:
                                        logger.WarnFormat("主服务节点 [{0}]({1}|{2}) 连接失败. 但是由于找不到其它可用服务节点连接, 将继续使用该主节点并发起重连", connIdx, connectionList[connIdx].Host, connectionList[connIdx].Port);
                                        break;
                                    default: throw new IndexOutOfRangeException(string.Format("{0}", e.NewState));
                                }
                            }
                            else
                            {
                                // 有的选,但是如果在工作状态,不能变!
                                if (workingState.Any(x => x == AgentState))
                                {
                                    if (e.NewState == ConnectionState.Lost)
                                        logger.WarnFormat("主服务节点 [{0}]({1}|{2}) 连接丢失. 由于处于工作状态, 将继续使用该主节点并发起重连", connIdx, connectionList[connIdx].Host, connectionList[connIdx].Port);
                                    else
                                        logger.WarnFormat("主服务节点 [{0}]({1}|{2}) 连接失败. 由于处于工作状态, 将继续使用该主节点并发起重连", connIdx, connectionList[connIdx].Host, connectionList[connIdx].Port);
                                }
                                else
                                {
                                    isChangeMain = true;
                                }
                            }
                            ///
                            if (isChangeMain)
                            {
                                var rand = new Random();
                                if (indices.Length > 0)
                                {
                                    mainConnectionIndex = indices[rand.Next(indices.Length)];
                                    logger.WarnFormat(
                                        "主服务节点 [{0}]({1}|{2}) 连接丢失. 切换主服务节点到 [{3}]({4}|{5})",
                                        connIdx, connInfo.Host, connInfo.Port,
                                        mainConnectionIndex, MainConnectionInfo.Host, MainConnectionInfo.Port
                                    );
                                }
                                else
                                {
                                    var indices2 = (
                                        from vi
                                        in internalConnections.Select((value, index) => new { value, index })
                                        where vi.index != mainConnectionIndex && vi.value.State != ConnectionState.Closed
                                        select vi.index
                                    ).ToList();
                                    mainConnectionIndex = indices2[rand.Next(indices2.Count)];
                                    logger.WarnFormat(
                                        "主服务节点 [{0}]({1}|{2}) 连接丢失. 切换主服务节点(目前不可用)到 [{3}]({4}|{5})",
                                        connIdx, connInfo.Host, connInfo.Port,
                                        mainConnectionIndex, MainConnectionInfo.Host, MainConnectionInfo.Port
                                    );
                                }
                                action = new Action(async () =>
                                {
                                    logger.InfoFormat("从服务节点 [{0}]({1}|{2}) 重新连接 ... ", connIdx, connInfo.Host, connInfo.Port);
                                    try
                                    {
                                        await conn.Open(connInfo.Host, connInfo.Port, workerNumber, password, flag: 0);
                                        logger.InfoFormat("从服务节点 [{0}]({1}|{2}) 重新连接成功", connIdx, connInfo.Host, connInfo.Port);
                                    }
                                    catch (ConnectionException) { };
                                });
                                // 需要抛出切换新的主服务节事件
                                evtMainConnChanged = new EventArgs();
                            }
                            else
                            {
                                action = new Action(async () =>
                                {
                                    logger.InfoFormat("主服务节点 [{0}]({1}|{2}) 重新连接 ... ", connIdx, connInfo.Host, connInfo.Port);
                                    try
                                    {
                                        await conn.Open(connInfo.Host, connInfo.Port, workerNumber, password, flag: 1);
                                        logger.InfoFormat("主服务节点 [{0}]({1}|{2}) 重新连接成功", connIdx, connInfo.Host, connInfo.Port);
                                    }
                                    catch (ConnectionException) { };
                                });
                            }
                        }
                    }
                    else
                    {
                        if (runningState != AgentRunningState.Started)
                        {
                            logger.WarnFormat(
                                "从服务节点 [{0}]({1}|{2}) 连接断开. {3} {4}. 放弃重连.",
                                connIdx, connInfo.Host, connInfo.Port,
                                e.NewState, runningState
                            );
                        }
                        else
                        {
                            logger.WarnFormat("从服务节点 [{0}]({1}|{2}) 连接丢失. 将发起重连", connIdx, connInfo.Host, connInfo.Port);
                            action = new Action(async () =>
                            {
                                logger.InfoFormat("从服务节点 [{0}]({1}|{2}) 重新连接 ... ", connIdx, connInfo.Host, connInfo.Port);
                                try
                                {
                                    await conn.Open(connInfo.Host, connInfo.Port, workerNumber, password, flag: 0);
                                    logger.InfoFormat("从服务节点 [{0}]({1}|{2}) 成功", connIdx, connInfo.Host, connInfo.Port);
                                }
                                catch (ConnectionException) { };
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
            lock (this)
            {
                if (runningState != AgentRunningState.Stopped)
                    throw new InvalidOperationException(string.Format("{0}", runningState));
                savedRunningState = runningState;
                runningState = AgentRunningState.Starting;
            }
            // 首先，主节点
            try
            {
                var rand = new Random();
                mainConnectionIndex = rand.Next(0, connectionList.Count);
                minorIndices = from i in Enumerable.Range(0, connectionList.Count)
                               where i != mainConnectionIndex
                               select i;
                this.workerNumber = workerNumber;
                this.password = password;
                logger.InfoFormat("主服务节点 [{0}]({1}|{2}) 首次连接 ... ", mainConnectionIndex, MainConnectionInfo.Host, MainConnectionInfo.Port);
                await MainConnection.Open(MainConnectionInfo.Host, MainConnectionInfo.Port, workerNumber, password, flag: 1);
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
                    throw new InvalidOperationException(string.Format("{0}", runningState));
                savedRunningState = runningState;
                runningState = AgentRunningState.Stopping;
                isMainNotConnected = !MainConnection.Connected;
            }
            try
            {
                // 首先，主节点
                if (isMainNotConnected)
                {
                    logger.WarnFormat("{0} 主节点连接 {1} 已经关闭！", this, MainConnection, graceful);
                }
                else
                {
                    logger.DebugFormat("{0} close(master) {1} graceful={2}...", this, MainConnection, graceful);
                    await MainConnection.Close(graceful, flag: 1);
                    logger.DebugFormat("{0} close(master) {1} Ok.", this, MainConnection);
                }

                // 然后其他节点
                var itConnObj =
                    from x in internalConnections.Select((value, index) => new { value, index })
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
                                logger.DebugFormat("{0} close(slaver) {1} ...", this, conn);
                                await conn.Close(graceful, flag: 0);
                                logger.DebugFormat("{0} close(slaver) {1} Ok.", this, conn);
                            }
                            catch (Exception ex)
                            {
                                if (
                                    ex is DisconnectionTimeoutException ||
                                    ex is ErrorResponse ||
                                    ex is InvalidOperationException
                                )
                                {
                                    logger.DebugFormat("{0} force close(slaver) {1} ...", this, conn);
                                    await conn.Close(graceful: false, flag: 0);
                                    logger.DebugFormat("{0} force close(slaver) {1} ...", this, conn);
                                }
                                else
                                {
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
                throw new ArgumentOutOfRangeException(nameof(workType), string.Format("Invalid work type {0}", workType));
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

        public void Dial()
        {
            throw new NotImplementedException();
        }

        public async Task Xfer(int agentId, int channel, string groupId, string customString = "", bool consultative = false)
        {
            var s = string.Format("{0}|{1}|{2}", channel, groupId, customString);
            var mt = consultative ? MessageType.REMOTE_MSG_TRANSFER_EX : MessageType.REMOTE_MSG_TRANSFER;
            var req = new AgentRequestMessage(mt, agentId, s);
            await MainConnection.Request(req);
        }

        public void XferExt()
        {
            throw new NotImplementedException();
        }

        public async Task Hold()
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_HOLD);
            await MainConnection.Request(req);
        }

        public async Task UnHold(HoldInfo holdInfo)
        {
            var connObj = GetConnection(holdInfo.ConnectionInfo);
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_RETRIEVE, holdInfo.Channel);
            await connObj.Request(req);
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

        public async Task HangUp()
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_HANGUP);
            await MainConnection.Request(req);
        }

        public async Task HangUp(int agentId)
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_FORCEHANGUP, agentId);
            await MainConnection.Request(req);
        }

        public async Task OffHook()
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_OFFHOOK);
            await MainConnection.Request(req);
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

        public async Task Dequeue(QueueInfo queueInfo)
        {
            var connObj = GetConnection(queueInfo.ConnectionInfo);
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_GETQUEUE, queueInfo.Channel);
            await connObj.Request(req);
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

        public async Task CallIvr(int channel, string ivrName, IvrInvokeType invokeType, string customString)
        {
            var s = string.Format("{0}|{1}|{2}", ivrName, (int)invokeType, customString);
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_CALLSUBFLOW, channel, s);
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
                    throw new InvalidOperationException(string.Format("{0}", internalConnections[index].State));
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

    }
}
