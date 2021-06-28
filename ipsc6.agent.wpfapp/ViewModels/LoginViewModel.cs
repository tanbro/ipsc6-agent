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
using System.Threading;
using System.Windows.Threading;

namespace ipsc6.agent.wpfapp.ViewModels
{
    public class LoginViewModel : Utils.SingletonObservableObject<LoginViewModel>
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
            if (loginSem.CurrentCount < 1) return false;
            if (string.IsNullOrEmpty(workerNum)) return false;
            if (passwordLength <= 0) return false;
            return true;
        }

        static readonly SemaphoreSlim loginSem = new(1);

        public static async Task DoLoginAsync(object parameter)
        {
            string password;
            if (parameter is PasswordBox)
            {
                password = (parameter as PasswordBox).Password;
            }
            else if (parameter is string)
            {
                password = parameter as string;
            }
            else
            {
                throw new InvalidCastException();
            }

            await App.InvokeAsync(async () =>
            {
                bool isOk = false;
                await loginSem.WaitAsync();
                try
                {
                    loginCommand.NotifyCanExecuteChanged();
                    var cfg = Config.Manager.ConfigurationRoot;
                    var options = new Config.Ipsc();
                    cfg.GetSection(nameof(Config.Ipsc)).Bind(options);
                    logger.InfoFormat(
                        "CreateAgent - ServerList: {0}, LocalPort: {1}, LocalAddress: \"{2}\"",
                        (options.ServerList == null) ? "<null>" : $"\"{string.Join(",", options.ServerList)}\"",
                        options.LocalPort,
                        options.LocalAddress
                    );
                    App.mainService.Create(options.ServerList, options.LocalPort, options.LocalAddress);
                    try
                    {
                        await App.mainService.LogInAsync(workerNum, password);
                        logger.InfoFormat("登录成功");
                        isOk = true;
                    }
                    catch (Exception err)
                    {
                        App.mainService.Destroy();
                        if (err is ConnectionException)
                        {
                            MessageBox.Show(
                                $"登录失败\r\n\r\n{err}",
                                Application.Current.MainWindow.Title,
                                MessageBoxButton.OK, MessageBoxImage.Error
                            );
                        }
                        else
                        {
                            throw;
                        }
                    }
                    if (isOk)
                    {
                        window.DialogResult = true;
                        window.Close();
                    }
                }
                finally
                {
                    loginSem.Release();
                }
                loginCommand.NotifyCanExecuteChanged();
            });
        }
        #endregion
    }

}
