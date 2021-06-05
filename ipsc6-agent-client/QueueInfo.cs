using System;
using System.Collections.Generic;
using System.Linq;

namespace ipsc6.agent.client
{
    //ref: EventManager.cpp, EventQueueInfo.cpp

    public class QueueInfo : ServerSideData, IEquatable<QueueInfo>
    {
        public int Channel { get; }
        public string Id { get; }
        public QueueInfoType Type { get; }
        public QueueEventType EventType { get; }
        public string SessionId { get; }
        public string CallingNo { get; }
        public string WorkerNum { get; }
        public string CustomeString { get; }
        private readonly HashSet<AgentGroup> groups = new HashSet<AgentGroup>();
        public IReadOnlyCollection<AgentGroup> Groups => groups;

        public QueueInfo(ConnectionInfo connectionInfo, ServerSentMessage msg, IReadOnlyCollection<AgentGroup> refGroups) : base(connectionInfo)
        {
            Channel = msg.N1;
            Type = (QueueInfoType)msg.N2;
            var parts = msg.S.Split(Constants.SemicolonBarDelimiter);
            foreach (var pair in parts.Select((str, index) => (str, index)))
            {
                switch (pair.index)
                {
                    case 0:
                        foreach (var id in pair.str.Split(Constants.VerticalBarDelimiter))
                        {
                            var groupObj = refGroups.FirstOrDefault(m => m.Id == id);
                            if (groupObj != null)
                            {
                                groups.Add(groupObj);
                            }
                        }
                        break;
                    case 1:
                        EventType = (QueueEventType)int.Parse(pair.str);
                        break;
                    case 2:
                        SessionId = pair.str;
                        break;
                    case 3:
                        Id = pair.str;
                        break;
                    case 4:
                        WorkerNum = pair.str;
                        break;
                    case 5:
                        CallingNo = pair.str;
                        break;
                    case 6:
                        CustomeString = pair.str;
                        break;
                    default:
                        break;
                }
            }
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

        public override int GetHashCode()
        {
            int hashCode = 379874733;
            hashCode = hashCode * -1521134295 + EqualityComparer<ConnectionInfo>.Default.GetHashCode(ConnectionInfo);
            hashCode = hashCode * -1521134295 + Channel.GetHashCode();
            return hashCode;
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
