using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{

    [ValueConversion(typeof(IList<client.AgentGroup>), typeof(string))]
    public class AgentGroupsToNumberTitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var groups = value as IList<client.AgentGroup>;
            var totalCount = groups.Count;
            var signedCount = (
                from m in groups
                where m.Signed == true
                select 1
            ).Count();
            return $"{signedCount}/{totalCount}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
