using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(IList<client.CallInfo>), typeof(string))]
    class AgentCallsToHoldPanelTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var members = value as IList<client.CallInfo>;
            var v = (
                from m in members
                where m.IsHeld
                select 1
            ).Count();
            return v.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
