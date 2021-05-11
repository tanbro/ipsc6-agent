using System;
using System.Collections.Generic;

namespace ipsc6.agent.client
{
    public class ConnectionInfo : IEquatable<ConnectionInfo>
    {
        public string Host { get; }
        public ushort Port { get; }

        public ConnectionInfo(string host, ushort port = 0)
        {
            Host = host;
            Port = port;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(
                "<{0} at 0x{1:x8} Host={2}, Port={3}>",
                GetType().Name, GetHashCode(), Host, Port);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConnectionInfo);
        }

        public bool Equals(ConnectionInfo other)
        {
            return other != null &&
                   Host == other.Host &&
                   Port == other.Port;
        }

        public static bool operator ==(ConnectionInfo left, ConnectionInfo right)
        {
            return EqualityComparer<ConnectionInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(ConnectionInfo left, ConnectionInfo right)
        {
            return !(left == right);
        }
    }
}
