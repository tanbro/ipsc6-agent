using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.IO;
using System.Threading;
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

        public static TaskScheduler TaskScheduler { get; private set; }
        public static TaskFactory TaskFactory { get; private set; }

        void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
            }
            catch (Exception err)
            {
                logger.ErrorFormat("日志配置加载失败: {0}\r\n^^^^^^^^^^^^^^^^^^^^ Shutdown ^^^^^^^^^^^^^^^^^^^^\r\n", err);
                MessageBox.Show(
                    $"应用程序日志配置加载失败，程序无法运行，即将退出。\r\n\r\n{err}",
                    "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                Shutdown(1);
                return;
            }

            logger.Warn("\r\n!!!!!!!!!!!!!!!!!!!! Startup !!!!!!!!!!!!!!!!!!!!\r\n");

            TaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            TaskFactory = new TaskFactory(TaskScheduler);

            try
            {
                Config.Manager.Initialize();
            }
            catch (Exception err)
            {
                Shutdown(2);
                logger.ErrorFormat("配置文件加载失败: {0}\r\n^^^^^^^^^^^^^^^^^^^^ Shutdown ^^^^^^^^^^^^^^^^^^^^\r\n", err);
                MessageBox.Show(
                    $"应用程序配置文件加载失败，程序无法运行，即将退出。\r\n\r\n{err}",
                    "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                return;
            }

            var jsonRpcWsCanceller = new CancellationTokenSource();

            try
            {
                logger.Debug("Initial()");
                services.Service.Initial();
                try
                {
                    var jsonRpcWs = new server.Server(() => new services.Service());
                    var jsonRpcWsTask = Task.Run(() => jsonRpcWs.RunAsync(jsonRpcWsCanceller.Token));
                    try
                    {
                        if (new Views.LoginWindow().ShowDialog() == true)
                        {
                            new Views.MainWindow().ShowDialog();
                        }
                    }
                    finally
                    {
                        jsonRpcWsCanceller.Cancel();
#pragma warning disable VSTHRD002
                        jsonRpcWsTask.Wait();
#pragma warning restore VSTHRD002
                        services.Service.DestroyAgent();
                    }
                }
                finally
                {
                    logger.Debug("Release()");
                    services.Service.Release();
                }

            }
            finally
            {
                Shutdown();
                logger.Warn("\r\n^^^^^^^^^^^^^^^^^^^^ Shutdown ^^^^^^^^^^^^^^^^^^^^\r\n");
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
