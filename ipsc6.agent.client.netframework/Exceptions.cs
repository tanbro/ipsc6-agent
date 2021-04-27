using System;

namespace ipsc6.agent.client
{
    public class BaseAgentException : Exception
    {
        public BaseAgentException() { }
        public BaseAgentException(string message) : base(message) { }
        public BaseAgentException(string message, Exception inner) : base(message, inner) { }
    }

    public class AgentConnectException : BaseAgentException
    {
        public AgentConnectException() { }
        public AgentConnectException(string message) : base(message) { }
        public AgentConnectException(string message, Exception inner) : base(message, inner) { }
    }

    public class AgentConnectFailedException : AgentConnectException
    {
        public AgentConnectFailedException() { }
        public AgentConnectFailedException(string message) : base(message) { }
        public AgentConnectFailedException(string message, Exception inner) : base(message, inner) { }
    }

    public class AgentConnectLostException : AgentConnectException
    {
        public AgentConnectLostException() { }
        public AgentConnectLostException(string message) : base(message) { }
        public AgentConnectLostException(string message, Exception inner) : base(message, inner) { }
    }

    public class AgentDisconnectedException : AgentConnectException
    {
        public AgentDisconnectedException() { }
        public AgentDisconnectedException(string message) : base(message) { }
        public AgentDisconnectedException(string message, Exception inner) : base(message, inner) { }
    }

    public class AgentRequestException : BaseAgentException
    {
        public AgentRequestException() { }
        public AgentRequestException(string message) : base(message) { }
        public AgentRequestException(string message, Exception inner) : base(message, inner) { }
    }

    public class AgentRequestTimeoutException : AgentRequestException
    {
        public AgentRequestTimeoutException() { }
        public AgentRequestTimeoutException(string message) : base(message) { }
        public AgentRequestTimeoutException(string message, Exception inner) : base(message, inner) { }
    }

}
