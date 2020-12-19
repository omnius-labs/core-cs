using System;
using System.Threading;

namespace Omnius.Core
{
    /// <summary>
    /// <see cref="ReaderWriterLockSlim"/>のラッパークラスです。
    /// </summary>
    public sealed class ReaderWriterLockProvider : DisposableBase
    {
        private readonly ReaderWriterLockSlim _lock;

        public ReaderWriterLockProvider(LockRecursionPolicy recursionPolicy)
        {
            _lock = new ReaderWriterLockSlim(recursionPolicy);
        }

        public IDisposable ReadLock()
        {
            try
            {
                _lock.EnterUpgradeableReadLock();
                return new ReadLockCookie(_lock);
            }
            catch (Exception)
            {
                _lock.ExitUpgradeableReadLock();

                throw;
            }
        }

        public IDisposable WriteLock()
        {
            try
            {
                _lock.EnterWriteLock();
                return new WriteLockCookie(_lock);
            }
            catch (Exception)
            {
                _lock.ExitWriteLock();

                throw;
            }
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _lock.Dispose();
            }
        }

        private readonly struct ReadLockCookie : IDisposable
        {
            private readonly ReaderWriterLockSlim _readerWriterLock;

            public ReadLockCookie(ReaderWriterLockSlim readerWriterLock)
            {
                _readerWriterLock = readerWriterLock;
            }

            public void Dispose()
            {
                _readerWriterLock.ExitUpgradeableReadLock();
            }
        }

        private readonly struct WriteLockCookie : IDisposable
        {
            private readonly ReaderWriterLockSlim _readerWriterlock;

            public WriteLockCookie(ReaderWriterLockSlim readerWriterlock)
            {
                _readerWriterlock = readerWriterlock;
            }

            public void Dispose()
            {
                _readerWriterlock.ExitWriteLock();
            }
        }
    }
}
