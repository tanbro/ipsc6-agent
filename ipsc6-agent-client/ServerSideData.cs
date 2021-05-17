namespace ipsc6.agent.client
{
    public class ServerSideData
    {
        private readonly ConnectionInfo connectionInfo;
        public ConnectionInfo ConnectionInfo => connectionInfo;

        public ServerSideData(ConnectionInfo connectionInfo)
        {
            this.connectionInfo = connectionInfo;
        }

    }
}
