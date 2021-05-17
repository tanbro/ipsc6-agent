using System;

// ref: agtctrl2/Events/EventHoldInfo.cpp

namespace ipsc6.agent.client
{
    public class HoldInfo : ServerSideData, IEquatable<HoldInfo>
    {
        public int Channel { get; }
        public HoldEventType EventType { get; }
        public string SessionId { get; }

        public HoldInfo(ConnectionInfo connectionInfo, ServerSentMessage msg) : base(connectionInfo)
        {
            Channel = msg.N1;
            EventType = (HoldEventType)msg.N2;
            SessionId = msg.S;
        }

        public bool Equals(HoldInfo other)
        {
            return ConnectionInfo == other.ConnectionInfo
                && Channel == other.Channel;
        }

        public override bool Equals(object obj)
        {
            var that = obj as HoldInfo;
            return Equals(that);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public static bool operator ==(HoldInfo lhs, HoldInfo rhs)
        {
            return (lhs is object) && (rhs is object) && lhs.Equals(rhs);
        }

        public static bool operator !=(HoldInfo lhs, HoldInfo rhs)
        {
            return !(lhs == rhs);
        }
    }
}
