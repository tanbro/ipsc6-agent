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
            var rand = new Random();
            mainConnectionIndex = rand.Next(0, connectionList.Count);
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
                if (this.disposed)
                {
                    logger.Error("Dispose has already been called.");
                }
                else
                {
                    foreach (var conn in internalConnections)
                    {
                        conn.Dispose();
                    }
                    // Note disposing has been done.
                    disposed = true;
                }
            }
        }

        readonly object lck = new object();

        public string WorkerNumber;

        readonly List<ConnectionInfo> connectionList = new List<ConnectionInfo>();
        public IReadOnlyCollection<ConnectionInfo> ConnectionList
        {
            get { return connectionList; }
        }
        readonly List<Connection> internalConnections = new List<Connection>();

        int mainConnectionIndex;
        public int MainConnectionIndex
        {
            get { return mainConnectionIndex; }
        }
        public event EventHandler OnMainConnectionChanged;

        public ConnectionInfo MainConnectionInfo
        {
            get { return connectionList[mainConnectionIndex]; }
        }

        Connection MainConnection
        {
            get { return internalConnections[mainConnectionIndex]; }
        }
        public int AgentId
        {
            get { return MainConnection.AgentId; }
        }

        string displayName;
        public string DisplayName
        {
            get { return displayName; }
        }

        int channel = -1;
        public int Channel
        {
            get { return channel; }
        }

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
            lock (lck)
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
            lock (lck)
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
            lock (lck)
            {
                // 修改之前,一律先删除
                queueInfoCollection.RemoveWhere((m) => m == info);
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
            var info = new HoldInfo(connInfo, msg);
            var ev = new HoldInfoEventArgs(connInfo, info);
            // 记录 HoldInfo
            lock (lck)
            {
                // 修改之前,一律先删除
                holdInfoCollection.RemoveWhere((m) => m == info);
                if (info.EventType != HoldEventType.Cancel)
                {
                    holdInfoCollection.Add(info);
                }
            }
            //
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
            lock (lck)
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
            var evt = new RingInfoReceivedEventArgs(connInfo, _ringInfo);
            lock (lck)
            {
                workingChannelInfo = _workChInfo;
                ringInfo = _ringInfo;
            }
            OnRingInfoReceived?.Invoke(this, evt);
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
            lock (lck)
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
            lock (lck)
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
            lock (lck)
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
            lock (lck)
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
            lock (lck)
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
            lock (lck)
            {
                channel = evt.Value;
            }
            OnChannelAssigned?.Invoke(this, evt);
        }

        public event AgentDisplayNameReceivedEventHandler OnAgentDisplayNameReceived;
        void DoOnAgentId(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var evt = new AgentDisplayNameReceivedEventArgs(connInfo, msg.S);
            lock (lck)
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
            var e_ = new ConnectionInfoStateChangedEventArgs<ConnectionState>(connInfo, e.OldState, e.NewState);
            // TODO: 断线处理???
            OnConnectionStateChanged?.Invoke(this, e_);
        }

        public async Task StartUp(string workerNumber, string password)
        {
            // 首先，主节点
            await MainConnection.Open(MainConnectionInfo.Host, MainConnectionInfo.Port, workerNumber, password, flag: 1);
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
            await MainConnection.Close(graceful, flag: 1);
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
                            await conn.Close(graceful, flag: 0);
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
            throw new NotImplementedException();
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_INTERCEPT, agentId);
            await MainConnection.Request(req);
        }

        public void Dial()
        {
            throw new NotImplementedException();
        }

        public async Task Xfer(int agentId, int channel, string groupId, string customString)
        {
            var s = string.Format("{0}|{1}|{2}", channel, groupId, customString);
            var req = new AgentRequestMessage(MessageType.REMOTE_MSG_TRANSFER, agentId, s);
            await MainConnection.Request(req);
        }

        //public async Task XferExt()
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task Consult(int agentId, int channel, string groupId, string customString)
    }
}
