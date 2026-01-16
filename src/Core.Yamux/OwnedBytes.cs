using System.Buffers;

namespace Omnius.Yamux;

internal readonly struct OwnedBytes : IDisposable
{
    public static OwnedBytes Empty { get; } = new OwnedBytes(ReadOnlyMemory<byte>.Empty, null);

    private readonly IMemoryOwner<byte>? _owner;

    public OwnedBytes(ReadOnlyMemory<byte> memory, IMemoryOwner<byte>? owner)
    {
        this.Memory = memory;
        _owner = owner;
    }

    public ReadOnlyMemory<byte> Memory { get; }

    public void Dispose()
    {
        _owner?.Dispose();
    }
}
