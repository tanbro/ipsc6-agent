using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(IList<client.Call>), typeof(string))]
    class AgentCallsToNumberTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var members = value as IList<client.Call>;
            var totalCount = members.Count;
            var subCount = (
                from m in members
                where m.IsHeld
                select 1
            ).Count();
            return $"{subCount}/{totalCount}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
