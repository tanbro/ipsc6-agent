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
    [ValueConversion(typeof(client.TeleState), typeof(SolidColorBrush))]
    internal class AgentTeleStateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#808080");
            var vm = ViewModels.MainViewModel.Instance;
            var sipAccounts = vm.SipAccounts;
            if (value != null && sipAccounts != null)
            {
                var teleState = (client.TeleState)value;
                if (sipAccounts.Any(x => x.IsRegisterActive && x.LastRegisterError == 0))  // 有注册了的
                {
                    switch (teleState)
                    {
                        case client.TeleState.OffHook:
                            color = (Color)ColorConverter.ConvertFromString("#3ea4d8");
                            break;
                        case client.TeleState.OnHook:
                            color = (Color)ColorConverter.ConvertFromString("#ba0300");
                            break;
                        default: break;
                    }
                }
            }
            return new SolidColorBrush(color);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(client.TeleState), typeof(PackIconMaterialKind))]
    internal class AgentTeleStateToMaterialIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = PackIconMaterialKind.PhoneOff;
            var vm = ViewModels.MainViewModel.Instance;
            var sipAccounts = vm.SipAccounts;
            if (value != null && sipAccounts != null)
            {
                var teleState = (client.TeleState)value;
                switch (teleState)
                {
                    case client.TeleState.OffHook:
                        return PackIconMaterialKind.Phone;
                    case client.TeleState.OnHook:
                        if (sipAccounts.Any(x => x.IsRegisterActive && x.LastRegisterError == 0))  // 有任何一个注册了的
                            return PackIconMaterialKind.PhoneHangup;
                        else
                            return PackIconMaterialKind.PhoneOff;
                    default: break;
                }
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
