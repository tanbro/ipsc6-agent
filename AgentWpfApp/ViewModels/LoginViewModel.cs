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
    class LoginViewModel : Utils.SingletonBase<LoginViewModel>
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoginViewModel));

        static LoginViewModel()
        {
            loginModel.WorkerNum = "1000";
        }

        private static Login loginWindow;
        public Login LoginWindow { set { loginWindow = value; } }

        static readonly Models.LoginModel loginModel = Models.LoginModel.Instance;
        public Models.LoginModel LoginModel => loginModel;

        #region Login Command
        static readonly Utils.RelayCommand loginCommand = new Utils.RelayCommand(x => LoginExecute(x), x => LoginCanExecute(x));
        public ICommand LoginCommand => loginCommand;

        static bool _isLogging = false;

        static bool LoginCanExecute(object _)
        {
            if (_isLogging) return false;
            if (string.IsNullOrEmpty(loginModel.WorkerNum)) return false;
            return true;
        }

        static public async void LoginExecute(object parameter)
        {
            _isLogging = true;
            try
            {
                var workerNum = loginModel.WorkerNum.Trim();
                var password = (parameter as PasswordBox).Password;

                bool isOk = false;
                var window = loginWindow;
                string[] addresses = { "192.168.2.108" };
                try
                {
                    if (G.agent == null)
                    {
                        logger.Debug("new Agent");
                        G.agent = new Agent(addresses);
                    }
                    logger.Debug("agent.StartUp ...");
                    await G.agent.StartUp(workerNum, password);
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
                        logger.Error("agent.StartUp 失败");
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
