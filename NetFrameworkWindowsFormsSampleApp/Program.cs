using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ipsc6.agent.network;

namespace NetFrameworkWindowsFormsSampleApp
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Connector.Initial();
            try
            {
                Application.Run(new Form1());
            }
            finally
            {
                Connector.Release();
            }
        }
    }
}
