using System;
using System.Linq;

namespace ipsc6.agent.client
{
    //ref: EventManager.cpp, EventQueueInfo.cpp

    public class QueueInfo : ServerSideData, IEquatable<QueueInfo>
    {
        public readonly int Channel;
        public readonly string GroupId;
        public readonly string Id;
        public readonly QueueInfoType Type;
        public readonly QueueEventType EventType;
        public readonly string SessionId;
        public readonly string CallingNo;
        public readonly string Username;
        public readonly string CustomeString;

        public QueueInfo(ConnectionInfo connectionInfo, ServerSentMessage msg) : base(connectionInfo)
        {
            Channel = msg.N1;
            Type = (QueueInfoType)msg.N2;
            var parts = msg.S.Split(Constants.VerticalBarDelimiter);
            var it = parts.Select((s, i) => new { s, i });
            foreach (var m in it)
            {
                switch (m.i)
                {
                    case 0:
                        GroupId = m.s;
                        break;
                    case 1:
                        EventType = (QueueEventType)Convert.ToInt32(m.s);
                        break;
                    case 2:
                        SessionId = m.s;
                        break;
                    case 3:
                        Id = m.s;
                        break;
                    case 4:
                        Username = m.s;
                        break;
                    case 5:
                        CallingNo = m.s;
                        break;
                    case 6:
                        CustomeString = m.s;
                        break;
                    default:
                        break;
                }
            }
        }

        public bool Equals(QueueInfo other)
        {
            return ConnectionInfo == other.ConnectionInfo && Channel == other.Channel;
        }

        public override bool Equals(object obj)
        {
            var that = obj as QueueInfo;
            return Equals(that);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(QueueInfo lhs, QueueInfo rhs)
        {
            return (lhs is object) && (rhs is object) && lhs.Equals(rhs);
        }

        public static bool operator !=(QueueInfo lhs, QueueInfo rhs)
        {
            return !(lhs == rhs);
        }

    }
}
