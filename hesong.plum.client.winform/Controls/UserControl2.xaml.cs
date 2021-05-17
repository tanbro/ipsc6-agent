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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace hesong.plum.client.Controls
{
    /// <summary>
    /// UserControl2.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl2 : UserControl
    {
        public UserControl2()
        {
            InitializeComponent();
        }

        private string callOutNo = "";
        /// <summary>
        /// 外呼号码
        /// </summary>
        public string CallOutNo
        {
            get
            {
                return callOutNo;
            }
            set
            {
                callOutNo = value;
                txtCallOut.Text = callOutNo;
            }
        }

        private void butDialogPanel_Click(object sender, RoutedEventArgs e)
        {
            OnClick?.Invoke(sender, e);
        }

        public event System.EventHandler OnClick;
    }
}
