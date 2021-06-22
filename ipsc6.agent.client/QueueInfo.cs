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
        public QueueEventType EventType { get; }
        public QueueInfoType Type { get; }
        public long ProcessId { get; }
        public string CallingNo { get; }
        public string WorkerNum { get; }
        public string CustomeString { get; }
        private readonly HashSet<Group> groups = new();
        public IReadOnlyCollection<Group> Groups => groups;

        public QueueInfo(CtiServer connectionInfo, ServerSentMessage msg, IReadOnlyCollection<Group> refGroups) : base(connectionInfo)
        {
            Channel = msg.N1;
            EventType = (QueueEventType)msg.N2;
            var parts = msg.S.Split(Constants.SemicolonBarDelimiter);
            foreach (var pair in parts.Select((s, i) => (s, i)))
            {
                var i = pair.i;
                var s = pair.s;
                switch (i)
                {
                    case 0:
                        foreach (var id in s.Split(Constants.VerticalBarDelimiter))
                        {
                            var groupObj = refGroups.FirstOrDefault(m => m.Id == id);
                            if (groupObj != null && !groups.Add(groupObj))
                                throw new InvalidOperationException();
                        }
                        break;
                    case 1:
                        Type = (QueueInfoType)Enum.Parse(typeof(QueueInfoType), s);
                        break;
                    case 2:
#pragma warning disable CA1305
                        ProcessId = long.Parse(s);
#pragma warning restore CA1305
                        break;
                    case 3:
                        Id = s;
                        break;
                    case 4:
                        WorkerNum = s;
                        break;
                    case 5:
                        CallingNo = s;
                        break;
                    case 6:
                        CustomeString = s;
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
                   EqualityComparer<CtiServer>.Default.Equals(ConnectionInfo, other.ConnectionInfo) &&
                   Channel == other.Channel;
        }

        public override int GetHashCode()
        {
            int hashCode = 379874733;
            hashCode = hashCode * -1521134295 + EqualityComparer<CtiServer>.Default.GetHashCode(ConnectionInfo);
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
        public override string ToString() =>
            $"<{GetType().Name} Connection={ConnectionInfo}, Channel={Channel}, EventType={EventType}, Type={Type}, ProcessId={ProcessId}, CallingNo={CallingNo}>";
    }
}
