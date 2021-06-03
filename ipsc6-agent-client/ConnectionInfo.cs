using System;
using System.Collections.Generic;

namespace ipsc6.agent.client
{
    public class ConnectionInfo : IEquatable<ConnectionInfo>
    {
        public string Host { get; }
        public ushort Port { get; }

        public ConnectionInfo(string address)
        {
            var parts = address.Split(new char[] { ':' }, 2);
            Host = parts[0];
            if (parts.Length > 1)
                Port = ushort.Parse(parts[1]);
        }

        public ConnectionInfo(string host, ushort port)
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
            return $"<{GetType().Name} {Host}|{Port}>";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ConnectionInfo);
        }

        public bool Equals(ConnectionInfo other)
        {
            return other != null
                && Host == other.Host
                && Port == other.Port;
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
