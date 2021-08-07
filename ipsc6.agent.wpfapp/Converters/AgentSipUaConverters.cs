using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;


namespace ipsc6.agent.wpfapp.Converters
{
    [ValueConversion(typeof(IEnumerable<services.Models.SipAccount>), typeof(string))]
    internal class AgentSipAccountsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "N/A";

            var sipAccounts = value as IEnumerable<services.Models.SipAccount>;
            try
            {
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
            catch (ArgumentNullException) { }
            return "N/A";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(IEnumerable<services.Models.SipAccount>), typeof(SolidColorBrush))]
    internal class AgentSipAccountsToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Colors.Gray;
            do
            {
                if (value == null) break;
                var sipAccounts = value as IEnumerable<services.Models.SipAccount>;
                try
                {
                    // 全部注册了，算正常
                    if (sipAccounts.All(x => x.IsRegisterActive && x.LastRegisterError == 0) && sipAccounts.Count() > 0)
                    {
                        color = Colors.Green;
                    }
                    // 有任何一个注册了，也算正常
                    else if (sipAccounts.Any(x => x.IsRegisterActive && x.LastRegisterError == 0))
                    {
                        color = Colors.Yellow;
                    }
                    else
                    {
                        color = Colors.Red;
                    }
                }
                catch (ArgumentNullException) { }
            } while (false);

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
