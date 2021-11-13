using System.Buffers;

namespace Omnius.Core.Storages;

public interface IKeyValueStorage<TKey> : IDisposable
    where TKey : notnull
{
    ValueTask MigrateAsync(CancellationToken cancellationToken = default);

    ValueTask RebuildAsync(CancellationToken cancellationToken = default);

    ValueTask<bool> TryChangeKeyAsync(TKey oldKey, TKey newKey, CancellationToken cancellationToken = default);

    ValueTask<bool> ContainsKeyAsync(TKey key, CancellationToken cancellationToken = default);

    IAsyncEnumerable<TKey> GetKeysAsync(CancellationToken cancellationToken = default);

    ValueTask<IMemoryOwner<byte>?> TryReadAsync(TKey key, CancellationToken cancellationToken = default);

    ValueTask<bool> TryReadAsync(TKey key, IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default);

    ValueTask<bool> TryWriteAsync(TKey key, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default);

    ValueTask<bool> TryWriteAsync(TKey key, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default);

    ValueTask<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = default);
}