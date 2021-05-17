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
    /// UserControl3.xaml 的交互逻辑
    /// </summary>
    public partial class UserControl3 : UserControl
    {
        public UserControl3()
        {
            InitializeComponent();
        }

        private int _queueCount = 0;
        /// <summary>
        /// 排队数
        /// </summary>
        public int QueueCount
        {
            set
            {
                _queueCount = value;
                txtQueueCount.Text = _queueCount.ToString();
            }
            get
            {
                return _queueCount;
            }
        }
        private int _lostCount = 0;
        /// <summary>
        /// 漏接数
        /// </summary>
        public int LostCount
        {
            set
            {
                _lostCount = value;
                txtLostCount.Text = _lostCount.ToString();
            }
            get
            {
                return _lostCount;
            }
        }

    }
}
