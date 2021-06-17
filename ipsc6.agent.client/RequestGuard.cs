using System;
using System.Threading;

namespace ipsc6.agent.client
{
    class DisposableSingleSemaphore : IDisposable
    {
        private readonly SemaphoreSlim _semaphoreSlim;

        public DisposableSingleSemaphore(SemaphoreSlim semaphore)
        {
            _semaphoreSlim = semaphore;
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                    _semaphoreSlim.Release();
                }
                // 释放未托管的资源(未托管的对象)并重写终结器
                // 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~DisposableSingleSemaphore()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    class RequestGuard
    {
        private readonly SemaphoreSlim semaphore = new(1);

        public IDisposable TryEnter()
        {
            if (!semaphore.Wait(0))
                throw new RequestNotCompleteError();
            return new DisposableSingleSemaphore(semaphore);
        }

        public bool IsEntered => semaphore.CurrentCount == 0;
    }

}
