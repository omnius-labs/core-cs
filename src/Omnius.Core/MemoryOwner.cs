using System.Buffers;

namespace Omnius.Core;

public sealed class MemoryOwner<T> : IMemoryOwner<T>
{
    public static IMemoryOwner<T> Empty { get; } = new MemoryOwner<T>();

    private readonly Action? _disposeCallback;

    public MemoryOwner()
    {
        this.Memory = Memory<T>.Empty;
    }

    public MemoryOwner(Memory<T> memory)
    {
        this.Memory = memory;
    }

    public MemoryOwner(Memory<T> memory, Action disposeCallback)
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