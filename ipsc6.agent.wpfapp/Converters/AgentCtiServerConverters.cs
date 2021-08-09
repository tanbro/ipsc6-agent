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
    [ValueConversion(typeof(IEnumerable<services.Models.CtiServer>), typeof(string))]
    internal class CtiServersConnectionStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "<null>";

            var servers = value as IEnumerable<services.Models.CtiServer>;

            try
            {
                // 全部连接上
                if (servers.All(x => x.State == client.ConnectionState.Ok))
                {
                    return "已连接";
                }
                // 部分连接上了
                if (servers.Any(x => x.State == client.ConnectionState.Ok))
                {
                    return "已连接";
                }
            }
            catch (ArgumentNullException) { }
            // 全部没有连接上
            return "未连接";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(IEnumerable<services.Models.CtiServer>), typeof(SolidColorBrush))]
    internal class CtiServersConnectionStateToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Colors.Gray;
            do
            {
                if (value == null) break;
                var servers = value as IEnumerable<services.Models.CtiServer>;

                try
                {
                    // 全部连接上
                    if (servers.All(x => x.State == client.ConnectionState.Ok))
                    {
                        color = Colors.Green;
                    }
                    // 部分连接上了
                    else if (servers.Any(x => x.State == client.ConnectionState.Ok))
                    {
                        color = Colors.Coral;
                    }
                    // 全部没有连接上
                    else
                    {
                        color = Colors.Red;
                    }
                }
                catch (ArgumentNullException) { }
            } while (false);
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(client.ConnectionState), typeof(string))]
    internal class CtiServerConnectionStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "N/A";
            try
            {
                do
                {
                    if (value == null) break;
                    var obj = (client.ConnectionState)value;
                    switch (obj)
                    {
                        case client.ConnectionState.Init:
                            result = "初始化 ...";
                            break;
                        case client.ConnectionState.Opening:
                            result = "连接 ...";
                            break;
                        case client.ConnectionState.Failed:
                            result = "连接失败";
                            break;
                        case client.ConnectionState.Ok:
                            result = "已连接";
                            break;
                        case client.ConnectionState.Closing:
                            result = "关闭 ...";
                            break;
                        case client.ConnectionState.Closed:
                            result = "已关闭";
                            break;
                        case client.ConnectionState.Lost:
                            result = "已断开";
                            break;
                        default:
                            break;
                    }
                } while (false);
            }
            catch (ArgumentNullException) { }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(client.ConnectionState), typeof(SolidColorBrush))]
    internal class CtiServerConnectionStateToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color result = Colors.Gray;
            try
            {
                do
                {
                    if (value == null) break;
                    var obj = (client.ConnectionState)value;

                    switch (obj)
                    {
                        case client.ConnectionState.Init:
                            result = Colors.LightGray;
                            break;
                        case client.ConnectionState.Opening:
                            result = Colors.DarkBlue;
                            break;
                        case client.ConnectionState.Failed:
                            result = Colors.Red;
                            break;
                        case client.ConnectionState.Ok:
                            result = Colors.Green;
                            break;
                        case client.ConnectionState.Closing:
                            result = Colors.DarkSlateBlue;
                            break;
                        case client.ConnectionState.Closed:
                            result = Colors.DarkRed;
                            break;
                        case client.ConnectionState.Lost:
                            result = Colors.Red;
                            break;
                        default:
                            break;
                    }
                } while (false);
            }
            catch (ArgumentNullException) { }
            return new SolidColorBrush(result);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
