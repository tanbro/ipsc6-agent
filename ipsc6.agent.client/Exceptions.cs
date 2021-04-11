using System;

namespace ipsc6.agent.client
{
    public class BaseAgentException : Exception
    {
        public BaseAgentException() { }
        public BaseAgentException(string message) : base(message) { }
        public BaseAgentException(string message, Exception inner) : base(message, inner) { }
    }

    public class AgentConnectFailedException : BaseAgentException { }
}
