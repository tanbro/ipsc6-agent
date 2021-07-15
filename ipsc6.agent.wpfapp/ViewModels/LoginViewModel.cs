using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Microsoft.Toolkit.Mvvm.Input;

#pragma warning disable VSTHRD100

namespace ipsc6.agent.wpfapp.ViewModels
{
    public class LoginViewModel : Utils.SingletonObservableObject<LoginViewModel>
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoginViewModel));

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

        private static bool isAllowInput = true;
        public bool IsAllowInput
        {
            get => isAllowInput;
            set => SetProperty(ref isAllowInput, value);
        }

        private static readonly IRelayCommand loginCommand = new RelayCommand(DoLogin, CanLogin);
        public IRelayCommand LoginCommand => loginCommand;

        private static bool isLoginCompleted;

        public static async void DoLogin()
        {
            if (isLoginCompleted)
            {
                throw new InvalidOperationException();
            }

            Instance.IsAllowInput = false;
            try
            {
                using (await Utils.CommandGuard.EnterAsync(loginCommand))
                {
                    var window = MainViewModel.Instance.LoginWindow;
                    try
                    {
                        await ExecuteLoginAsync();
                        isLoginCompleted = true;
                        window.DialogResult = true;
                        window.Close();
                    }
                    catch (Exception err)
                    {
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
            finally
            {
                Instance.IsAllowInput = true;
            }
        }

        internal static async Task DoLoginAsync(string workerNum, string password, IEnumerable<string> serverList)
        {
            var dispatcher = Application.Current.Dispatcher;
            var svc = MainViewModel.Instance.MainService;

            using (await Utils.CommandGuard.EnterAsync(loginCommand))
            {
                LoginViewModel.workerNum = workerNum;
                LoginViewModel.password = password;

                await dispatcher.Invoke(async () =>
                {
                    var window = MainViewModel.Instance.LoginWindow;
                    try
                    {
                        if (isLoginCompleted)
                        {
                            throw new InvalidOperationException();
                        }
                        await ExecuteLoginAsync(serverList);
                        isLoginCompleted = true;
                        window.DialogResult = true;
                        window.Close();
                    }
                    catch (Exception err)
                    {
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

        internal static bool CanLogin()
        {
            if (string.IsNullOrEmpty(workerNum) || string.IsNullOrEmpty(password))
                return false;
            if (Utils.CommandGuard.IsGuarding)
                return false;
            return true;
        }

        private static async Task ExecuteLoginAsync(IEnumerable<string> serverList = null)
        {
            var svc = MainViewModel.Instance.MainService;
            var mainViewModel = MainViewModel.Instance;
            if (serverList is null)
            {
                serverList = mainViewModel.cfgIpsc.ServerList;
            }
            else if (serverList.Count() == 0)
            {
                serverList = mainViewModel.cfgIpsc.ServerList;
            }
            logger.Info("ExecuteLoginAsync - 开始登录...");
            await svc.LogInAsync(workerNum, password, serverList);
            logger.Info("ExecuteLoginAsync - 登录成功!");
        }

    }

}

#pragma warning restore VSTHRD100
