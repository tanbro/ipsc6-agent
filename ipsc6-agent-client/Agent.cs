using System;
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

        public event AgentStateChangedEventHandler OnAgentStateChanged;

        private readonly object lck = new object();

        private AgentState agentState = AgentState.NotExist;
        public AgentState AgentState
        {
            get { return agentState; }
        }

        private void Conn_OnServerSend(object sender, ServerSentEventArgs e)
        {
            var msg = e.Message;
            switch (msg.Type)
            {
                case MessageType.REMOTE_MSG_SETSTATE:
                    /// 状态改变
                    {
                        AgentState currState;
                        AgentState newState = (AgentState)msg.N1;
                        AgentStateChangedEventArgs<AgentState> ev = null;
                        lock (lck)
                        {
                            currState = agentState;
                            if (currState != newState)
                            {
                                ev = new AgentStateChangedEventArgs<AgentState>(currState, newState);
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
                    break;
                case MessageType.REMOTE_MSG_SETTELEMODE:
                    /// 话机模式改变
                    break;
                case MessageType.REMOTE_MSG_QUEUEINFO:
                    /// 排队信息
                    break;
                case MessageType.REMOTE_MSG_HOLDINFO:
                    /// 保持信息
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
            var e_ = new ConnectionInfoStateChangedEventArgs<ConnectionState>(index, connInfo, e.CurrState, e.NewState);
            OnConnectionStateChanged?.Invoke(this, e_);
        }

        public event ConnectionInfoStateChangedEventHandler OnConnectionStateChanged;

    }
}
