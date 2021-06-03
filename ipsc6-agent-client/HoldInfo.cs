using System;
using System.Collections.Generic;

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
            return other != null
                && ConnectionInfo == other.ConnectionInfo
                && Channel == other.Channel;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HoldInfo);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"<{GetType().Name} Connection={ConnectionInfo}, Channel ={Channel}， EventType={EventType}， SessionId={SessionId}> ";
        }

        public static bool operator ==(HoldInfo left, HoldInfo right)
        {
            return EqualityComparer<HoldInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(HoldInfo left, HoldInfo right)
        {
            return !(left == right);
        }

    }
}
