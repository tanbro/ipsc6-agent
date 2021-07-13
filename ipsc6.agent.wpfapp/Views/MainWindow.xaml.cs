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

namespace ipsc6.agent.wpfapp.Views
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

            var viewModel = ViewModels.MainViewModel.Instance;
            DataContext = viewModel;
            viewModel.Initial();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            var viewModel = DataContext as ViewModels.MainViewModel;
            viewModel.Release();
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
    }
}
