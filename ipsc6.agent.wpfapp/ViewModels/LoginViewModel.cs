using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ipsc6.agent.client;


namespace ipsc6.agent.wpfapp.ViewModels
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
                var settings = Properties.Settings.Default;

                char[] separator = { ';' };
                var addresses = settings.CtiServerAddress.Split(separator);

                logger.InfoFormat("登录开始: {0}@{1}", workerNum, addresses);

                var agent = Enties.Cti.AgentController.CreateAgent(addresses);
                try
                {
                    await Enties.Cti.AgentController.StartupAgent(workerNum, password);
                    logger.InfoFormat("登录成功");
                    isOk = true;
                }
                catch (ConnectionException err)
                {
                    Enties.Cti.AgentController.DisposeAgent();
                    MessageBox.Show($"{err}", "登陆失败");
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
