namespace ipsc6.agent.client
{
    public class ServerSideData
    {
        public ConnectionInfo ConnectionInfo { get; private set; }

        public ServerSideData(ConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo;
        }

    }
}
