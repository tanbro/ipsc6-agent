using System;

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

        public bool Equals(ConnectionInfo other)
        {
            return Host == other.Host && Port == other.Port;
        }

        public override bool Equals(object obj)
        {
            var that = obj as ConnectionInfo;
            return Equals(that);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(ConnectionInfo lhs, ConnectionInfo rhs)
        {
            return (lhs is object) && (rhs is object) && lhs.Equals(rhs);
        }

        public static bool operator !=(ConnectionInfo lhs, ConnectionInfo rhs)
        {
            return !(lhs == rhs);
        }

        public override string ToString()
        {
            return string.Format(
                "<{0} at 0x{1:x8} Host={2}, Port={3}>",
                GetType().Name, GetHashCode(), Host, Port);
        }

    }
}
