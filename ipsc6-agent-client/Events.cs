using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ipsc6.agent.network;

namespace ipsc6.agent.client
{
    public delegate void ServerSentEventHandler(object sender, ServerSentEventArgs e);
    public delegate void ClosedEventHandler(object sender);
    public delegate void LostEventHandler(object sender);
    public delegate void ConnectionStateChangedEventHandler(object sender, ConnectionStateChangedEventArgs e);
    public delegate void ConnectionInfoStateChangedEventHandler(object sender, ConnectionInfoStateChangedEventArgs e);

    public class ServerSentEventArgs: EventArgs
    {
        public readonly ServerSentMessage Message;
        public ServerSentEventArgs(ServerSentMessage message) : base()
        {
            Message = message;
        }
    }

    public class ConnectionStateChangedEventArgs : EventArgs
    {
        public readonly ConnectionState CurrState;
        public readonly ConnectionState NewState;

        public ConnectionStateChangedEventArgs(ConnectionState currState, ConnectionState newState) : base()
        {
            CurrState = currState;
            NewState = newState;
        }
    }

    public class ConnectionInfoStateChangedEventArgs : ConnectionStateChangedEventArgs
    {
        public readonly int Index;
        public readonly ConnectionInfo ConnectionInfo;

        public ConnectionInfoStateChangedEventArgs(
            int index, ConnectionInfo connectionInfo,
            ConnectionState currState, ConnectionState newState
        ) : base(currState, newState)
        {
            Index = index;
            ConnectionInfo = connectionInfo;
        }
    }

    class BaseConnectorEventArgs : EventArgs { }
    class ConnectorConnectedEventArgs: BaseConnectorEventArgs { }

    class ConnectorConnectAttemptFailedEventArgs : BaseConnectorEventArgs { }
    class ConnectorDisconnectedEventArgs : BaseConnectorEventArgs { }
    class ConnectorConnectionLostEventArgs : BaseConnectorEventArgs { }
}
