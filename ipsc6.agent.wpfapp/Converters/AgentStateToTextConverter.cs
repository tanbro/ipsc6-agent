using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


namespace ipsc6.agent.wpfapp.Converters
{
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;

    [ValueConversion(typeof(AgentStateWorkType), typeof(string))]
    public class AgentStateToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var tuple = value as AgentStateWorkType;
            switch (tuple?.Item1)
            {
                case client.AgentState.NotExist: return "不存在";
                case client.AgentState.OffLine: return "离线";
                case client.AgentState.OnLine: return "在线";
                case client.AgentState.Block: return "被阻塞";
                case client.AgentState.Pause:
                    switch (tuple?.Item2)
                    {
                        case client.WorkType.PauseBusy: return "忙碌";
                        case client.WorkType.PauseLeave: return "离开";
                        case client.WorkType.PauseTyping: return "文书";
                        case client.WorkType.PauseForce: return "强制暂停";
                        case client.WorkType.PauseDisconnect: return "连接断开";
                        case client.WorkType.PauseSnooze: return "小休";
                        case client.WorkType.PauseDinner: return "用餐";
                        case client.WorkType.PauseTrain: return "培训";
                        default: break;
                    }
                    return "小休";
                case client.AgentState.Leave: return "离开(Deprecated)";
                case client.AgentState.Idle: return "空闲";
                case client.AgentState.Ring: return "振铃";
                case client.AgentState.Work:
                    switch (tuple?.Item2)
                    {
                        case client.WorkType.CallIn: return "呼入";
                        case client.WorkType.Transfer: return "转移";
                        case client.WorkType.SysCall: return "系统呼叫";
                        case client.WorkType.HandCall: return "手动呼叫";
                        case client.WorkType.Consult: return "咨询";
                        case client.WorkType.Flow: return "IVR";
                        case client.WorkType.ForceInsert: return "插话";
                        case client.WorkType.Listen: return "监听";
                        case client.WorkType.Hold: return "保持";
                        default: break;
                    }
                    return "工作";
                case client.AgentState.WorkPause: return "工作暂停(Deprecated)";
                default: break;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
