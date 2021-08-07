using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ipsc6.agent.wpfapp.Converters
{
    using AgentStateWorkType = Tuple<client.AgentState, client.WorkType>;

    [ValueConversion(typeof(AgentStateWorkType), typeof(SolidColorBrush))]
    public class AgentStateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#808080"); //"不存在";

            do
            {
                if (value == null) break;

                var tuple = value as AgentStateWorkType;
                switch (tuple?.Item1)
                {
                    case client.AgentState.OffLine:
                        color = (Color)ColorConverter.ConvertFromString("#808080"); // "离线";
                        break;
                    case client.AgentState.OnLine:
                        color = (Color)ColorConverter.ConvertFromString("#c1d8f0"); // "在线";
                        break;
                    case client.AgentState.Block:
                        color = (Color)ColorConverter.ConvertFromString("#808080"); // "被阻塞";
                        break;
                    case client.AgentState.Pause:
                        {
                            bool isSet = false;
                            switch (tuple?.Item2)
                            {
                                case client.WorkType.PauseBusy:
                                    color = (Color)ColorConverter.ConvertFromString("#f50600");// "忙碌";
                                    isSet = true;
                                    break;
                                case client.WorkType.PauseLeave:
                                    color = (Color)ColorConverter.ConvertFromString("#f50600");// "离开";
                                    isSet = true;
                                    break;
                                case client.WorkType.PauseTyping:
                                    color = (Color)ColorConverter.ConvertFromString("#bc9100");// "文书";
                                    isSet = true;
                                    break;
                                case client.WorkType.PauseForce: break;// "强制暂停";
                                case client.WorkType.PauseDisconnect: break;// "连接断开";
                                case client.WorkType.PauseSnooze:
                                    color = (Color)ColorConverter.ConvertFromString("#3ea4d8");//turn "小休";
                                    isSet = true;
                                    break;
                                case client.WorkType.PauseDinner:
                                    color = (Color)ColorConverter.ConvertFromString("#bc9100");// "用餐";
                                    isSet = true;
                                    break;
                                case client.WorkType.PauseTrain:
                                    color = (Color)ColorConverter.ConvertFromString("#fbda57");// "培训";
                                    isSet = true;
                                    break;
                                default: break;
                            }
                            if (!isSet)
                            {
                                color = (Color)ColorConverter.ConvertFromString("#ba0300");
                            }
                        }
                        break;
                    case client.AgentState.Leave: break; // "离开(Deprecated)";
                    case client.AgentState.Idle:
                        color = (Color)ColorConverter.ConvertFromString("#578329"); //  "空闲";
                        break;
                    case client.AgentState.Ring:
                        color = (Color)ColorConverter.ConvertFromString("#f9c100"); //  "振铃";
                        break;
                    case client.AgentState.Work:
                        color = (Color)ColorConverter.ConvertFromString("#ba0300"); // "工作";
                        break;
                    case client.AgentState.WorkPause: break; // "工作暂停(Deprecated)";
                    default:
                        break;
                }

            } while (false);

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(AgentStateWorkType), typeof(SolidColorBrush))]
    public class AgentStateToBrushConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ViewModels.MainViewModel.IsMainConnectionOk)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
            }
            AgentStateToBrushConverter _conv = new();
            return _conv.Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
