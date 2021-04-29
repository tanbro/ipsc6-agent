using System;
using System.Collections.Generic;
using System.Text;

namespace ipsc6.agent.client
{
    public class ServerAddress : IEquatable<ServerAddress>
    {
        public readonly string Host;
        public readonly ushort Port;

        public ServerAddress(string host, ushort port=0)
        {
            Host = host;
            Port = port;
        }

        public bool Equals(ServerAddress other)
        {
            return (
                (Host == other.Host) &&
                (Port == other.Port)
            );
        }

    }
}
