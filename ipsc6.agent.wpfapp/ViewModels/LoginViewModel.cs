using System;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Extensions.Configuration;
using Microsoft.Toolkit.Mvvm.Input;

#pragma warning disable VSTHRD100

namespace ipsc6.agent.wpfapp.ViewModels
{
    public class LoginViewModel : Utils.SingletonObservableObject<LoginViewModel>
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoginViewModel));

        private static Views.LoginWindow window;
        public Views.LoginWindow Window { set => window = value; }

        private static string workerNum;
        public string WorkerNum
        {
            get => workerNum;
            set
            {
                if (SetProperty(ref workerNum, value))
                {
                    Application.Current.Dispatcher.Invoke(LoginCommand.NotifyCanExecuteChanged);
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
                    Application.Current.Dispatcher.Invoke(LoginCommand.NotifyCanExecuteChanged);
                }
            }
        }

        private static readonly IRelayCommand loginCommand = new RelayCommand(DoLogin, CanLogin);
        public IRelayCommand LoginCommand => loginCommand;

        public static async void DoLogin()
        {
            var svc = App.mainService;

            using (await Utils.CommandGuard.CreateAsync(loginCommand))
            {
                try
                {
                    await ExecuteLoginAsync();
                    window.DialogResult = true;
                    window.Close();
                }
                catch (Exception err)
                {
                    svc.DestroyAgent();
                    if (err is client.ConnectionException)
                    {
                        logger.ErrorFormat("DoLogin - 登录失败: {0}", err);
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
            }
        }

        internal static async Task DoLoginAsync(string workerNum, string password)
        {
            var dispatcher = Application.Current.Dispatcher;
            var svc = App.mainService;

            using (await Utils.CommandGuard.CreateAsync(loginCommand))
            {
                LoginViewModel.workerNum = workerNum;
                LoginViewModel.password = password;

                await dispatcher.Invoke(async () =>
                {
                    try
                    {
                        await ExecuteLoginAsync();
                        window.DialogResult = true;
                        window.Close();
                    }
                    catch (Exception err)
                    {
                        svc.DestroyAgent();
                        logger.ErrorFormat("DoLoginAsync - 登录失败: {0}", err);
                        if (err is client.ConnectionException)
                        {
#pragma warning disable CS4014
                            dispatcher.InvokeAsync(() =>
                            {
                                MessageBox.Show(
                                    $"登录失败\r\n\r\n{err}",
                                    Application.Current.MainWindow.Title,
                                    MessageBoxButton.OK, MessageBoxImage.Error
                                );
                            });
#pragma warning restore CS4014

                        }
                        throw;
                    }
                });
            }

        }

        private static bool CanLogin()
        {
            if (string.IsNullOrEmpty(workerNum) || string.IsNullOrEmpty(password))
                return false;
            if (Utils.CommandGuard.IsGuarding)
                return false;
            return true;
        }

        private static void CreateAgentInstance()
        {
            var svc = App.mainService;
            IConfigurationRoot cfgRoot = Config.Manager.ConfigurationRoot;
            Config.Ipsc cfgIpsc = new();
            cfgRoot.GetSection(nameof(Config.Ipsc)).Bind(cfgIpsc);
            logger.InfoFormat(
                "CreateAgentInstance - ServerList: {0}, LocalPort: {1}, LocalAddress: \"{2}\"",
                (cfgIpsc.ServerList == null) ? "<null>" : $"\"{string.Join(",", cfgIpsc.ServerList)}\"",
                cfgIpsc.LocalPort,
                cfgIpsc.LocalAddress);
            svc.CreateAgent(cfgIpsc.ServerList, cfgIpsc.LocalPort, cfgIpsc.LocalAddress);
        }

        private static async Task ExecuteLoginAsync()
        {
            var svc = App.mainService;
            CreateAgentInstance();
            logger.Debug("ExecuteLoginAsync - 开始登录 ...");
            await svc.LogInAsync(workerNum, password);
            logger.Info("ExecuteLoginAsync - 登录成功");
        }

    }

}

#pragma warning restore VSTHRD100
