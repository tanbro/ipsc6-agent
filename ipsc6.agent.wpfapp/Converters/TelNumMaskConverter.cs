using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(string), typeof(string))]
    internal class TelNumMaskConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var s = value as string;
            s = s.TrimStart(new char['+']);
            if (s.StartsWith("00"))
            {
                s = s.Substring(2);
            }
            if (s.Length >= 11)
            {
                return s.Substring(0, 4) + new string('*', s.Length - 7) + s.Substring(s.Length - 3);
            }
            if (s.Length >= 7)
            {
                return new string('*', s.Length - 3) + s.Substring(s.Length - 3);
            }
            return new string('*', 6);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
