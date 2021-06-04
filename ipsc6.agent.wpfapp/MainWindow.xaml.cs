using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ipsc6.agent.wpfapp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModels.MainViewModel.Instance;
        }

        private void InfoPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        CancellationTokenSource snapCts = null;

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            var vm = DataContext as ViewModels.MainViewModel;
            if (Top <= 0)
            {
                // 准备吸附
                snapCts = new CancellationTokenSource();
                App.TaskFactory.StartNew(async () =>
                {
                    logger.Debug("开始吸附判断delay");
                    await Task.Delay(2500);
                    if (Top<=0)
                    {
                        /// 吸附！！！
                        logger.Debug("吸附！！！");
                        vm.Snapped = true;
                    }
                    snapCts.Cancel();
                    snapCts = null;
                }, snapCts.Token);
            }
            else
            {
                if (snapCts!=null)
                {
                    snapCts.Cancel();
                    snapCts = null;
                }
                logger.Debug("取消吸附！！！");
                vm.Snapped = false;
            }
        }
    }
}
