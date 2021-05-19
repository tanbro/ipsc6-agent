using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AgentWpfApp.ViewModels
{
    class LoginViewModel : Utils.SingletonBase<LoginViewModel>
    {
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

        static bool LoginCanExecute(object _)
        {
            return true;
        }

        static public void LoginExecute(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            MessageBox.Show($"登录: WorkerNum={loginModel.WorkerNum}. Password is: {passwordBox.Password}");
            var window = loginWindow;
            window.DialogResult = true;
            window.Close();
        }
        #endregion
    }

}
