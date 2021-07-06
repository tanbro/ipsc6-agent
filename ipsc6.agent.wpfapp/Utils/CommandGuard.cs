using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.Input;

#pragma warning disable IDE0058, VSTHRD003, VSTHRD111

namespace ipsc6.agent.wpfapp.Utils
{
    public class CommandGuard : IAsyncDisposable, IDisposable
    {
        //private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(CommandGuard));

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Application.Current.Dispatcher.Invoke(Release);
                }
                semaphore.Release();
                disposedValue = true;
            }
        }

        // // 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~CommandGuard()
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

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();

            Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
        }

#pragma warning disable VSTHRD200
        protected async ValueTask DisposeAsyncCore()
#pragma warning restore VSTHRD200
        {
            await Application.Current.Dispatcher.InvokeAsync(Release);
        }

        private static readonly SemaphoreSlim semaphore = new(1);
        private readonly IReadOnlyCollection<IRelayCommand> commands;

        private CommandGuard(IEnumerable<IRelayCommand> commands)
        {
            this.commands = new HashSet<IRelayCommand>(commands);
        }

        private CommandGuard Initial()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
                foreach (var command in commands)
                {
                    command.NotifyCanExecuteChanged();
                }
            });
            return this;
        }

        private void Release()
        {
            try
            {
                foreach (var command in commands)
                {
                    command.NotifyCanExecuteChanged();
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private async Task<CommandGuard> InitialAsync()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
                foreach (var command in commands)
                {
                    command.NotifyCanExecuteChanged();
                }
            });
            return this;
        }

        public static CommandGuard Create()
        {
            IRelayCommand[] commands = new IRelayCommand[] { };
            return Create(commands);
        }

        public static CommandGuard Create(IRelayCommand command)
        {
            IRelayCommand[] commands = new IRelayCommand[] { command };
            return Create(commands);
        }

        public static CommandGuard Create(IEnumerable<IRelayCommand> commands)
        {
            semaphore.Wait();
            return new CommandGuard(commands).Initial();
        }

        public static async Task<CommandGuard> CreateAsync()
        {
            IRelayCommand[] commands = new IRelayCommand[] { };
            return await CreateAsync(commands);
        }

        public static async Task<CommandGuard> CreateAsync(IRelayCommand command)
        {
            IRelayCommand[] commands = new IRelayCommand[] { command };
            return await CreateAsync(commands);
        }

        public static async Task<CommandGuard> CreateAsync(IEnumerable<IRelayCommand> commands)
        {
            await semaphore.WaitAsync();
            return await new CommandGuard(commands).InitialAsync();
        }

        public static bool IsGuarding => semaphore.CurrentCount == 0;
        public static int CurrentCount => semaphore.CurrentCount;
    }
}
#pragma warning restore IDE0058, VSTHRD003, VSTHRD111
