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

        private static Window GetOrCreateWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x is Views.LoginWindow);
            return window ?? new Views.LoginWindow();
        }

        public static async void DoLogin()
        {
            var dispatcher = Application.Current.Dispatcher;

            using (await Utils.CommandGuard.EnterAsync(loginCommand))
            {
                if (isLoginCompleted)
                {
                    throw new InvalidOperationException();
                }

                var window = GetOrCreateWindow() as Views.LoginWindow;
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                dispatcher.InvokeAsync(() =>
                {
                    window.ShowDialog();
                });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法

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

        /// 这个是专门给 RPC 使用的
        internal static async Task DoLoginAsync(string workerNum, string password, IEnumerable<string> serverList)
        {
            var dispatcher = Application.Current.Dispatcher;
            var svc = MainViewModel.Instance.MainService;

            using (await Utils.CommandGuard.EnterAsync(loginCommand))
            {
                if (isLoginCompleted)
                {
                    throw new InvalidOperationException();
                }

                Window window = null;
                dispatcher.Invoke(() =>
                {
                    window = GetOrCreateWindow();
                });

                LoginViewModel.workerNum = workerNum;
                LoginViewModel.password = password;

#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                dispatcher.InvokeAsync(() =>
                {
                    window.ShowDialog();
                });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法

                try
                {
                    await ExecuteLoginAsync(serverList);
                    isLoginCompleted = true;
                    dispatcher.Invoke(() =>
                    {
                        window.DialogResult = true;
                        window.Close();
                    });
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
                    else
                    {
#pragma warning disable CS4014
                        dispatcher.InvokeAsync(() =>
                        {
                            throw err;
                        });
#pragma warning restore CS4014
                    }
                }
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
            Instance.IsAllowInput = false;
            try
            {
                await svc.LogInAsync(workerNum, password, serverList);
                logger.Info("ExecuteLoginAsync - 登录成功!");
            }
            finally
            {
                Instance.IsAllowInput = true;
            }

            mainViewModel.StartTimer();
        }

    }

}

#pragma warning restore VSTHRD100
