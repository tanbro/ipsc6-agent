using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(IReadOnlyCollection<services.Models.QueueInfo>), typeof(string))]
    public class QueueListToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result;
            try
            {
                var v = value as IReadOnlyCollection<services.Models.QueueInfo>;
                switch (v.Count)
                {
                    case < 100:
                        result = $"{v.Count}";
                        break;
                    case < 1000:
                        result = $"{v.Count / 100}00+";
                        break;
                    case < 10000:
                        result = $"{v.Count / 1000}K+";
                        break;
                    default:
                        result = $"...";
                        break;
                }
            }
            catch (NullReferenceException)
            {
                result = "NullReferenceException";
            }
            catch (InvalidCastException)
            {
                result = "InvalidCastException";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(services.Models.QueueInfo), typeof(string))]
    public class QueueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result;
            try
            {
                var v = (services.Models.QueueInfo)value;
                result = $"{v.CallingTelNum}";
            }
            catch (NullReferenceException)
            {
                result = "NullReferenceException";
            }
            catch (InvalidCastException)
            {
                result = "InvalidCastException";
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
