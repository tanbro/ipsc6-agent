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
    [ValueConversion(typeof(client.CallDirection), typeof(string))]
    class AgentCallDirectionToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var v = (client.CallDirection)value;
            switch (v)
            {
                case client.CallDirection.In:
                    return "呼入";
                case client.CallDirection.Out:
                    return "呼出";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(client.CallDirection), typeof(PackIconMaterialKind))]
    class AgentCallDirectionToMaterialIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return PackIconMaterialKind.Null;
            var v = (client.CallDirection)value;
            switch (v)
            {
                case client.CallDirection.In:
                    return PackIconMaterialKind.PhoneIncoming;
                case client.CallDirection.Out:
                    return PackIconMaterialKind.PhoneOutgoing;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
