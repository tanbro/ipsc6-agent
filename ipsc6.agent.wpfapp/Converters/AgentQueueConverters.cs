using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(IList<client.QueueInfo>), typeof(string))]
    public class QueueListToTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "0";
            var v = value as IList<client.QueueInfo>;
            if (v.Count < 100) return $"{v.Count}";
            if (v.Count < 200) return "100+";
            if (v.Count < 300) return "200+";
            if (v.Count < 400) return "300+";
            if (v.Count < 500) return "400+";
            if (v.Count < 600) return "500+";
            if (v.Count < 700) return "600+";
            if (v.Count < 800) return "700+";
            if (v.Count < 900) return "800+";
            if (v.Count < 1000) return "900+";
            return "1K+";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
