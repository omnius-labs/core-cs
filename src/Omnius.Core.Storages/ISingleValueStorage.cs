using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Storages
{
    public interface ISingleValueStorage : IDisposable
    {
        ValueTask<IMemoryOwner<byte>?> TryReadAsync(CancellationToken cancellationToken = default);

        ValueTask<bool> TryReadAsync(IBufferWriter<byte> bufferWriter, CancellationToken cancellationToken = default);

        ValueTask<bool> TryWriteAsync(ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default);

        ValueTask<bool> TryWriteAsync(ReadOnlyMemory<byte> memory, CancellationToken cancellationToken = default);

        ValueTask<bool> TryDeleteAsync(CancellationToken cancellationToken = default);
    }
}
