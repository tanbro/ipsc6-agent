namespace ipsc6.agent.client
{
    public class AgentRequestMessage
    {
        public MessageType Type { get; }
        public int N { get; }
        public string S { get; }

        public AgentRequestMessage(MessageType type, int n, string s)
        {
            Type = type;
            N = n;
            S = s;
        }

        public AgentRequestMessage(MessageType type, string s)
        {
            Type = type;
            N = 0;
            S = s;
        }

        public AgentRequestMessage(MessageType type, int n)
        {
            Type = type;
            N = n;
            S = "";
        }

        public AgentRequestMessage(MessageType type)
        {
            Type = type;
            N = 0;
            S = "";
        }

        public override string ToString()
        {
            return $"<{GetType().Name}@{GetHashCode():x8} Command={Type}, N={N}, S=\"{S}\">";
        }
    }
}
