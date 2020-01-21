using System.Buffers;

namespace Omnius.Core
{
    public interface IBufferPool<T>
    {
        ArrayPool<T> Array { get; }
        MemoryPool<T> Memory { get; }
    }
}
