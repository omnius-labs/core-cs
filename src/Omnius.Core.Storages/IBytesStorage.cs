using System;
using System.Buffers;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Storages
{
    public interface IBytesStorageFactory
    {
        IBytesStorage<TKey> Create<TKey>(string path, IBytesPool bytesPool)
            where TKey : notnull, IEquatable<TKey>;
    }

    public interface IBytesStorage<TKey> : IDisposable
        where TKey : notnull, IEquatable<TKey>
    {
        ValueTask MigrateAsync(CancellationToken cancellationToken = default);

        ValueTask RebuildAsync(CancellationToken cancellationToken = default);

        ValueTask ChangeKeyAsync(TKey oldKey, TKey newKey, CancellationToken cancellationToken = default);

        ValueTask<bool> ContainsKeyAsync(TKey key, CancellationToken cancellationToken = default);

        IAsyncEnumerable<TKey> GetKeysAsync(CancellationToken cancellationToken = default);

        ValueTask<IMemoryOwner<byte>?> TryReadAsync(TKey key, CancellationToken cancellationToken = default);

        ValueTask<bool> TryReadAsync(TKey key, IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default);

        ValueTask WriteAsync(TKey key, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default);

        ValueTask WriteAsync(TKey key, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default);

        ValueTask<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = default);
    }

    public class DuplicateKeyException : Exception
    {
        public DuplicateKeyException()
            : base()
        {
        }

        public DuplicateKeyException(string message)
            : base(message)
        {
        }

        public DuplicateKeyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
