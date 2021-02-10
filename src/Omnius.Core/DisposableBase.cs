using System;
using System.Threading;

namespace Omnius.Core
{
    /// <summary>
    /// <see cref="IDisposable"/>を実装します。
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {
        private int _called = 0;

        protected bool IsDisposed => (_called == 1);

        protected void ThrowIfDisposingRequested()
        {
            if (this.IsDisposed) throw new ObjectDisposedException(this.GetType().FullName);
        }

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
    }
}
