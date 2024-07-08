using System.Buffers;

namespace Omnius.Core.Base;

public interface IBytesPool
{
    ArrayPool<byte> Array { get; }
    MemoryPool<byte> Memory { get; }
}
