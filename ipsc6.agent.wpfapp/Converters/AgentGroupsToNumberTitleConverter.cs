using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(IReadOnlyCollection<services.Models.Group>), typeof(string))]
    public class AgentGroupsToNumberTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = "0/0";
            do
            {
                if (value == null) break;
                try
                {
                    var v = value as IReadOnlyCollection<services.Models.Group>;
                    result = $"{v.Count(x => x.IsSigned)}/{v.Count}";
                }
                catch (ArgumentNullException) { }
            } while (false);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
