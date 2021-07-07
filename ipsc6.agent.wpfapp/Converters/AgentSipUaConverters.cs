using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(IEnumerable<services.Models.SipAccount>), typeof(string))]
    internal class AgentSipAccountsToStringConverters : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "<null>";

            var sipAccounts = value as IEnumerable<services.Models.SipAccount>;

            // 有任何一个注册了，就算正常
            if (sipAccounts.Any(x => x.IsRegisterActive && x.LastRegisterError == 0))
            {
                return "已注册";
            }
            else
            {
                return "未注册";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
