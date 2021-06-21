using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Extensions.Configuration;

using Microsoft.Toolkit.Mvvm.Input;

using ipsc6.agent.client;

namespace ipsc6.agent.wpfapp.ViewModels
{
    class LoginViewModel : Utils.SingletonObservableObject<LoginViewModel>
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoginViewModel));

        static Views.LoginWindow window;
        public Views.LoginWindow Window { set => window = value; }

        static string workerNum;
        public string WorkerNum
        {
            get => workerNum;
            set => SetProperty(ref workerNum, value);
        }

        static int passwordLength;
        public int PasswordLength
        {
            get => passwordLength;
            set
            {
                passwordLength = value;
                LoginCommand.NotifyCanExecuteChanged();
            }
        }

        #region Login Command
        readonly static IRelayCommand loginCommand = new AsyncRelayCommand<object>(DoLoginAsync, CanLogin);
        public IRelayCommand LoginCommand => loginCommand;

        static bool CanLogin(object _)
        {
            if (string.IsNullOrEmpty(workerNum)) return false;
            if (passwordLength <= 0) return false;
            if (_isLogging) return false;
            return true;
        }

        static bool _isLogging = false;

        public static async Task DoLoginAsync(object parameter)
        {
            _isLogging = true;
            try
            {
                loginCommand.NotifyCanExecuteChanged();

                var password = (parameter as PasswordBox).Password;
                bool isOk = false;

                var cfg = Config.Manager.ConfigurationRoot;
                var options = new Config.Ipsc();
                cfg.GetSection(nameof(Config.Ipsc)).Bind(options);
                logger.InfoFormat(
                    "CreateAgent - ServerList: {0}, LocalPort: {1}, LocalAddress: \"{2}\"",
                    (options.ServerList == null) ? "<null>" : $"\"{string.Join(",", options.ServerList)}\"",
                    options.LocalPort,
                    options.LocalAddress
                );
                services.Service.CreateAgent(options.ServerList, options.LocalPort, options.LocalAddress);
                try
                {
                    await services.Service.LogInAsync(workerNum, password);
                    logger.InfoFormat("登录成功");
                    isOk = true;
                }
                catch (ConnectionException err)
                {
                    services.Service.DestroyAgent();
                    MessageBox.Show(
                        $"登录失败\r\n\r\n{err}",
                        Application.Current.MainWindow.Title,
                        MessageBoxButton.OK, MessageBoxImage.Error
                    );
                }
                if (isOk)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            finally
            {
                _isLogging = false;
                loginCommand.NotifyCanExecuteChanged();
            }
        }
        #endregion
    }

}
