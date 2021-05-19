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

using ipsc6.agent.client;

namespace AgentWpfApp
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            DataContext = ViewModels.LoginViewModel.Instance;
            ViewModels.LoginViewModel.Instance.LoginWindow = this;
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            string[] addresses = { "192.168.2.108" };
            if (G.agent == null)
            {
                G.agent = new Agent(addresses);
            }
            DialogResult = true;
        }

    }
}
