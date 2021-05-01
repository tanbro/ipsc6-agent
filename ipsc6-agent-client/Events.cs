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

    public delegate void AgentStateChangedEventHandler(object sender, AgentStateChangedEventArgs<AgentStateWorkType> e);
    public delegate void TeleStateChangedEventHandler(object sender, TeleStateChangedEventArgs<TeleState> e);
    public delegate void QueueInfoEventHandler(object sender, QueueInfoEventArgs e);
    public delegate void HoldInfoEventHandler(object sender, HoldInfoEventArgs e);

    public delegate void AgentIdAssignedEventHandler(object sender, AgentIdAssignedEventArgs e);
    public delegate void AgentDisplayNameReceivedEventHandler(object sender, AgentDisplayNameReceivedEventArgs e);
    public delegate void ChannelAssignedEventHandler(object sender, ChannelAssignedEventArgs e);
    public delegate void WorkingChannelInfoReceivedEventHandler(object sender, WorkingChannelInfoReceivedEventArgs e);
    public delegate void RingInfoReceivedEventHandler(object sender, RingInfoReceivedEventArgs e);
    public delegate void IvrDataReceivedEventHandler(object sender, IvrDataReceivedEventArgs e);
    public delegate void CustomStringReceivedEventArgsReceivedEventHandler(object sender, CustomStringReceivedEventArgs e);
    public delegate void SipRegistrarListReceivedEventHandler(object sender, SipRegistrarListReceivedEventArgs e);

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
        public readonly T OldState;
        public readonly T NewState;

        public StateChangedEventArgs(T oldState, T newState) : base()
        {
            OldState = oldState;
            NewState = newState;
        }
    }

    public class ConnectionStateChangedEventArgs<T> : StateChangedEventArgs<ConnectionState>
    {
        public ConnectionStateChangedEventArgs(ConnectionState oldState, ConnectionState newState) : base(oldState, newState) { }
    }

    public class ConnectionInfoStateChangedEventArgs<T> : StateChangedEventArgs<ConnectionState>
    {
        public readonly ConnectionInfo ConnectionInfo;

        public ConnectionInfoStateChangedEventArgs(
            ConnectionInfo connectionInfo,
            ConnectionState oldState, ConnectionState newState
        ) : base(oldState, newState)
        {
            ConnectionInfo = connectionInfo;
        }
    }

    public class BaseConnectorEventArgs : EventArgs { }
    public class ConnectorConnectedEventArgs : BaseConnectorEventArgs { }

    public class ConnectorConnectAttemptFailedEventArgs : BaseConnectorEventArgs { }
    public class ConnectorDisconnectedEventArgs : BaseConnectorEventArgs { }
    public class ConnectorConnectionLostEventArgs : BaseConnectorEventArgs { }

    public class AgentStateChangedEventArgs<T> : StateChangedEventArgs<AgentStateWorkType>
    {
        public AgentStateChangedEventArgs(AgentStateWorkType oldState, AgentStateWorkType newState) : base(oldState, newState) { }
    }

    public class TeleStateChangedEventArgs<T> : StateChangedEventArgs<TeleState>
    {
        public TeleStateChangedEventArgs(TeleState oldState, TeleState newState) : base(oldState, newState) { }
    }

    public class QueueInfoEventArgs: EventArgs
    {
        public readonly QueueInfo Info;
        public QueueInfoEventArgs(QueueInfo info) : base()
        {
            Info = info;
        }
    }

    public class HoldInfoEventArgs : EventArgs
    {
        public readonly HoldInfo Info;
        public HoldInfoEventArgs(HoldInfo info) : base()
        {
            Info = info;
        }
    }

    public class AgentIdAssignedEventArgs: EventArgs
    {
        public readonly int Id;
        public readonly string DisplayName;

        public AgentIdAssignedEventArgs(int id, string displayName)
        {
            Id = id;
            DisplayName = displayName;
        }
    }

    public class AgentDisplayNameReceivedEventArgs : EventArgs
    {
        public readonly string Value;

        public AgentDisplayNameReceivedEventArgs(string value)
        {
            Value = value;
        }
    }
    
    public class ChannelAssignedEventArgs: EventArgs
    {
        public readonly int Channel;
        public ChannelAssignedEventArgs(int channel)
        {
            Channel = channel;
        }
    }

    public class WorkingChannelInfoReceivedEventArgs: EventArgs
    {
        public readonly WorkingChannelInfo Value;
        public WorkingChannelInfoReceivedEventArgs(WorkingChannelInfo value) :base()
        {
            Value = value;
        }
    }

    public class RingInfoReceivedEventArgs : EventArgs
    {
        public readonly RingInfo Value;
        public RingInfoReceivedEventArgs(RingInfo value) : base()
        {
            Value = value;
        }
    }

    public class IvrDataReceivedEventArgs : EventArgs
    {
        public readonly IvrData Value;
        public IvrDataReceivedEventArgs(IvrData value) : base()
        {
            Value = value;
        }
    }

    public class CustomStringReceivedEventArgs : EventArgs
    {
        public readonly ServerSentCustomString Value;
        public CustomStringReceivedEventArgs(ServerSentCustomString value) : base()
        {
            Value = value;
        }
    }

    public class SipRegistrarListReceivedEventArgs : EventArgs
    {
        public readonly string[] Value;
        public SipRegistrarListReceivedEventArgs(string[] value) : base()
        {
            Value = value;
        }
    }
}
