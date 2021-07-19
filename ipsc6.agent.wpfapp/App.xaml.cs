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
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine("Config", "log4net.config")));
                logger.WarnFormat("\r\n!!!!!!!!!!!!!!!!!!!! Startup (AssemblyVersion {0}, FileVersion {1}) !!!!!!!!!!!!!!!!!!!!\r\n", Assembly.GetName().Version, VersionInfo.FileVersion);
            }
            catch (Exception err)
            {
                logger.ErrorFormat("日志配置加载失败: {0}\r\n^^^^^^^^^^^^^^^^^^^^ Shutdown ^^^^^^^^^^^^^^^^^^^^\r\n", err);
                MessageBox.Show(
                    $"日志配置加载失败，程序无法运行，即将退出。\r\n\r\n{err}",
                    VersionInfo.FileDescription,
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                Shutdown(2);
                return;
            }

            try
            {
                ConfigManager.Initialize();
            }
            catch (Exception err)
            {
                logger.ErrorFormat("配置信息加载失败: {0}\r\n^^^^^^^^^^^^^^^^^^^^ Shutdown ^^^^^^^^^^^^^^^^^^^^\r\n", err);
                MessageBox.Show(
                    $"配置信息加载失败，程序无法运行，即将退出。\r\n\r\n{err}",
                    VersionInfo.FileDescription,
                    MessageBoxButton.OK, MessageBoxImage.Error
                );
                Shutdown(4);
                return;
            }

            IsStartupOk = true;
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            logger.ErrorFormat("Application_DispatcherUnhandledException: {0}", e.Exception);
            MessageBox.Show(
                $"程序运行过程中出现了未捕获的异常。\r\n\r\n{e.Exception}",
                VersionInfo.ProductName,
                MessageBoxButton.OK, MessageBoxImage.Error
            );
        }

    }
}
