using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;


namespace ipsc6.agent.wpfapp
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
                //var settings = wpfapp.Properties.Settings.Default;
                //logger.DebugFormat("settings.CtiServerAddress = {0}", settings.CtiServerAddress);
                //settings.CtiServerAddress = "192.168.2.108";
                //logger.DebugFormat("settings.CtiServerAddress = {0}", settings.CtiServerAddress);
                //settings.Save();

                logger.Debug("Connector.Initial");
                network.Connector.Initial();
                try
                {
                    if (new LoginWindow().ShowDialog() == true)
                    {
                        logger.Debug("登录成功");
                        new MainWindow().ShowDialog();
                    }
                    else
                    {
                        logger.Error("登录失败");
                    }
                }
                finally
                {
                    logger.Debug("Connector.Release");
                    network.Connector.Release();
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
