using System;
using System.Collections.Generic;
using System.Linq;

namespace ipsc6.agent.client
{
    /**
     21-07-06
     发送弹屏信息
     振铃弹屏消息数据格式：
         总体格式，用|号分割：系统数据|用户自定义数据（排队节点传入的数据）
         系统数据，用;号分割：(0)ProcessId;(1)AgentSessionId;(2)呼叫方向;(3)用户电话号码;(4)号码归属地;(5)24小时来电次数;(6)48小时来电次数;(7)系统本地电话号码;(8)坐席工号;(9)当天通话次数;(10)当天通话时长（秒）;(11)排队类型;(12)排队技能组ID;(13)IVR路径
            注1：AgentSessionId 和 ProcessId 参数是在agentmgr模块里添加进来的，本模块不体现。
            注2：通道方向是CHSTATE_CALLIN 时，用户电话号码为主叫号码，系统本地电话号码为被叫号码；通道方向是CHSTATE_CALLOUT 时，用户电话号码为被叫号码，系统本地电话号码为主叫号码
         用户自定义数据：用|号分割的数据串
         完整格式：ProcessId;AgentSessionId;呼叫方向;用户电话号码;号码归属地;24小时来电次数;48小时来电次数;系统本地电话号码;坐席工号;当天通话次数;当天通话时长;排队类型;排队技能组ID;IVR路径|InValueStr(原来格式的排队数据)
         样例：10000156512848000;10000156512848003;0;18600001111;广州;1;2;10050;1001;16;450;1;Group1;;|ABCD|123

     21-05-23
     发送弹屏信息
     振铃弹屏消息数据格式：
         总体格式，用|号分割：系统数据|用户自定义数据（排队节点传入的数据）
         系统数据，用;号分割：ProcessId;AgentSessionId;呼叫方向;用户电话号码;号码归属地;系统本地电话号码;坐席工号;排队类型;排队技能组ID;IVR路径
            注2：通道方向是CHSTATE_CALLIN(1) 时，用户电话号码为主叫号码，系统本地电话号码为被叫号码；通道方向是CHSTATE_CALLOUT(1) 时，用户电话号码为被叫号码，系统本地电话号码为主叫号码
         用户自定义数据：用|号分割的数据串
         完整格式：ProcessId;AgentSessionId;呼叫方向;用户电话号码;号码归属地;系统本地电话号码;坐席工号;排队类型;排队技能组ID;IVR路径|InValueStr(原来格式的排队数据)
         样例：10000156512848000;10000156512848003;0;18600001111;广州;10050;1001;1;Group1;;|ABCD|123
    **/

    public class CallInfo : ServerSideData, IEquatable<CallInfo>
    {
        public int Channel { get; }
        public long ProcessId { get; }
        public long AgentSessionId { get; }
        public CallDirection CallDirection { get; }
        public string RemoteTelNum { get; }
        public string RemoteLocation { get; }
        public string LocalTelNum { get; }
        public int H24Count { get; }
        public int H48Count { get; }
        public string WorkerNum { get; }
        public QueueInfoType QueueType { get; }
        public string SkillGroupId { get; }
        public string IvrPath { get; }
        public string CustomString { get; }

        public bool IsHeld { get; internal set; }
        public HoldEventType HoldType { get; internal set; }

        /// TODO: 座席的呼叫累计数据？？？

        public CallInfo(CtiServer ctiServer, int channel, string dataString) : base(ctiServer)
        {
            Channel = channel;
            IsHeld = false;
            var parts = dataString.Split(Constants.VerticalBarDelimiter, 2);
            var values = parts[0].Split(Constants.SemicolonBarDelimiter);
            foreach (var pair in values.Select((s, i) => (s, i)))
            {
                var s = pair.s;
                var i = pair.i;
                switch (i)
                {
                    case 0:
#pragma warning disable CA1305
                        ProcessId = long.Parse(s);
#pragma warning restore CA1305
                        break;
                    case 1:
#pragma warning disable CA1305
                        AgentSessionId = long.Parse(s);
#pragma warning restore CA1305
                        break;
                    case 2:
                        CallDirection = (CallDirection)Enum.Parse(typeof(CallDirection), s);
                        break;
                    case 3:
                        RemoteTelNum = s;
                        break;
                    case 4:
                        RemoteLocation = s;
                        break;
                    case 5:
#pragma warning disable CA1305
                        H24Count = int.Parse(s);
#pragma warning restore CA1305
                        break;
                    case 6:
#pragma warning disable CA1305
                        H48Count = int.Parse(s);
#pragma warning restore CA1305
                        break;
                    case 7:
                        LocalTelNum = s;
                        break;
                    case 8:
                        WorkerNum = s;
                        break;
                    case 9:
                        /// 座席当天通话数量
                        break;
                    case 10:
                        /// 座席当天通话秒
                        break;
                    case 11:
                        QueueType = (QueueInfoType)Enum.Parse(typeof(QueueInfoType), s);
                        break;
                    case 12:
                        SkillGroupId = s;
                        break;
                    case 13:
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
            hashCode = hashCode * -1521134295 + EqualityComparer<CtiServer>.Default.GetHashCode(CtiServer);
            hashCode = hashCode * -1521134295 + Channel.GetHashCode();
            return hashCode;
        }
        public override bool Equals(object obj) => Equals(obj as CallInfo);
        public bool Equals(CallInfo other) => other != null
            && EqualityComparer<CtiServer>.Default.Equals(CtiServer, other.CtiServer)
            && Channel == other.Channel;
        public static bool operator ==(CallInfo left, CallInfo right) => EqualityComparer<CallInfo>.Default.Equals(left, right);
        public static bool operator !=(CallInfo left, CallInfo right) => !(left == right);
        public override string ToString() =>
            $"<{GetType().Name} Connection={CtiServer}, Channel={Channel}, IsHeld={IsHeld}, HoldType={HoldType}, CallDirection={CallDirection}, RemoteTelNum={RemoteTelNum}>";
    }
}
