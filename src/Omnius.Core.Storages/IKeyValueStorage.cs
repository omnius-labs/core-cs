using System.Buffers;

namespace Omnius.Core.Storages;

public interface IKeyValueStorage : IAsyncDisposable
{
    ValueTask MigrateAsync(CancellationToken cancellationToken = default);
    ValueTask RebuildAsync(CancellationToken cancellationToken = default);
    ValueTask<bool> TryChangeKeyAsync(string oldKey, string newKey, CancellationToken cancellationToken = default);
    ValueTask<bool> ContainsKeyAsync(string key, CancellationToken cancellationToken = default);
    IAsyncEnumerable<string> GetKeysAsync(CancellationToken cancellationToken = default);
    ValueTask<IMemoryOwner<byte>?> TryReadAsync(string key, CancellationToken cancellationToken = default);
    ValueTask<bool> TryReadAsync(string key, IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default);
    ValueTask WriteAsync(string key, ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default);
    ValueTask WriteAsync(string key, ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default);
    ValueTask<bool> TryDeleteAsync(string key, CancellationToken cancellationToken = default);
    ValueTask ShrinkAsync(IEnumerable<string> excludedKeys, CancellationToken cancellationToken = default);
}
