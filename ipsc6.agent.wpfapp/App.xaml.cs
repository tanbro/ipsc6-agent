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
                logger.Debug("network.Connector.Initial()");
                network.Connector.Initial();
                try
                {
                    try
                    {
                        if (new LoginWindow().ShowDialog() == true)
                        {
                            new MainWindow().ShowDialog();
                        }
                    }
                    finally
                    {
                        Enties.Cti.AgentController.DisposeAgent();
                    }
                }
                finally
                {
                    logger.Debug("network.Connector.Release()");
                    network.Connector.Release();
                }

            }
            finally
            {
                logger.Debug("Shutdown()");
                Shutdown();
            }
        }
    }
}
