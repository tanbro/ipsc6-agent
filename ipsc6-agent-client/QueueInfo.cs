using System;
using System.Collections.Generic;
using System.Linq;

namespace ipsc6.agent.client
{
    //ref: EventManager.cpp, EventQueueInfo.cpp

    public class QueueInfo : ServerSideData, IEquatable<QueueInfo>
    {
        public int Channel { get; }
        public string GroupId { get; }
        public string Id { get; }
        public QueueInfoType Type { get; }
        public QueueEventType EventType { get; }
        public string SessionId { get; }
        public string CallingNo { get; }
        public string Username { get; }
        public string CustomeString { get; }

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

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as QueueInfo);
        }

        public bool Equals(QueueInfo other)
        {
            return other != null &&
                   EqualityComparer<ConnectionInfo>.Default.Equals(ConnectionInfo, other.ConnectionInfo) &&
                   Channel == other.Channel;
        }

        public static bool operator ==(QueueInfo left, QueueInfo right)
        {
            return EqualityComparer<QueueInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(QueueInfo left, QueueInfo right)
        {
            return !(left == right);
        }
    }
}
