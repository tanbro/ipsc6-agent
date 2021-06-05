using System;
using System.Collections.Generic;
using System.Linq;

namespace ipsc6.agent.client
{
    /*
    // 发送弹屏信息
    // 振铃弹屏消息数据格式：
    //     总体格式，用|号分割：系统数据|用户自定义数据（排队节点传入的数据）
    //     系统数据，用;号分割：ProcessId;AgentSessionId;呼叫方向;用户电话号码;号码归属地;系统本地电话号码;坐席工号;排队类型;排队技能组ID;IVR路径
    //        注2：通道方向是CHSTATE_CALLIN(1) 时，用户电话号码为主叫号码，系统本地电话号码为被叫号码；通道方向是CHSTATE_CALLOUT(1) 时，用户电话号码为被叫号码，系统本地电话号码为主叫号码
    //     用户自定义数据：用|号分割的数据串
    //     完整格式：ProcessId;AgentSessionId;呼叫方向;用户电话号码;号码归属地;系统本地电话号码;坐席工号;排队类型;排队技能组ID;IVR路径|InValueStr(原来格式的排队数据)
    //     样例：10000156512848000;10000156512848003;0;18600001111;广州;10050;1001;1;Group1;;|ABCD|123
    */
    public class CallInfo : ServerSideData, IEquatable<CallInfo>
    {
        public int Channel { get; }
        public long ProcessId { get; }
        public long AgentSessionId { get; }
        public CallDirection CallDirection { get; }
        public string RemoteTelnum { get; }
        public string RemoteLocation { get; }
        public string LocalTelnum { get; }
        public string WorkerNum { get; }
        public QueueInfoType QueueType { get; }
        public string SkillGroupId { get; }
        public string IvrPath { get; }
        public string CustomString { get; }

        public bool IsHeld { get; internal set; }
        public HoldEventType HoldType { get; internal set; }
        public bool IsActive { get; internal set; }

        public CallInfo(ConnectionInfo connectionInfo, int channel, string data) : base(connectionInfo)
        {
            IsHeld = false;
            IsActive = false;
            Channel = channel;
            var parts = data.Split(Constants.VerticalBarDelimiter, 2);
            var values = parts[0].Split(Constants.SemicolonBarDelimiter);
            foreach (var pair in values.Select((s, i) => (s, i)))
            {
                var s = pair.s;
                var i = pair.i;
                switch (i)
                {
                    case 0:
                        ProcessId = long.Parse(s);
                        break;
                    case 1:
                        AgentSessionId = long.Parse(s);
                        break;
                    case 2:
                        CallDirection = (CallDirection)Enum.Parse(typeof(CallDirection), s);
                        break;
                    case 3:
                        RemoteTelnum = s;
                        break;
                    case 4:
                        RemoteLocation = s;
                        break;
                    case 5:
                        LocalTelnum = s;
                        break;
                    case 6:
                        WorkerNum = s;
                        break;
                    case 7:
                        QueueType = (QueueInfoType)Enum.Parse(typeof(QueueInfoType), s);
                        break;
                    case 8:
                        SkillGroupId = s;
                        break;
                    case 9:
                        IvrPath = s;
                        break;
                    default:
                        break;
                }
            }
            if (parts.Length > 1)
            {
                CustomString = parts[1];
            }
        }

        public override int GetHashCode()
        {
            int hashCode = 1152885954;
            hashCode = hashCode * -1521134295 + EqualityComparer<ConnectionInfo>.Default.GetHashCode(ConnectionInfo);
            hashCode = hashCode * -1521134295 + Channel.GetHashCode();
            return hashCode;
        }
        public override bool Equals(object obj) => Equals(obj as CallInfo);
        public bool Equals(CallInfo other) => other != null
            && EqualityComparer<ConnectionInfo>.Default.Equals(ConnectionInfo, other.ConnectionInfo)
            && Channel == other.Channel;
        public static bool operator ==(CallInfo left, CallInfo right) => EqualityComparer<CallInfo>.Default.Equals(left, right);
        public static bool operator !=(CallInfo left, CallInfo right) => !(left == right);
        public override string ToString() =>
            $"<{GetType().Name} Connection={ConnectionInfo}, Channel={Channel}, IsActive={IsActive}, IsHeld={IsHeld}, HoldType={HoldType}>";
    }
}
