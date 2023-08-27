using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

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
                semaphore.Release();
                try
                {
                    if (disposing)
                    {
                        Application.Current.Dispatcher.Invoke(ReleaseCore);
                    }
                }
                finally
                {
                    disposedValue = true;
                }
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
            await Application.Current.Dispatcher.InvokeAsync(ReleaseCore);
        }

        private static readonly SemaphoreSlim semaphore = new(1);
        private readonly IReadOnlyCollection<IRelayCommand> commands;

        private CommandGuard(IEnumerable<IRelayCommand> commands)
        {
            this.commands = new HashSet<IRelayCommand>(commands);
        }

        private void InitialCore()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                foreach (var command in commands)
                {
                    command.NotifyCanExecuteChanged();
                }
            }
            catch
            {
                Mouse.OverrideCursor = null;
                throw;
            }
        }

        private CommandGuard Initial()
        {
            Application.Current.Dispatcher.Invoke(InitialCore);
            return this;
        }

        private async Task<CommandGuard> InitialAsync()
        {
            await Application.Current.Dispatcher.InvokeAsync(InitialCore);
            return this;
        }

        private void ReleaseCore()
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

        public static CommandGuard Enter()
        {
            return Enter(Array.Empty<IRelayCommand>());
        }

        public static CommandGuard Enter(IRelayCommand command)
        {
            return Enter(new IRelayCommand[] { command });
        }

        public static CommandGuard Enter(IEnumerable<IRelayCommand> commands)
        {
            semaphore.Wait();
            return new CommandGuard(commands).Initial();
        }

        public static async Task<CommandGuard> EnterAsync()
        {
            return await EnterAsync(Array.Empty<IRelayCommand>());
        }

        public static async Task<CommandGuard> EnterAsync(IRelayCommand command)
        {
            return await EnterAsync(new IRelayCommand[] { command });
        }

        public static async Task<CommandGuard> EnterAsync(IEnumerable<IRelayCommand> commands)
        {
            await semaphore.WaitAsync();
            return await new CommandGuard(commands).InitialAsync();
        }

        public static bool IsGuarding => semaphore.CurrentCount == 0;
        public static int CurrentCount => semaphore.CurrentCount;
    }
}
#pragma warning restore IDE0058, VSTHRD003, VSTHRD111
