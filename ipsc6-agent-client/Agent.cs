using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ipsc6.agent.client
{
    public class Agent : IDisposable
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Agent));
        // Flag: Has Dispose already been called?
        private bool disposed = false;
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

        ~Agent()
        {
            Dispose(false);
        }

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
                        logger.DebugFormat("{0} Dispose {1}", this, conn);
                        conn.Dispose();
                    }
                    // Note disposing has been done.
                    disposed = true;
                }
            }
        }

        string workerNumber;
        public string WorkerNumber => workerNumber;

        readonly List<ConnectionInfo> connectionList = new List<ConnectionInfo>();
        public IReadOnlyCollection<ConnectionInfo> ConnectionList => connectionList;

        readonly List<Connection> internalConnections = new List<Connection>();

        int mainConnectionIndex = -1;
        public int MainConnectionIndex => mainConnectionIndex;

        public event EventHandler OnMainConnectionChanged;

        public ConnectionInfo MainConnectionInfo => (mainConnectionIndex < 0) ? null : connectionList[mainConnectionIndex];

        Connection MainConnection
        {
            get { return internalConnections[mainConnectionIndex]; }
        }
        public int AgentId => MainConnection.AgentId;

        string displayName;
        public string DisplayName => displayName;

        int channel = -1;
        public int Channel => channel;

        AgentState agentState = AgentState.NotExist;
        WorkType workType = WorkType.Unknown;
        public AgentState AgentState
        {
            get { return agentState; }
        }
        public WorkType WorkType
        {
            get { return workType; }
        }
        void SetAgentStateWorkType(AgentStateWorkType value)
        {
            agentState = value.AgentState;
            workType = value.WorkType;
        }
        public AgentStateWorkType AgentStateWorkType
        {
            get { return new AgentStateWorkType(agentState, workType); }
        }
        public event AgentStateChangedEventHandler OnAgentStateChanged;


        TeleState teleState = TeleState.HangUp;
        public TeleState TeleState
        {
            get { return teleState; }
        }
        public event TeleStateChangedEventHandler OnTeleStateChanged;


        HashSet<QueueInfo> queueInfoCollection = new HashSet<QueueInfo>();
        public IReadOnlyCollection<QueueInfo> QueueInfoCollection
        {
            get { return queueInfoCollection; }
        }
        public event QueueInfoEventHandler OnQueueInfo;

        HashSet<HoldInfo> holdInfoCollection = new HashSet<HoldInfo>();
        public IReadOnlyCollection<HoldInfo> HoldInfoCollection
        {
            get { return holdInfoCollection; }
        }
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
            AgentStateWorkType oldState;
            AgentStateChangedEventArgs<AgentStateWorkType> ev = null;
            var newState = new AgentStateWorkType((AgentState)msg.N1, (WorkType)msg.N2);
            lock (this)
            {
                var index = connectionList.FindIndex(x => x == connInfo);
                oldState = AgentStateWorkType.Clone() as AgentStateWorkType;
                var connObj = internalConnections[index];
                if (
                    (newState.AgentState == AgentState.Ring) ||
                    (newState.AgentState == AgentState.Work)
                )
                {
                    TakeOverMaster(index);
                }
                if (oldState != newState)
                {
                    SetAgentStateWorkType(newState);
                    ev = new AgentStateChangedEventArgs<AgentStateWorkType>(oldState, newState);
                }
            }
            if (ev != null)
            {
                OnAgentStateChanged?.Invoke(this, ev);
            }
        }

        private void TakeOverMaster(int index)
        {
            mainConnectionIndex = index;
            OnMainConnectionChanged?.Invoke(this, new EventArgs());
        }

        void ProcessTeleStateChangedMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            TeleState oldState;
            TeleState newState = (TeleState)msg.N1;
            TeleStateChangedEventArgs<TeleState> ev = null;
            lock (this)
            {
                oldState = teleState;
                if (oldState != newState)
                {
                    teleState = newState;
                    ev = new TeleStateChangedEventArgs<TeleState>(oldState, newState);
                }
            }
            if (ev != null)
            {
                OnTeleStateChanged?.Invoke(this, ev);
            }
        }

        void ProcessQueueInfoMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var info = new QueueInfo(connInfo, msg);
            var ev = new QueueInfoEventArgs(connInfo, info);
            // 记录 QueueInfo
            lock (this)
            {
                // 修改之前,一律先删除
                queueInfoCollection.RemoveWhere(m => m == info);
                if (info.EventType != QueueEventType.Cancel)
                {
                    queueInfoCollection.Add(info);
                }
            }
            /// 
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
        private void DoOnSignedGroupIdList(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var signed = Convert.ToBoolean(msg.N2);
            var ids = msg.S.Split(new char[] { '|' });
            lock (this)
            {
                foreach (var id in ids)
                {
                    try
                    {
                        groupCollection.First(m => m.Id == id)
                            .Signed = signed;
                    }
                    catch (InvalidOperationException exc)
                    {
                        logger.ErrorFormat("DoOnSignedGroupIdList - {0} (id=\"{1})\"", exc, id);
                    }
                }
            }
            OnSignedGroupsChanged?.Invoke(this, new EventArgs());
        }

        public event SipRegistrarListReceivedEventHandler OnSipRegistrarListReceived;
        private void DoOnSipRegistrarList(ConnectionInfo _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var val = msg.S.Split(new char[] { '|' });
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
        public RingInfo RingInfo
        {
            get { return ringInfo; }
        }
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
        public WorkingChannelInfo WorkingChannel
        {
            get { return workingChannelInfo; }
        }
        public event WorkingChannelInfoReceivedEventHandler OnWorkingChannelInfoReceived;
        private void DoOnWorkingChannel(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var info = new WorkingChannelInfo(msg.N2, msg.S);
            var evt = new WorkingChannelInfoReceivedEventArgs(connInfo, info);
            lock (this)
            {
                workingChannelInfo = info;
            }
            OnWorkingChannelInfoReceived?.Invoke(this, evt);
        }

        private HashSet<Privilege> privilegeCollection = new HashSet<Privilege>();
        public IReadOnlyCollection<Privilege> PrivilegeCollection
        {
            get { return privilegeCollection; }
        }
        public event EventHandler OnPrivilegeCollectionReceived;
        private void DoOnPrivilegeList(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var parts = msg.S.Split(new char[] { '|' });
            lock (this)
            {
                foreach (var s in parts)
                {
                    privilegeCollection.Add((Privilege)Convert.ToInt32(s));
                }
            }
            OnPrivilegeCollectionReceived?.Invoke(this, new EventArgs());
        }

        private HashSet<int> privilegeExternCollection = new HashSet<int>();
        public IReadOnlyCollection<int> PrivilegeExternCollection
        {
            get { return privilegeExternCollection; }
        }
        public event EventHandler OnPrivilegeExternCollectionReceived;
        private void DoOnPrivilegeExternList(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var parts = msg.S.Split(new char[] { '|' });
            lock (this)
            {
                foreach (var s in parts)
                {
                    privilegeExternCollection.Add(Convert.ToInt32(s));
                }
            }
            OnPrivilegeExternCollectionReceived?.Invoke(this, new EventArgs());
        }

        HashSet<AgentGroup> groupCollection = new HashSet<AgentGroup>();
        public IReadOnlyCollection<AgentGroup> GroupCollection
        {
            get { return groupCollection; }
        }
        public event EventHandler OnGroupCollectionReceived;
        void DoOnGroupIdList(ConnectionInfo _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var ids = msg.S.Split(new char[] { '|' });
            lock (this)
            {
                foreach (var id in ids)
                {
                    if (!groupCollection.Any(m => m.Id == id))
                    {
                        groupCollection.Add(new AgentGroup(id));
                    }
                }
            }
        }

        void DoOnGroupNameList(ConnectionInfo _, ServerSentMessage msg)
        {
            var names = msg.S.Split(new char[] { '|' });
            lock (this)
            {
                var it = groupCollection.Zip(names, (first, second) => new { first, second });
                foreach (var m in it)
                {
                    m.first.Name = m.second;
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
        void Conn_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs<ConnectionState> e)
        {
            var conn = sender as Connection;
            var i = internalConnections.IndexOf(conn);
            var connInfo = connectionList[i];
            var evt = new ConnectionInfoStateChangedEventArgs<ConnectionState>(connInfo, e.OldState, e.NewState);
            // TODO: 断线处理???
            OnConnectionStateChanged?.Invoke(this, evt);
        }

        bool starting = false;
        bool started = false;

        public async Task StartUp(string workerNumber, string password)
        {
            lock (this)
            {
                if (starting)
                    throw new InvalidOperationException("starting");
                if (started)
                    throw new InvalidOperationException("started");
                starting = true;
                started = false;
            }
            this.workerNumber = workerNumber;
            // 首先，主节点
            var rand = new Random();
            mainConnectionIndex = rand.Next(0, connectionList.Count);
            bool isMainConnectOk = false;
            try
            {
                await MainConnection.Open(MainConnectionInfo.Host, MainConnectionInfo.Port, workerNumber, password, flag: 1);
                isMainConnectOk = true;
            }
            finally
            {
                lock (this)
                {
                    starting = false;
                    started = isMainConnectOk;
                }
            }
            // 然后其他节点
            var itConnInfo =
                from x in connectionList.Select((value, index) => new { value, index })
                where x.index != mainConnectionIndex
                select x.value;
            var itConnObj =
                from x in internalConnections.Select((value, index) => new { value, index })
                where x.index != mainConnectionIndex
                select x.value;
            var itZipped = itConnInfo.Zip(
                itConnObj,
                (info, conn) => new { info, conn }
            );
            var tasks =
                from x in itZipped
                select x.conn.Open(x.info.Host, x.info.Port, workerNumber, password, flag: 0);
            await Task.WhenAll(tasks);
        }

        public async Task ShutDown(bool force = false)
        {
            var graceful = !force;
            // 首先，主节点
            logger.DebugFormat("{0} close(master) {1} ...", this, MainConnection);
            await MainConnection.Close(graceful, flag: 1);
            logger.DebugFormat("{0} close(master) {1} Ok.", this, MainConnection);
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
                                (ex is DisconnectionTimeoutException) ||
                                (ex is ErrorResponse) ||
                                (ex is InvalidOperationException)
                            )
                            {
                                await conn.Close(graceful: false, flag: 0);
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
        }

        public async Task<ServerSentMessage> Request(AgentRequestMessage args, int millisecondsTimeout = Connection.DefaultTimeoutMilliseconds)
        {
            return await MainConnection.Request(args, millisecondsTimeout);
        }

        public async Task SignIn()
        {
            await SignIn(new string[] { });
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
            await SignOut(new string[] { });
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
                throw new ArgumentOutOfRangeException(string.Format("{0}", workType));
            }
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_PAUSE, (int)workType);
            await MainConnection.Request(req);
        }

        public async Task Intercept(ConnectionInfo connInfo, int agentId)
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

        public async Task UnHold(int channel = -1)
        {
            if (channel < 0)
            {
                if (workingChannelInfo.Channel < 0)
                {
                    throw new InvalidOperationException();
                }
                channel = workingChannelInfo.Channel;
            }
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_RETRIEVE, channel);
            await MainConnection.Request(req);
        }

        public async Task Break(int channel = -1, string customString = "")
        {
            if (channel < 0)
            {
                if (workingChannelInfo.Channel < 0)
                {
                    throw new InvalidOperationException();
                }
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

        public async Task Dequeue(int channel)
        {
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_GETQUEUE, channel);
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

        public async Task CallIvr(int channel, string ivrName, IvrInvokeType invokeType, string customString)
        {
            var s = string.Format("{0}|{1}|{2}", ivrName, (int)invokeType, customString);
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_CALLSUBFLOW, channel, s);
            await MainConnection.Request(req);
        }

    }
}
