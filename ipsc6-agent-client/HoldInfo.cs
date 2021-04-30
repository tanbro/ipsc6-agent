using System;
using System.Collections.Generic;
using System.Text;

// ref: agtctrl2/Events/EventHoldInfo.cpp

namespace ipsc6.agent.client
{
    public class HoldInfo: IEquatable<HoldInfo>
    {
        public readonly int Channel;
        public readonly HoldEventType EventType;
        public readonly string SessionId;

        public HoldInfo(ServerSentMessage msg)
        {
            Channel = msg.N1;
            EventType = (HoldEventType)msg.N2;
            SessionId = msg.S;
        }

        public bool Equals(HoldInfo other)
        {
            return Channel == other.Channel;
        }
    }
}
