using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(IReadOnlyCollection<client.Group>), typeof(string))]
    public class AgentGroupsToNumberTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "0/0";
            var v = value as IReadOnlyCollection<client.Group>;
            return $"{v.Count(x => x.IsSigned)}/{v.Count}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
