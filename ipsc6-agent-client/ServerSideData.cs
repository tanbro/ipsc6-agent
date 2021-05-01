using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client
{
    public class ServerSideData
    {
        public readonly ConnectionInfo ConnectionInfo;

        public ServerSideData(ConnectionInfo connectionInfo)
        {
            ConnectionInfo = connectionInfo;
        }
    }
}
