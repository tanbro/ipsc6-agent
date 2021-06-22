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
    class AgentTeleStateToBrushConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = (Color)ColorConverter.ConvertFromString("#808080");
            if (value != null)
            {
                var teleState = (client.TeleState)value;
                var agent = Controllers.AgentController.Agent;
                if (agent != null)
                {
                    if (agent.SipAccounts.Any(x => x.IsRegisterActive))  // 有注册了的
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
            }
            return new SolidColorBrush(color);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(client.TeleState), typeof(PackIconMaterialKind))]
    class AgentTeleStateToMaterialIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = PackIconMaterialKind.PhoneOff;
            if (value != null)
            {
                var teleState = (client.TeleState)value;
                var model = Models.Cti.AgentBasicInfo.Instance;
                switch (teleState)
                {
                    case client.TeleState.OffHook:
                        return PackIconMaterialKind.Phone;
                    case client.TeleState.OnHook:
                        if (model.SipAccountList.Any(x => x.IsRegisterActive))  // 有任何一个注册了的
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
