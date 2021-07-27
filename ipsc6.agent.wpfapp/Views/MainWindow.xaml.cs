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
using System.ComponentModel;

namespace ipsc6.agent.wpfapp.Views
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MainWindow));

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip notifyMenu;
        private System.Windows.Forms.ToolStripMenuItem menuItemExit;

        public MainWindow()
        {
            InitializeComponent();

            var app = Application.Current as App;
            if (!app.IsStartupOk)
            {
                Close();
                return;
            }

            var viewModel = ViewModels.MainViewModel.Instance;
            DataContext = viewModel;

        }

        /// 一些 WinForm 的 UI 初始化
        private void InitializeWinFormComponents()
        {
            menuItemExit = new("退出(&E)", null, MenuItemExit_Click);

            notifyMenu = new();
            notifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripMenuItem[]
            {
                menuItemExit
            });

            var notifyIconRes = Application.GetResourceStream(new Uri(@"pack://application:,,,/Icons/App.ico"));
            notifyIcon = new()
            {
                Icon = new(notifyIconRes.Stream),
                Text = Title,
                ContextMenuStrip = notifyMenu,
                Visible = true,
            };
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            var viewModel = DataContext as ViewModels.MainViewModel;
            viewModel.ShowMainWindow();
        }

        private void MenuItemExit_Click(object sender, EventArgs e)
        {
            var viewModel = DataContext as ViewModels.MainViewModel;
            viewModel.CloseMainWindow();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            notifyIcon?.Dispose();
            var app = Application.Current as App;
            if (app.IsStartupOk)
            {
                logger.Info("窗口已关闭");
                var viewModel = DataContext as ViewModels.MainViewModel;
                viewModel.Release();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            var app = Application.Current as App;
            if (app.IsStartupOk)
            {
                var viewModel = DataContext as ViewModels.MainViewModel;
                if (!viewModel.IsExiting)
                {
                    e.Cancel = true;
                    viewModel.HideMainWindow();
                }
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            var viewModel = DataContext as ViewModels.MainViewModel;
            viewModel.MouseEnter();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            var viewModel = DataContext as ViewModels.MainViewModel;
            viewModel.MouseLeave();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /// ViewModel 初始化
            var viewModel = ViewModels.MainViewModel.Instance;
            if (viewModel.Initial())
            {
                InitializeWinFormComponents();
            }
            else
            {
                viewModel.CloseMainWindow();
            }
            
        }
    }
}
