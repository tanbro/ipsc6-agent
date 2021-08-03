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
                        color = Colors.Yellow;
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
}
