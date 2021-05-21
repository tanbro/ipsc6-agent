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
    class LoginViewModel : Utils.SingletonModelBase<LoginViewModel>
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(LoginViewModel));

        public LoginViewModel()
        {
            loginCommand = new Utils.RelayCommand(x => DoLogin(x), x => CanLogin(x));
        }

        private LoginWindow window;
        public LoginWindow Window { set { window = value; } }

        static string workerNum;
        public string WorkerNum
        {
            get => workerNum;
            set => SetField(ref workerNum, value);
        }

        int passwordLength;
        public int PasswordLength
        {
            get => passwordLength;
            set
            {
                passwordLength = value;
            }
        }

        #region Login Command
        readonly Utils.RelayCommand loginCommand;
        public ICommand LoginCommand => loginCommand;

        bool _isLogging = false;

        bool CanLogin(object _)
        {
            if (string.IsNullOrEmpty(workerNum)) return false;
            if (passwordLength <= 0) return false;
            if (_isLogging) return false;
            return true;
        }

        public async void DoLogin(object parameter)
        {
            _isLogging = true;
            try
            {
                var password = (parameter as PasswordBox).Password;

                bool isOk = false;
                MessageBox.Show("Mock: 登录开始：Delay(1000)");
                await Task.Delay(1000);
                MessageBox.Show("Mock: 登录成功");
                isOk = true;

                //string[] addresses = { "192.168.2.108" };
                //try
                //{
                //    if (G.agent == null)
                //    {
                //        logger.Debug("new Agent");
                //        G.agent = new Agent(addresses);
                //    }
                //    logger.Debug("agent.StartUp ...");
                //    await G.agent.StartUp(workerNum.Trim(), password);
                //    isOk = true;
                //}
                //catch (ConnectionException err)
                //{
                //    if (G.agent.GetConnectionState(G.agent.MainConnectionIndex) == ConnectionState.Ok)
                //    {
                //        isOk = true;
                //        logger.Debug("agent.StartUp 主服务节点连接成功");
                //    }
                //    else
                //    {
                //        logger.Error("agent.StartUp 失败. agent.Dispose");
                //        G.agent.Dispose();
                //        G.agent = null;
                //        MessageBox.Show($"{err}", "登陆失败");
                //    }
                //}
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
