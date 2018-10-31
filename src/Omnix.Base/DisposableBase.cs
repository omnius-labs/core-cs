using System;

namespace Omnix.Base
{
    /// <summary>
    /// <see cref="IDisposable"/>を実装します。
    /// </summary>
    public abstract class DisposableBase : IDisposable
    {
        ~DisposableBase()
        {
            this.Dispose(false);
        }

        protected abstract void Dispose(bool disposing);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
