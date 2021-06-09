using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.IconPacks;


namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(bool), typeof(PackIconMaterialKind))]
    class ToggledBooleanToMaterialIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = value != null && (bool)value;
            if (v)
                return PackIconMaterialKind.ToggleSwitch;
            else
                return PackIconMaterialKind.ToggleSwitchOffOutline;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    class ToggledBooleanToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool v = value != null && (bool)value;
            Color color;
            if (v)
                color = Colors.Green;
            else
                color = Colors.Gray;
            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
