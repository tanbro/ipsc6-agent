using System;
using System.Collections.Generic;

namespace ipsc6.agent.client
{
    public class CtiServer : IEquatable<CtiServer>
    {
        public string Host { get; }
        public ushort Port { get; }

        public CtiServer(string address)
        {
            var parts = address.Split(new char[] { ':' }, 2);
            Host = parts[0];
            if (parts.Length > 1)
                Port = ushort.Parse(parts[1]);
        }

        public CtiServer(string host, ushort port)
        {
            Host = host;
            Port = port;
        }

        public override string ToString()
        {
            return $"<{GetType().Name} {Host}|{Port}>";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CtiServer);
        }

        public bool Equals(CtiServer other)
        {
            return other != null
                && Host == other.Host
                && Port == other.Port;
        }

        public override int GetHashCode()
        {
            int hashCode = 995452845;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Host);
            hashCode = hashCode * -1521134295 + Port.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(CtiServer left, CtiServer right)
        {
            return EqualityComparer<CtiServer>.Default.Equals(left, right);
        }

        public static bool operator !=(CtiServer left, CtiServer right)
        {
            return !(left == right);
        }

    }
}
