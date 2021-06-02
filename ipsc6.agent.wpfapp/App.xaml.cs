using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
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
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));

            try
            {
                Config.Manager.Initialize();
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    $"应用程序配置文件加载失败，程序无法运行，即将退出。\r\n\r\n{err}",
                    Current.MainWindow.Title,
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                Shutdown();
                return;
            }

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

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            logger.ErrorFormat("UnhandledException: {0}", e.Exception);
            MessageBox.Show(
                $"程序运行过程中出现了未捕获的异常。\r\n\r\n{e.Exception}",
                Current.MainWindow.Title,
                MessageBoxButton.OK, MessageBoxImage.Error
            );
        }
    }
}
