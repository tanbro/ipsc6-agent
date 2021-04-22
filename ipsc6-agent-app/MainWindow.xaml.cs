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

namespace ipsc6_agent_app
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<string, string> MyDict;
        public MainWindow()
        {
            InitializeComponent();

            //
            MyDict = new();
            MyDict["key1"] = "This is value1";
            MyDict["key2"] = "This is value2";
            DataContext = MyDict;
        }
    }
}
