using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core
{
    /// <summary>
    /// <see cref="IAsyncDisposable"/>を実装します。
    /// </summary>
    public abstract class AsyncDisposableBase : IAsyncDisposable
    {
        private int _called = 0;

        protected bool IsDisposed => (_called == 1);

        protected void ThrowIfDisposingRequested()
        {
            if (this.IsDisposed) throw new ObjectDisposedException(this.GetType().FullName);
        }

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref _called, 1, 0) == 0)
            {
                await this.OnDisposeAsync();
            }
        }

        protected abstract ValueTask OnDisposeAsync();
    }
}
