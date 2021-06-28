using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

            try
            {
                Config.Manager.Initialize();
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
                logger.Debug("Initial()");

                var localRpcCreators = new LocalRpcTargetFunc[] {
                    (_, _) => mainService,
                    (_, _) => guiService,
                };
                var rpcServer = new server.Server(localRpcCreators);
                var rpcServerCanceller = new CancellationTokenSource();

                var rpcServerTask = Task.Run(() => rpcServer.RunAsync(rpcServerCanceller.Token));
                try
                {
                    _ = ViewModels.MainViewModel.Instance; // ensure lazy create
                    if (new Views.LoginWindow().ShowDialog() == true)
                    {
                        new Views.MainWindow().ShowDialog();
                    }
                }
                finally
                {
                    rpcServerCanceller.Cancel();
#pragma warning disable VSTHRD002
                    rpcServerTask.Wait();
#pragma warning restore VSTHRD002
                    mainService.Destroy();
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

        public static void Invoke(Action action)
        {
            if (Thread.CurrentThread == Current.Dispatcher.Thread)
            {
                action();
            }
            else
            {
                Current.Dispatcher.Invoke(action);
            }
        }

        public static async Task InvokeAsync(Task task)
        {
            var awaitable = task.ConfigureAwait(false);
            if (Thread.CurrentThread == Current.Dispatcher.Thread)
            {
                await awaitable;
            }
            else
            {
                var subTask = await Current.Dispatcher.InvokeAsync(async () => await awaitable);
                await subTask;
            }
        }

        public static async Task InvokeAsync(Func<Task> callback)
        {
            if (Thread.CurrentThread == Current.Dispatcher.Thread)
            {
                await callback().ConfigureAwait(false);
            }
            else
            {
                var task = await Current.Dispatcher.InvokeAsync(callback);
                await task;
            }
        }

        internal static services.Service mainService = new();
        internal static GuiService guiService = new();
    }
}
