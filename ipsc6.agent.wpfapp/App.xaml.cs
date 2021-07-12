using ipsc6.agent.services;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;


using LocalRpcTargetFunc = System.Func<EmbedIO.WebSockets.WebSocketModule, EmbedIO.WebSockets.IWebSocketContext, object>;


namespace ipsc6.agent.wpfapp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(App));

        internal static Views.LoginWindow LoginWindow { get; private set; }

        internal static Service MainService { get; private set; }
        internal static GuiService GuiService { get; private set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));
                logger.Warn("\r\n!!!!!!!!!!!!!!!!!!!! Startup !!!!!!!!!!!!!!!!!!!!\r\n");
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

            config.Ipsc cfgIpsc;
            try
            {
                ConfigManager.Initialize();
                var cfgRoot = ConfigManager.ConfigurationRoot;
                cfgIpsc = new();
                cfgRoot.GetSection(nameof(config.Ipsc)).Bind(cfgIpsc);
                logger.InfoFormat("Config.Ipsc: {0}", cfgIpsc);
            }
            catch (Exception err)
            {
                logger.ErrorFormat("配置文件加载失败: {0}\r\n^^^^^^^^^^^^^^^^^^^^ Shutdown ^^^^^^^^^^^^^^^^^^^^\r\n", err);
                MessageBox.Show(
                    $"应用程序配置文件加载失败，程序无法运行，即将退出。\r\n\r\n{err}",
                    "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                Shutdown(2);
                return;
            }

            try
            {
                logger.Debug("create RPC Server");
                using (MainService = new(cfgIpsc))
                using (GuiService = new())
                {
                    LocalRpcTargetFunc[] localRpcCreators = {
                        (_, _) => MainService,
                        (_, _) => GuiService,
                    };
                    using CancellationTokenSource rpcServerCanceller = new();
                    server.Server rpcServer = new(localRpcCreators);
                    using var rpcServerRunningTask = Task.Run(() => rpcServer.RunAsync(rpcServerCanceller.Token));
                    try
                    {
                        logger.Debug("create MainViewModel");
                        _ = ViewModels.MainViewModel.Instance; // ensure lazy create
                        logger.Debug("show LoginWindow");
                        bool isLoginOk;
                        LoginWindow = new();
                        try
                        {
                            isLoginOk = LoginWindow.ShowDialog() == true;
                        }
                        finally
                        {
                            LoginWindow = null;
                        }
                        //if (isLoginOk)
                        {
                            logger.Debug("show MainWindow");
                            new Views.MainWindow().ShowDialog();
                        }
                    }
                    finally
                    {
                        logger.Debug("close RPC Server");
                        rpcServerCanceller.Cancel();
#pragma warning disable VSTHRD002
                        try
                        {
                            rpcServerRunningTask.Wait();
                        }
                        catch (OperationCanceledException) { }
#pragma warning restore VSTHRD002
                    }
                }

            }
            finally
            {
                logger.Debug("Shutdown app");
                Shutdown();
                logger.Warn("\r\n^^^^^^^^^^^^^^^^^^^^ Shutdown ^^^^^^^^^^^^^^^^^^^^\r\n");
            }
        }



        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            logger.ErrorFormat("Application UnhandledException: {0}", e.Exception);
            MessageBox.Show(
                $"程序运行过程中出现了未捕获的异常。\r\n\r\n{e.Exception}",
                Current.MainWindow.Title,
                MessageBoxButton.OK, MessageBoxImage.Error
            );
        }

    }
}
