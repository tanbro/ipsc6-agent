using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Extensions.Configuration;
using Microsoft.Toolkit.Mvvm.Input;


namespace ipsc6.agent.wpfapp.ViewModels
{
    public class LoginViewModel : Utils.SingletonObservableObject<LoginViewModel>
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoginViewModel));

        private static Views.LoginWindow window;
        public Views.LoginWindow Window { set => window = value; }

        private static string workerNumber;
        public string WorkerNumber
        {
            get => workerNumber;
            set
            {
                if (SetProperty(ref workerNumber, value))
                {
                    LoginCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private static string password;
        public string Password
        {
            get => password;
            set
            {
                if (SetProperty(ref password, value))
                {
                    LoginCommand.NotifyCanExecuteChanged();
                }
            }
        }

        #region Login Command
        private static readonly IAsyncRelayCommand loginCommand = new AsyncRelayCommand<object>(DoLoginAsync, CanLogin);
        public IAsyncRelayCommand LoginCommand => loginCommand;

        private static bool CanLogin(object _)
        {
            return loginSem.CurrentCount > 0 && !string.IsNullOrEmpty(workerNumber) && !string.IsNullOrEmpty(password);
        }

        private static readonly SemaphoreSlim loginSem = new(1);

        public static async Task DoLoginAsync(object parameter)
        {
            await loginSem.WaitAsync();
            try
            {
                string _password = parameter is string ? parameter as string : password;
                IConfigurationRoot cfgRoot = Config.Manager.ConfigurationRoot;
                Config.Ipsc cfgIpsc = new();
                cfgRoot.GetSection(nameof(Config.Ipsc)).Bind(cfgIpsc);
                logger.InfoFormat(
                    "CreateAgent - ServerList: {0}, LocalPort: {1}, LocalAddress: \"{2}\"",
                    (cfgIpsc.ServerList == null) ? "<null>" : $"\"{string.Join(",", cfgIpsc.ServerList)}\"",
                    cfgIpsc.LocalPort, cfgIpsc.LocalAddress
                );

                await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    loginCommand.NotifyCanExecuteChanged();
                    App.mainService.CreateAgent(cfgIpsc.ServerList, cfgIpsc.LocalPort, cfgIpsc.LocalAddress);
                    try
                    {
                        logger.Debug("开始登录 ...");
                        await App.mainService.LogInAsync(workerNumber, _password);
                        logger.Info("登录成功");
                        window.DialogResult = true;
                        window.Close();
                    }
                    catch (Exception err)
                    {
                        App.mainService.DestroyAgent();
                        if (err is client.ConnectionException)
                        {
                            logger.ErrorFormat("登录失败: {0}", err);
                            MessageBox.Show(
                                $"登录失败\r\n\r\n{err}",
                                Application.Current.MainWindow.Title,
                                MessageBoxButton.OK, MessageBoxImage.Error
                            );
                        }
                        else
                        {
                            logger.FatalFormat("登录期间发成了意料之外的错误: {0}", err);
                            throw;
                        }
                    }
                    finally
                    {
                        loginCommand.NotifyCanExecuteChanged();
                    }
                });
            }
            finally
            {
                loginSem.Release();
            }
        }
        #endregion
    }

}
