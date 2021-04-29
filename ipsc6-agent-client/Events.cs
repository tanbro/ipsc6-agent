using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ipsc6.agent.network;

namespace ipsc6.agent.client
{
    public delegate void ServerSendEventHandler(object sender, AgentMessageReceivedEventArgs e);
    public delegate void ClosedEventHandler(object sender);
    public delegate void LostEventHandler(object sender);
    public delegate void ConnectionStateChangedEventHandler(object sender, ConnectionStateChangedEventArgs e);
    public delegate void AgentConnectionStateChangedEventHandler(object sender, AgentConnectionStateChangedEventArgs e);

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

    public class AgentConnectionStateChangedEventArgs : ConnectionStateChangedEventArgs
    {
        public readonly int ServerIndex;
        public readonly ServerAddress ServerAddress;

        public AgentConnectionStateChangedEventArgs(
            int serverIndex, ServerAddress serverAddress,
            ConnectionState currState, ConnectionState newState
        ) : base(currState, newState)
        {
            ServerIndex = serverIndex;
            ServerAddress = serverAddress;
        }
    }
}
