using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



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

#pragma warning disable VSTHRD200
        public async Task LogIn(string workerNum, string password)
        {
            /// 改 UI 的输入框
            ViewModels.LoginViewModel.Instance.WorkerNum = workerNum;
#pragma warning disable VSTHRD111
            await ViewModels.LoginViewModel.DoLoginAsync(workerNum, password);
#pragma warning restore VSTHRD111
        }
#pragma warning restore VSTHRD200
    }
}
