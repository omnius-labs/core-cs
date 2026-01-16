using System.Buffers;

namespace Omnius.Core.Base;

public sealed class OwnedMemory<T> : IMemoryOwner<T>
{
    public static IMemoryOwner<T> Empty { get; } = new OwnedMemory<T>();

    private readonly Action? _disposeCallback;

    public OwnedMemory()
    {
        this.Memory = Memory<T>.Empty;
    }

    public OwnedMemory(Memory<T> memory)
    {
        this.Memory = memory;
    }

    public OwnedMemory(Memory<T> memory, Action disposeCallback)
    {
        this.Memory = memory;
        _disposeCallback = disposeCallback;
    }

    public Memory<T> Memory { get; }

    public void Dispose()
    {
        _disposeCallback?.Invoke();
    }
}
