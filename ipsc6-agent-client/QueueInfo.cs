using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ipsc6.agent.client
{
    //ref: EventManager.cpp, EventQueueInfo.cpp

    public class QueueInfo: ServerSideData, IEquatable<QueueInfo>
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

        public QueueInfo(ConnectionInfo connectionInfo, ServerSentMessage msg): base(connectionInfo)
        {
            Channel = msg.N1;
            Type = (QueueInfoType)msg.N2;
            var parts = msg.S.Split(new char[] { '|' });
            var it = parts.Select((v, i) => new { v, i });
            foreach (var m in it)
            {
                switch (m.i)
                {
                    case 0:
                        GroupId = m.v;
                        break;
                    case 1:
                        EventType = (QueueEventType)Convert.ToInt32(m.v);
                        break;
                    case 2:
                        SessionId = m.v;
                        break;
                    case 3:
                        Id = m.v;
                        break;
                    case 4:
                        Username = m.v;
                        break;
                    case 5:
                        CallingNo = m.v;
                        break;
                    case 6:
                        CustomeString = m.v;
                        break;
                    default:
                        break;
                }
            }
        }

        public bool Equals(QueueInfo other)
        {
            return ConnectionInfo==other.ConnectionInfo && Channel == other.Channel;
        }

    }
}
