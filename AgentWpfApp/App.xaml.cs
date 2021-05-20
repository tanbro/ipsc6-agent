using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using ipsc6.agent.network;

namespace AgentWpfApp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(App));

        void Application_Startup(object sender, StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));

            try
            {
                logger.Debug("Connector.Initial");
                Connector.Initial();
                try
                {
                    if (new LoginWindow().ShowDialog() == true)
                    {
                        logger.Debug("登录成功");
                        MessageBox.Show("登录成功");
                        new MainWindow().ShowDialog();
                    }
                    else
                    {
                        logger.Debug("登录失败");
                        MessageBox.Show("登录失败");
                    }
                }
                finally
                {
                    logger.Debug("Connector.Release");
                    Connector.Release();
                }
            }
            finally
            {
                logger.Debug("Shutdown");
                Shutdown();
            }
        }
    }
}
