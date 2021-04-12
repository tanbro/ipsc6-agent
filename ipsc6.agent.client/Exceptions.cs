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

    public class AgentRequestTimeoutException : AgentConnectException
    {
        public AgentRequestTimeoutException() { }
        public AgentRequestTimeoutException(string message) : base(message) { }
        public AgentRequestTimeoutException(string message, Exception inner) : base(message, inner) { }

    }
}
