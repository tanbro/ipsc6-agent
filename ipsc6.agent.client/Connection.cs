using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ipsc6.agent.network;


namespace ipsc6.agent.client
{
    class Connection
    {
        private Connector connector;
        private TaskCompletionSource<int> tcsConnected = null;

        private void Initialize(Connector connector)
        {
            this.connector = connector;
            this.connector.OnConnectAttemptFailed += Connector_OnConnectAttemptFailed;
            this.connector.OnConnected += Connector_OnConnected;
            this.connector.OnDisconnected += Connector_OnDisconnected;
            this.connector.OnConnectionLost += Connector_OnConnectionLost;
            this.connector.OnAgentMessageReceived += Connector_OnAgentMessageReceived;
        }

        private void Connector_OnAgentMessageReceived(object sender, AgentMessageReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Connector_OnConnectionLost(object sender)
        {
            if (tcsConnected.Task.Status == TaskStatus.Running)
            {
                tcsConnected.SetException(new AgentConnectFailedException());
                return;
            }
        }

        private void Connector_OnDisconnected(object sender)
        {
            if (tcsConnected.Task.Status == TaskStatus.Running)
            {
                tcsConnected.SetException(new AgentConnectFailedException());
                return;
            }
        }

        private void Connector_OnConnected(object sender, ConnectedEventArgs e)
        {
            tcsConnected.SetResult(e.ConnectionId);
        }

        private void Connector_OnConnectAttemptFailed(object sender)
        {
            tcsConnected.SetException(new AgentConnectFailedException());
        }

        public Connection(Connector connector)
        {
            Initialize(connector);
        }

        public Connection(ushort localPort = 0, string address = "")
        {
            var connector = Connector.CreateInstance(localPort, address);
            Initialize(connector);
        }

        public Task<int> Connect(string host, ushort port = 0)
        {
            if (port == 0) port = Connector.DEFAULT_REMOTE_PORT;
            tcsConnected = new();
            connector.Connect(host, port);
            return tcsConnected.Task;
        }
    }
}
