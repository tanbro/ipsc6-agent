using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;


namespace ipsc6.agent.wpfapp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(App));

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        internal Assembly Assembly { get; private set; }
        internal FileVersionInfo VersionInfo { get; private set; }

        internal bool IsStartupOk { get; private set; }

#pragma warning disable IDE0052 // 删除未读的私有成员
        private static Mutex mutex;
#pragma warning restore IDE0052 // 删除未读的私有成员
        private const string mutexName = "181f92f7-3f77-4803-b46b-1da4d1c2a66e";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 重复运行
            bool createdNew;
            mutex = new(true, mutexName, out createdNew);
            if (!createdNew)
            {
                var currentProcess = Process.GetCurrentProcess();
                var processes = Process.GetProcessesByName(currentProcess.ProcessName);
                var process = processes.FirstOrDefault(x => x.Id != currentProcess.Id);
                if (process != null)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        SetForegroundWindow(process.MainWindowHandle);
                    }
                }
                Shutdown(3);
                return;
            }

            try
            {
                Assembly = Assembly.GetExecutingAssembly();
                VersionInfo = FileVersionInfo.GetVersionInfo(Assembly.Location);
            }
            catch (Exception err)
            {
                MessageBox.Show(
                    Current.MainWindow,
                    $"程序集加载失败，即将退出。\r\n\r\n{err}",
                    "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                Shutdown(1);
                return;
            }

            try
            {
                log4net.GlobalContext.Properties["ProcessId"] = Process.GetCurrentProcess().Id;
                log4net.GlobalContext.Properties["ProductName"] = VersionInfo.ProductName;
                var userLoggingConfigFile = Path.Combine(Path.GetDirectoryName(ConfigManager.UserSettingsPath), "log4net.config");
                var appLoggingConfigFile = Path.Combine("Config", "log4net.config");
                if (File.Exists(userLoggingConfigFile))
                {
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(userLoggingConfigFile));
                }
                else
                {
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(appLoggingConfigFile));
                }
                logger.WarnFormat(
                    "\r\n!!!!!!!!!!!!!!!!!!!! Startup (version {0}) !!!!!!!!!!!!!!!!!!!!\r\n",
                    VersionInfo.ProductVersion
                );
            }
            catch (Exception err)
            {
                logger.ErrorFormat("日志配置加载失败: {0}\r\n^^^^^^^^^^^^^^^^^^^^ Shutdown ^^^^^^^^^^^^^^^^^^^^\r\n", err);
                MessageBox.Show(
                    Current.MainWindow,
                    $"日志配置加载失败，程序无法运行，即将退出。\r\n\r\n{err}",
                    VersionInfo.FileDescription,
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                Shutdown(2);
                return;
            }

            try
            {
                ConfigManager.GetAllSettings();
            }
            catch (Exception err)
            {
                logger.ErrorFormat("配置信息加载失败: {0}\r\n^^^^^^^^^^^^^^^^^^^^ Shutdown ^^^^^^^^^^^^^^^^^^^^\r\n", err);
                MessageBox.Show(
                    Current.MainWindow,
                    $"配置信息加载失败，程序无法运行，即将退出。\r\n\r\n{err}",
                    VersionInfo.FileDescription,
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                Shutdown(4);
                return;
            }

            IsStartupOk = true;
        }

        private static Application GetCurrent()
        {
            return Application.Current;
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            switch (e.Exception)
            {
                case client.ErrorResponse:
                    logger.ErrorFormat("Application Dispatcher Unhandled Exception - CTI ErrorResponse: {0}", e.Exception);
                    MessageBox.Show(
                        Current.MainWindow,
                        $"发送到 CTI 服务器的请求返回了错误结果 ({(e.Exception as client.ErrorResponse).Code})。\r\n\r\n{e.Exception.Message}",
                        Current.MainWindow.Title,
                        MessageBoxButton.OK, MessageBoxImage.Warning
                    );
                    break;
                case client.RequestTimeoutError:
                    logger.ErrorFormat("Application Dispatcher Unhandled Exception - CTI RequestTimeoutError: {0}", e.Exception);
                    MessageBox.Show(
                        Current.MainWindow,
                        "发送到 CTI 服务器的请求超时。",
                        Current.MainWindow.Title,
                        MessageBoxButton.OK, MessageBoxImage.Error
                    );
                    break;
                case client.RequestNotCompleteError:
                    logger.ErrorFormat("Application Dispatcher Unhandled Exception - CTI RequestNotCompleteError: {0}", e.Exception);
                    MessageBox.Show(
                        Current.MainWindow,
                        "由于已经有 CTI 服务请求正在执行，现在无法进行新的请求。",
                        Current.MainWindow.Title,
                        MessageBoxButton.OK, MessageBoxImage.Information
                    );
                    break;
                default:
                    logger.ErrorFormat("Application Dispatcher Unhandled Exception - {0}", e.Exception);
                    MessageBox.Show(
                        Current.MainWindow,
                        $"程序运行过程中出现了未捕获的异常。\r\n\r\n{e.Exception}",
                        Current.MainWindow.Title,
                        MessageBoxButton.OK, MessageBoxImage.Error
                    );
                    break;
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            logger.Warn("\r\n^^^^^^^^^^^^^^^^^^^^ Exit ^^^^^^^^^^^^^^^^^^^^\r\n");
        }
    }
}
