using System;

namespace ipsc6.agent.client
{

    public class ServerSentEventArgs : EventArgs
    {
        public ServerSentMessage Message { get; }
        public ServerSentEventArgs(ServerSentMessage message) : base()
        {
            Message = message;
        }
    }

    public class StateChangedEventArgs<T> : EventArgs
    {
        public T OldState { get; }
        public T NewState { get; }
        public StateChangedEventArgs(T oldState, T newState) : base()
        {
            OldState = oldState;
            NewState = newState;
        }
    }

    public class ConnectionStateChangedEventArgs : StateChangedEventArgs<ConnectionState>
    {
        public ConnectionStateChangedEventArgs(ConnectionState oldState, ConnectionState newState) : base(oldState, newState) { }
    }

    public class ConnectionInfoStateChangedEventArgs : StateChangedEventArgs<ConnectionState>
    {
        public CtiServer ConnectionInfo { get; }
        public ConnectionInfoStateChangedEventArgs(
            CtiServer connectionInfo,
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

    public class BaseCtiEventArgs<T> : EventArgs
    {
        public CtiServer CtiServer { get; }
        public T Value { get; }
        public BaseCtiEventArgs(CtiServer ctiServer, T value)
        {
            CtiServer = ctiServer;
            Value = value;
        }
    }

    public class AgentStateChangedEventArgs : StateChangedEventArgs<AgentStateWorkType>
    {
        public AgentStateChangedEventArgs(AgentStateWorkType oldState, AgentStateWorkType newState) : base(oldState, newState) { }
    }

    public class TeleStateChangedEventArgs : StateChangedEventArgs<TeleState>
    {
        public TeleStateChangedEventArgs(TeleState oldState, TeleState newState) : base(oldState, newState) { }
    }


    public class QueueInfoEventArgs : BaseCtiEventArgs<QueueInfo>
    {
        public QueueInfoEventArgs(CtiServer connectionInfo, QueueInfo value) : base(connectionInfo, value) { }
    }

    public class HoldInfoEventArgs : BaseCtiEventArgs<Call>
    {
        public HoldInfoEventArgs(CtiServer connectionInfo, Call value) : base(connectionInfo, value) { }
    }

    public struct AgentIdName
    {
        public int Id { get; }
        public string DisplayName { get; }
    }

    public class AgentIdAssignedEventArgs : BaseCtiEventArgs<AgentIdName>
    {
        public AgentIdAssignedEventArgs(CtiServer connectionInfo, AgentIdName value) : base(connectionInfo, value) { }
    }

    public class AgentDisplayNameReceivedEventArgs : BaseCtiEventArgs<string>
    {
        public AgentDisplayNameReceivedEventArgs(CtiServer connectionInfo, string value) : base(connectionInfo, value) { }
    }

    public class ChannelAssignedEventArgs : BaseCtiEventArgs<int>
    {
        public ChannelAssignedEventArgs(CtiServer connectionInfo, int value) : base(connectionInfo, value) { }
    }

    public class WorkingChannelInfoReceivedEventArgs : BaseCtiEventArgs<WorkingChannelInfo>
    {
        public WorkingChannelInfoReceivedEventArgs(CtiServer connectionInfo, WorkingChannelInfo value) : base(connectionInfo, value) { }
    }

    public class RingInfoReceivedEventArgs : BaseCtiEventArgs<Call>
    {
        public RingInfoReceivedEventArgs(CtiServer connectionInfo, Call value) : base(connectionInfo, value) { }
    }

    public class IvrDataReceivedEventArgs : BaseCtiEventArgs<IvrData>
    {
        public IvrDataReceivedEventArgs(CtiServer connectionInfo, IvrData value) : base(connectionInfo, value) { }
    }

    public class CustomStringReceivedEventArgs : BaseCtiEventArgs<ServerSentCustomString>
    {
        public CustomStringReceivedEventArgs(CtiServer connectionInfo, ServerSentCustomString value) : base(connectionInfo, value) { }
    }

    public class SipRegistrarListReceivedEventArgs : EventArgs
    {
        public string[] Value { get; }
        public SipRegistrarListReceivedEventArgs(string[] value) : base()
        {
            Value = value;
        }
    }

}
