using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ipsc6.agent.client
{
    public class Agent
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(Connection));

        public string WorkerNumber;
        public List<ServerAddress> ServerList = new List<ServerAddress>();
        private Dictionary<ServerAddress, Connection> connectionDict = new Dictionary<ServerAddress, Connection>();

        public Agent(string workerNumber, IEnumerable<ServerAddress> serverList)
        {
            WorkerNumber = workerNumber;
            foreach (var obj in serverList)
            {
                ServerList.Add(obj);
            }
            Initialize();
        }

        private void Initialize()
        {
            foreach (var addr in ServerList)
            {
                var connection = new Connection();
                connectionDict.Add(addr, connection);
            }
        }

        public event AgentConnectionStateChangedEventHandler OnConnectionStateChanged;

        private ServerAddress GetConnectionServerAddress(Connection connection, out int index )
        {
            index = -1;
            lock (connectionDict)
            {
                for (var i = 0; i < ServerList.Count; i++)
                {
                    var addr = ServerList[i];
                    var conn = connectionDict[addr];
                    if (connection == conn)
                    {
                        index = i;
                        return addr;
                    }
                }
            }
            return null;
        }

        private ConnectionState GetConnectionState(int serverIndex)
        {
            var addr = ServerList[serverIndex];
            var conn = connectionDict[addr];
            return conn.State;
        }

        private ConnectionState GetConnectionState(ServerAddress serverAddress)
        {
            var addr = ServerList.Find(m => m == serverAddress);
            var conn = connectionDict[addr];
            return conn.State;
        }

        private void Connection_OnStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            Connection connection = (Connection)sender;
            ServerAddress addr = GetConnectionServerAddress(connection, out int index);
            if ((index < 0) || (addr == null))
            {
                throw new NullReferenceException();
            }
            var e_ = new AgentConnectionStateChangedEventArgs(index, addr, e.CurrState, e.NewState);
            OnConnectionStateChanged?.Invoke(this, e_);
        }

        private void Connection_OnServerSendEvent(object sender, network.AgentMessageReceivedEventArgs e)
        {
            switch (e.CommandType)
            {
                case network.AgentMessage.REMOTE_MSG_SENDDATA:
                    break;
                default:
                    break;
            }
        }

        public void Start(string password)
        {
            var tasks = new List<Task>();
            foreach (var entry in connectionDict)
            {
                var addr = entry.Key;
                var conn = entry.Value;
                Task.Run(() =>
                {
                    conn.Open(addr.Host, addr.Port).Wait();
                });
            }
        }
    }
}
