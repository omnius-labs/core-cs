using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Base
{
    /// <summary>
    /// <see cref="IDisposable"/>を実装します。
    /// </summary>
    public abstract class DisposableBase : IDisposable, IAsyncDisposable
    {
        private int _called = 0;

        protected bool IsDisposed => (_called == 1);

        protected void ThrowIfDisposingRequested()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        #region IDisposable

        public void Dispose()
        {
            this.InternalDispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisposableBase()
        {
            this.InternalDispose(false);
        }

        private void InternalDispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref _called, 1, 0) == 0)
            {
                this.OnDispose(disposing);
            }
        }

        protected abstract void OnDispose(bool disposing);

        #endregion

        #region IAsyncDisposable

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref _called, 1, 0) == 0)
            {
                await this.OnDisposeAsync();
            }
        }

        protected virtual async ValueTask OnDisposeAsync()
        {
            await Task.Run(() =>
            {
                this.Dispose();
            });
        }

        #endregion
    }
}
