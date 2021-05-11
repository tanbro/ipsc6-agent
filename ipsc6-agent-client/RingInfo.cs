using System;
using System.Collections.Generic;

namespace ipsc6.agent.client
{
    public class RingInfo : ServerSideData, IEquatable<RingInfo>
    {
        public int WorkingChannel { get; }
        public string CustomString { get; }
        public RingInfo(ConnectionInfo connectionInfo, int workingChannel, string customString = "") : base(connectionInfo)
        {
            WorkingChannel = workingChannel;
            CustomString = customString;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RingInfo);
        }

        public bool Equals(RingInfo other)
        {
            return other != null
                && EqualityComparer<ConnectionInfo>.Default.Equals(ConnectionInfo, other.ConnectionInfo)
                && WorkingChannel == other.WorkingChannel;
        }

        public static bool operator ==(RingInfo left, RingInfo right)
        {
            return EqualityComparer<RingInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(RingInfo left, RingInfo right)
        {
            return !(left == right);
        }
    }
}
