using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client
{
    public class ConnectionInfo
    {
        public readonly string Host;
        public readonly ushort Port;

        public ConnectionInfo(string host, ushort port = 0)
        {
            Host = host;
            Port = port;
        }
    }
}