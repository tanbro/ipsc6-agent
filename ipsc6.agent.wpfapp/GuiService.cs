using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ipsc6.agent.wpfapp
{
    public class GuiService : IDisposable
    {
        #region Dispose
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing) { }
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~GuiService()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(GuiService));

#pragma warning disable VSTHRD200
        public async Task LogIn(string workerNum, string password, IEnumerable<string> serverList = null)
        {
            /// 改 UI 的输入框
            serverList ??= (new string[] { });
            Tuple<string, string, IEnumerable<string>> param = new(workerNum, password, serverList);
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                await ViewModels.LoginViewModel.DoLoginAsync(param);
            });
        }

        public async Task LogOut()
        {
            await Application.Current.Dispatcher.Invoke(async () =>
            {
                await ViewModels.MainViewModel.DoLogoutAsync(false);
            });
        }

        public void ExitApp(int code = 0)
        {
            logger.Warn("强行退出");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Application.Current.Shutdown(code);
            });
        }

        public void ShowApp()
        {
            var mainViewModel = ViewModels.MainViewModel.Instance;
            mainViewModel.ShowMainWindow();
        }

        public void HideApp()
        {
            var mainViewModel = ViewModels.MainViewModel.Instance;
            mainViewModel.HideMainWindow();
        }

#pragma warning restore VSTHRD200
    }
}
