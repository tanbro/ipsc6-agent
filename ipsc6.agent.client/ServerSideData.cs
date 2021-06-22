namespace ipsc6.agent.client
{
    public class ServerSideData
    {
        public CtiServer ConnectionInfo { get; }

        public ServerSideData(CtiServer connectionInfo)
        {
            ConnectionInfo = connectionInfo;
        }

    }
}
