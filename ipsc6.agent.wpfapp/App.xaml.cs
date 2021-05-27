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
                logger.Debug("client.Agent.Initial()");
                client.Agent.Initial();
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
                    logger.Debug("client.Agent.Release()");
                    client.Agent.Release();
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
