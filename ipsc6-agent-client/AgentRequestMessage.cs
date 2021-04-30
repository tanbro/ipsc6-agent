using ipsc6.agent.network;

namespace ipsc6.agent.client
{
    public class AgentRequestMessage
    {
        public readonly MessageType Type;
        public readonly int N;
        public readonly string S;

        public AgentRequestMessage(MessageType type, int n = 0, string s = "")
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

        public override string ToString()
        {
            return string.Format(
                "<{0} at 0x{1:x8} CommandType={2}, N={3}, S=\"{4}\">",
                GetType().Name, GetHashCode(), Type, N, S
            );
        }
    }
}
