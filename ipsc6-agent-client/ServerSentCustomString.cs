namespace ipsc6.agent.client
{
    public class ServerSentCustomString : ServerSideData
    {
        public int N { get; }
        public string S { get; }
        public ServerSentCustomString(ConnectionInfo connectionInfo, int n, string s) : base(connectionInfo)
        {
            N = n;
            S = s;
        }
    }
}
