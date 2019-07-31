using System;
using System.Threading;

namespace Omnix.Base
{
    /// <summary>
    /// <see cref="IDisposable"/>を実装します。
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {
        private int _called = 0;

        ~DisposableBase()
        {
            this.InternalDispose(false);
        }

        protected bool IsDisposed => (_called == 1);

        protected void ThrowIfDisposingRequested()
        {
            if (this.IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        private void InternalDispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref _called, 1, 0) == 0)
            {
                this.Dispose(disposing);
            }
        }

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            this.InternalDispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
