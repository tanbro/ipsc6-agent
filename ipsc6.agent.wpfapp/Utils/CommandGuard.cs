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
                    // 释放托管状态(托管对象)
                    semaphore.Release();
                    Application.Current.Dispatcher.Invoke(() =>
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
                    });
                }

                // 释放未托管的资源(未托管的对象)并重写终结器
                // 将大型字段设置为 null
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

            Dispose(disposing: false);
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
            GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
            await Task.CompletedTask;
        }

        private static readonly SemaphoreSlim semaphore = new(1);
        private readonly IReadOnlyCollection<IRelayCommand> commands;

        private CommandGuard()
        {
            commands = new IRelayCommand[] { };
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
            });
        }

        private CommandGuard(IEnumerable<IRelayCommand> commands)
        {
            this.commands = new HashSet<IRelayCommand>(commands);
            Application.Current.Dispatcher.Invoke(() =>
            {
                Mouse.OverrideCursor = Cursors.Wait;
                foreach (var command in commands)
                {
                    command.NotifyCanExecuteChanged();
                }
            });
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
            return new CommandGuard(commands);
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
            return new CommandGuard(commands);
        }

        //#pragma warning disable VSTHRD200
        //        private async Task DisposeAsyncCore()
        //#pragma warning restore VSTHRD200
        //        {
        //        }

        public static bool IsGuarding => semaphore.CurrentCount == 0;
        public static int CurrentCount => semaphore.CurrentCount;
    }
}
#pragma warning restore IDE0058, VSTHRD003, VSTHRD111
