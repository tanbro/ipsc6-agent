using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using MahApps.Metro.IconPacks;


namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(bool), typeof(PackIconRemixIconKind))]
    public class BooleanToRemixPinIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool pinned = value != null && (bool)value;
            if (pinned)
                return PackIconRemixIconKind.Pushpin2Fill;
            else
                return PackIconRemixIconKind.PushpinFill;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
