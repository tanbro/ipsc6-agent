using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ipsc6.agent.client
{
    public class Agent
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Connection));

        public string Username;

        private readonly List<Connection> internalConnections;
        private readonly List<ConnectionInfo> connectionList;
        public IReadOnlyCollection<ConnectionInfo> ConnectionList
        {
            get { return connectionList; }
        }

        public Agent(string username, IEnumerable<ConnectionInfo> connections)
        {
            Username = username;
            internalConnections = new List<Connection>();
            connectionList = new List<ConnectionInfo>();
            foreach (var m in connections)
            {
                var conn = new Connection();
                conn.OnConnectionStateChanged += Conn_OnConnectionStateChanged;
                conn.OnServerSentEvent += Conn_OnServerSend;
                internalConnections.Add(conn);
                connectionList.Add(m);
            }
        }

        private readonly object lck = new object();


        private AgentState agentState = AgentState.NotExist;
        private WorkType workType = WorkType.Unknown;
        public AgentState AgentState
        {
            get { return agentState; }
        }
        public WorkType WorkType
        {
            get { return workType; }
        }
        private void SetAgentStateWorkType(AgentStateWorkType value)
        {
            agentState = value.AgentState;
            workType = value.WorkType;
        }
        public AgentStateWorkType AgentStateWorkType
        {
            get { return new AgentStateWorkType(agentState, workType); }
        }
        public event AgentStateChangedEventHandler OnAgentStateChanged;


        private TeleState teleState = TeleState.HangUp;
        public TeleState TeleState
        {
            get { return teleState; }
        }
        public event TeleStateChangedEventHandler OnTeleStateChanged;


        private HashSet<QueueInfo> queueList = new HashSet<QueueInfo>();
        public IReadOnlyCollection<QueueInfo> QueueList
        {
            get { return queueList; }
        }
        public event QueueInfoEventHandler OnQueueInfo;

        private HashSet<HoldInfo> holdList = new HashSet<HoldInfo>();
        public IReadOnlyCollection<HoldInfo> HoldList
        {
            get { return holdList; }
        }
        public event HoldInfoEventHandler OnHoldInfo;

        private void Conn_OnServerSend(object sender, ServerSentEventArgs e)
        {
            var msg = e.Message;
            switch (msg.Type)
            {
                case MessageType.REMOTE_MSG_SETSTATE:
                    /// 状态改变
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
                    break;
                case MessageType.REMOTE_MSG_SETTELESTATE:
                    /// 电话状态改变
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
                    break;
                case MessageType.REMOTE_MSG_QUEUEINFO:
                    /// 排队信息
                    {
                        var info = new QueueInfo(msg);
                        var ev = new QueueInfoEventArgs(info);
                        // 记录 QueueInfo
                        // 修改之前,一律先删除
                        queueList.RemoveWhere((m) => m == info);
                        if (info.EventType != QueueEventType.Cancel)
                        {
                            queueList.Add(info);
                        }
                        /// 
                        OnQueueInfo?.Invoke(this, ev);
                    }
                    break;
                case MessageType.REMOTE_MSG_HOLDINFO:
                    /// 保持信息
                    {
                        var info = new HoldInfo(msg);
                        var ev = new HoldInfoEventArgs(info);
                        // 记录 HoldInfo
                        // 修改之前,一律先删除
                        holdList.RemoveWhere((m) => m == info);
                        if (info.EventType != HoldEventType.Cancel)
                        {
                            holdList.Add(info);
                        }
                        //
                        OnHoldInfo?.Invoke(this, ev);
                    }
                    break;
                case MessageType.REMOTE_MSG_SENDDATA:
                    /// 其他各种数据
                    break;
                default:
                    break;
            }
        }

        private void Conn_OnConnectionStateChanged(object sender, ConnectionStateChangedEventArgs<ConnectionState> e)
        {
            var conn = sender as Connection;
            var index = internalConnections.IndexOf(conn);
            var connInfo = connectionList[index];
            var e_ = new ConnectionInfoStateChangedEventArgs<ConnectionState>(index, connInfo, e.OldState, e.NewState);
            OnConnectionStateChanged?.Invoke(this, e_);
        }

        public event ConnectionInfoStateChangedEventHandler OnConnectionStateChanged;

    }
}
