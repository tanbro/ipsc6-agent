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

namespace ipsc6.agent.wpfapp.Dialogs
{
    /// <summary>
    /// PromptDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PromptDialog : Window
    {
        public PromptDialog() : base()
        {
            InitializeComponent();
        }

        public string InputText { get; internal set; }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var textbox = FindName("txtInput") as TextBox;
            InputText = textbox.Text;
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
