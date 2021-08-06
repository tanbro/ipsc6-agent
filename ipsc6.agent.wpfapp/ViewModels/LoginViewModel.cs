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

        private static readonly IRelayCommand loginCommand = new AsyncRelayCommand<object>(DoLoginAsync, CanLogin);
        public IRelayCommand LoginCommand => loginCommand;

        private static Window GetOrCreateWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x is Views.LoginWindow);
            return window ?? new Views.LoginWindow();
        }

        private static Window GetSingleWindow()
        {
            var window = Application.Current.Windows.OfType<Window>().Single(x => x is Views.LoginWindow);
            return window;
        }

        public static async Task DoLoginAsync(object parameter)
        {
            IEnumerable<string> serverList = Array.Empty<string>();
            if (parameter != null)
            {
                var realParam = parameter as Tuple<string, string, IEnumerable<string>>;
                Instance.WorkerNum = realParam.Item1;
                password = realParam.Item2;
                serverList = realParam.Item3;
            }

            var dispatcher = Application.Current.Dispatcher;
            var svc = MainViewModel.Instance.MainService;

            using (await Utils.CommandGuard.EnterAsync(loginCommand))
            {
                var window = GetOrCreateWindow() as Views.LoginWindow;
#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                dispatcher.InvokeAsync(() =>
                {
                    try
                    {
                        window.ShowDialog();
                    }
                    catch (InvalidOperationException) { }
                });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法

                try
                {
                    await ExecuteLoginAsync(serverList);
                    window.DialogResult = true;
                    window.Close();
                }
                catch (client.ConnectionException err)
                {
                    logger.ErrorFormat("DoLogin - CTI服务器连接失败: {0}", err);
                    string errMsg = err switch
                    {
                        client.ConnectionFailedException =>
                            "无法连接到 CTI 服务器。\r\n请检查网络设置。",
                        client.ConnectionTimeoutException =>
                            "网络连接超时。\r\n请检查网络设置。",
                        client.ConnecttionLostException =>
                            "网络连接中断。\r\n请检查网络设置。",
                        client.ConnectionClosedException =>
                            "CTI 服务器主动关闭了连接请求。这通常是因为登录工号或密码错误。\r\n请输入正确的登录工号和密码。",
                        _ => err.ToString(),
                    };
                    MessageBox.Show(
                        Application.Current.MainWindow,
                        $"CTI 服务器连接失败\r\n\r\n{errMsg}",
                        $"{Application.Current.MainWindow.Title} - {window.Title}",
                        MessageBoxButton.OK, MessageBoxImage.Warning
                    );
                }
                catch (client.BaseRequestError err)
                {
                    logger.ErrorFormat("DoLogin - 登录失败: {0}", err);
                    string errMsg = err switch
                    {
                        client.ErrorResponse =>
                            $"CTI 服务器返回的登录失败原因: {err.Message}",
                        client.RequestTimeoutError =>
                            "CTI 服务器登录请求超时",
                        client.RequestNotCompleteError =>
                            "无法进行重复的登录请求",
                        _ => err.ToString(),
                    };
                    MessageBox.Show(
                        Application.Current.MainWindow,
                        $"登录失败\r\n\r\n{errMsg}",
                        $"{Application.Current.MainWindow.Title} - {window.Title}",
                        MessageBoxButton.OK, MessageBoxImage.Warning
                    );
                }
            }
        }

        internal static bool CanLogin(object _)
        {
            if (string.IsNullOrEmpty(workerNum) || string.IsNullOrEmpty(password))
                return false;
            if (Utils.CommandGuard.IsGuarding)
                return false;
            var svc = MainViewModel.Instance.MainService;
            if (svc.GetAgentRunningState() != client.AgentRunningState.Stopped)
                return false;
            return true;
        }

        private static async Task ExecuteLoginAsync(IEnumerable<string> serverList = null)
        {
            var svc = MainViewModel.Instance.MainService;
            var mainViewModel = MainViewModel.Instance;

            if (svc.GetAgentRunningState() != client.AgentRunningState.Stopped)
            {
                throw new InvalidOperationException($"座席状态为 {svc.GetAgentRunningState()} 时不允许进行登录");
            }

            serverList ??= (new string[] { });
            if (serverList.Count() == 0)
            {
                mainViewModel.ReloadConfigure();
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
                password = "";
                Instance.IsAllowInput = true;
            }

            mainViewModel.StartTimer();
        }

        private static readonly IRelayCommand closeCommand = new RelayCommand(DoClose);
        public IRelayCommand CloseCommand => closeCommand;

        private static void DoClose()
        {
            var window = GetSingleWindow();
            window.DialogResult = false;
            window.Close();
        }

        private static readonly IRelayCommand showConfigWindowCommand = new RelayCommand(DoShowConfigWindow);
        public IRelayCommand ShowConfigWindowCommand => showConfigWindowCommand;

        private static void DoShowConfigWindow()
        {
            new Views.ConfigWindow().ShowDialog();
        }

    }

}

#pragma warning restore VSTHRD100
