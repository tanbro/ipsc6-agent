using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ipsc6.agent.client;


namespace AgentWpfApp.ViewModels
{
    class LoginViewModel : Models.SingletonModelBase<LoginViewModel>
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoginViewModel));

        static LoginViewModel()
        {

        }

        private static LoginWindow window;
        public LoginWindow Window { set { window = value; } }

        static string workerNum;
        public string WorkerNum
        {
            get => workerNum;
            set => SetField(ref workerNum, value);
        }

        #region Login Command
        static readonly Utils.RelayCommand loginCommand = new Utils.RelayCommand(x => LoginExecute(x), x => LoginCanExecute(x));
        public ICommand LoginCommand => loginCommand;

        static bool _isLogging = false;

        static bool LoginCanExecute(object _)
        {
            if (_isLogging) return false;
            if (string.IsNullOrEmpty(workerNum)) return false;
            return true;
        }

        static public async void LoginExecute(object parameter)
        {
            _isLogging = true;
            try
            {
                var password = (parameter as PasswordBox).Password;

                bool isOk = false;
                var window = LoginViewModel.window;
                string[] addresses = { "192.168.2.107" };
                try
                {
                    if (G.agent == null)
                    {
                        logger.Debug("new Agent");
                        G.agent = new Agent(addresses);
                    }
                    logger.Debug("agent.StartUp ...");
                    await G.agent.StartUp(workerNum.Trim(), password);
                    isOk = true;
                }
                catch (ConnectionException err)
                {
                    if (G.agent.GetConnectionState(G.agent.MainConnectionIndex) == ConnectionState.Ok)
                    {
                        isOk = true;
                        logger.Debug("agent.StartUp 主服务节点连接成功");
                    }
                    else
                    {
                        logger.Error("agent.StartUp 失败. agent.Dispose");
                        G.agent.Dispose();
                        G.agent = null;
                        MessageBox.Show($"{err}", "登陆失败");
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
                _isLogging = false;
            }
        }
        #endregion
    }

}
