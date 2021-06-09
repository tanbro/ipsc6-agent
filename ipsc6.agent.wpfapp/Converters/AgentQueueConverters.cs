using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(IReadOnlyCollection<client.QueueInfo>), typeof(string))]
    public class QueueListToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "0";
            var v = value as IReadOnlyCollection<client.QueueInfo>;
            if (v.Count < 100) return $"{v.Count}";
            if (v.Count < 1000) return $"{v.Count / 100}00+";
            if (v.Count < 10000) return $"{v.Count / 1000}K+";
            return $"...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(client.QueueInfo), typeof(string))]
    public class QueueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var v = value as client.QueueInfo;
            return v.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
