using System.Buffers;

namespace Core.Base;

public interface IBytesPool
{
    ArrayPool<byte> Array { get; }
    MemoryPool<byte> Memory { get; }
}
