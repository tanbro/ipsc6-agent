using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace ipsc6.agent.wpfapp.Views
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = ViewModels.LoginViewModel.Instance;
            ViewModels.LoginViewModel.Instance.Window = this;
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ViewModels.LoginViewModel;
            vm.Password = (sender as PasswordBox).Password;
        }
    }
}
