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
    public delegate void ConnectionStateChangedEventHandler(object sender, ConnectionStateChangedEventArgs<ConnectionState> e);
    public delegate void ConnectionInfoStateChangedEventHandler(object sender, ConnectionInfoStateChangedEventArgs<ConnectionState> e);

    public delegate void AgentStateChangedEventHandler(object sender, AgentStateChangedEventArgs<AgentState> e);

    public class ServerSentEventArgs : EventArgs
    {
        public readonly ServerSentMessage Message;
        public ServerSentEventArgs(ServerSentMessage message) : base()
        {
            Message = message;
        }
    }

    public class StateChangedEventArgs<T> : EventArgs
    {
        public readonly T CurrState;
        public readonly T NewState;

        public StateChangedEventArgs(T currState, T newState) : base()
        {
            CurrState = currState;
            NewState = newState;
        }
    }

    public class MyStateChangeEvent<T> : StateChangedEventArgs<int>
    {
        public MyStateChangeEvent(int currState, int newState) : base(currState, newState) { }
    }

    public class ConnectionStateChangedEventArgs<T> : StateChangedEventArgs<ConnectionState>
    {
        public ConnectionStateChangedEventArgs(ConnectionState currState, ConnectionState newState) : base(currState, newState) { }
    }

    public class ConnectionInfoStateChangedEventArgs<T> : StateChangedEventArgs<ConnectionState>
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

    public class BaseConnectorEventArgs : EventArgs { }
    public class ConnectorConnectedEventArgs : BaseConnectorEventArgs { }

    public class ConnectorConnectAttemptFailedEventArgs : BaseConnectorEventArgs { }
    public class ConnectorDisconnectedEventArgs : BaseConnectorEventArgs { }
    public class ConnectorConnectionLostEventArgs : BaseConnectorEventArgs { }

    public class AgentStateChangedEventArgs<T> : StateChangedEventArgs<AgentState>
    {
        public AgentStateChangedEventArgs(AgentState currState, AgentState newState) : base(currState, newState) { }
    }

}