using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Toolkit.Mvvm.Input;

#pragma warning disable IDE0058, VSTHRD003, VSTHRD111

namespace ipsc6.agent.wpfapp.Utils
{
    public class CommandGuard : IAsyncDisposable, IDisposable
    {
        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
#pragma warning disable IDE0058
                    semaphore.Release();
#pragma warning restore IDE0058
                    foreach (var command in commands)
                    {
                        Application.Current.Dispatcher.Invoke(command.NotifyCanExecuteChanged);
                    }
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
        private CommandGuard(IEnumerable<IRelayCommand> commands)
        {
            this.commands = new HashSet<IRelayCommand>(commands);
            foreach (var command in commands)
            {
                Application.Current.Dispatcher.Invoke(command.NotifyCanExecuteChanged);
            }
        }

        public static async Task<CommandGuard> CreateAsync(IRelayCommand command)
        {
            return await CreateAsync(new IRelayCommand[] { command });
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
        //            _onDispose?.Invoke();
        //            await Task.CompletedTask;
        //        }

        public static bool IsGuarding => semaphore.CurrentCount == 0;
    }
}
#pragma warning restore IDE0058, VSTHRD003, VSTHRD111
