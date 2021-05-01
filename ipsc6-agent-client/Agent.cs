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
            foreach (var conn in internalConnections.OrderBy(_ => rand.Next()))
            {
                shuffledConnections.Add(conn);
            }
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
        readonly List<Connection> shuffledConnections = new List<Connection>();
        int owershipIndex = 0;
        public int MainConnectionIndex
        {
            get
            {
                var conn = shuffledConnections[owershipIndex];
                return internalConnections.IndexOf(conn);
            }
        }
        public ConnectionInfo MainConnectionInfo
        {
            get { return connectionList[MainConnectionIndex]; }
        }

        Connection MainConnection
        {
            get { return internalConnections[MainConnectionIndex]; }
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


        HashSet<QueueInfo> queueList = new HashSet<QueueInfo>();
        public IReadOnlyCollection<QueueInfo> QueueList
        {
            get { return queueList; }
        }
        public event QueueInfoEventHandler OnQueueInfo;

        HashSet<HoldInfo> holdList = new HashSet<HoldInfo>();
        public IReadOnlyCollection<HoldInfo> HoldList
        {
            get { return holdList; }
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

        void ProcessStateChangedMessage(ConnectionInfo connectionInfo, ServerSentMessage msg)
        {
            AgentStateWorkType oldState;
            AgentStateChangedEventArgs<AgentStateWorkType> ev = null;
            var newState = new AgentStateWorkType((AgentState)msg.N1, (WorkType)msg.N2);
            lock (lck)
            {
                oldState = AgentStateWorkType.Clone() as AgentStateWorkType;
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

        void ProcessTeleStateChangedMessage(ConnectionInfo connectionInfo, ServerSentMessage msg)
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
            var ev = new QueueInfoEventArgs(info);
            // 记录 QueueInfo
            lock (lck)
            {
                // 修改之前,一律先删除
                queueList.RemoveWhere((m) => m == info);
                if (info.EventType != QueueEventType.Cancel)
                {
                    queueList.Add(info);
                }
            }
            /// 
            OnQueueInfo?.Invoke(this, ev);
        }

        void ProcessHoldInfoMessage(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var info = new HoldInfo(connInfo, msg);
            var ev = new HoldInfoEventArgs(info);
            // 记录 HoldInfo
            lock (lck)
            {
                // 修改之前,一律先删除
                holdList.RemoveWhere((m) => m == info);
                if (info.EventType != HoldEventType.Cancel)
                {
                    holdList.Add(info);
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
            var evt = new CustomStringReceivedEventArgs(val);
            OnCustomStringReceived?.Invoke(this, evt);
        }

        public event IvrDataReceivedEventHandler OnIvrDataReceived;
        private void DoOnIvrData(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            var val = new IvrData(connInfo, msg.N2, msg.S);
            var evt = new IvrDataReceivedEventArgs(val);
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
            var evt = new RingInfoReceivedEventArgs(_ringInfo);
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
            var evt = new WorkingChannelInfoReceivedEventArgs(info);
            lock (lck)
            {
                workingChannelInfo = info;
            }
            OnWorkingChannelInfoReceived?.Invoke(this, evt);
        }

        private HashSet<Privilege> privilegeSet = new HashSet<Privilege>();
        public IReadOnlyCollection<Privilege> PrivilegeSet
        {
            get { return privilegeSet; }
        }
        public event EventHandler OnPrivilegeSetReceived;
        private void DoOnPrivilegeList(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var parts = msg.S.Split(new char[] { '|' });
            lock (lck)
            {
                foreach (var s in parts)
                {
                    privilegeSet.Add((Privilege)Convert.ToInt32(s));
                }
            }
            OnPrivilegeSetReceived?.Invoke(this, new EventArgs());
        }

        private HashSet<int> privilegeExternSet = new HashSet<int>();
        public IReadOnlyCollection<int> PrivilegeExternSet
        {
            get { return privilegeExternSet; }
        }
        public event EventHandler OnPrivilegeExternSetReceived;
        private void DoOnPrivilegeExternList(ConnectionInfo connInfo, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var parts = msg.S.Split(new char[] { '|' });
            lock (lck)
            {
                foreach (var s in parts)
                {
                    privilegeExternSet.Add(Convert.ToInt32(s));
                }
            }
            OnPrivilegeExternSetReceived?.Invoke(this, new EventArgs());
        }

        List<AgentGroup> groupList = new List<AgentGroup>();
        public IReadOnlyCollection<AgentGroup> GroupSet
        {
            get { return groupList; }
        }
        public event EventHandler OnGroupListReceived;
        void DoOnGroupIdList(ConnectionInfo _, ServerSentMessage msg)
        {
            if (string.IsNullOrWhiteSpace(msg.S)) return;
            var parts = msg.S.Split(new char[] { '|' });
            lock (lck)
            {
                foreach (var s in parts)
                {
                    groupList.Add(new AgentGroup(s));
                }
            }
        }

        void DoOnGroupNameList(ConnectionInfo _, ServerSentMessage msg)
        {
            var parts = msg.S.Split(new char[] { '|' });
            lock (lck)
            {
                var it = groupList.Zip(parts, (first, second) => new { first, second });
                foreach (var m in it)
                {
                    m.first.Name = m.second;
                }
            }
            OnGroupListReceived?.Invoke(this, new EventArgs());
        }

        public event ChannelAssignedEventHandler OnChannelAssigned;
        void DoOnChannel(ConnectionInfo _, ServerSentMessage msg)
        {
            var evt = new ChannelAssignedEventArgs(msg.N2);
            lock (lck)
            {
                channel = evt.Channel;
            }
            OnChannelAssigned?.Invoke(this, evt);
        }

        public event AgentDisplayNameReceivedEventHandler OnAgentDisplayNameReceived;
        void DoOnAgentId(ConnectionInfo _, ServerSentMessage msg)
        {
            var evt = new AgentDisplayNameReceivedEventArgs(msg.S);
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


        public async Task Startup(string workerNumber, string password)
        {
            var it = connectionList.Zip(
                internalConnections,
                (info, conn) => new { info, conn }
            );
            var tasks = new List<Task>();
            foreach (var pair in it)
            {
                var conn = pair.conn;
                var info = pair.info;
                var task = conn.Open(info.Host, info.Port, workerNumber, password);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

    }
}
