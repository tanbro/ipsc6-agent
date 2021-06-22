using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(client.Call), typeof(string))]
    class AgentCallToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "<null>";
            var v = value as client.Call;
            //return $"[{ci.Channel}] ({ci.RemoteTelnum})=>({ci.LocalTelnum})";
            return v.ToString();

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
