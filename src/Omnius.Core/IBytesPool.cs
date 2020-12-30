using System.Buffers;

namespace Omnius.Core
{
    public interface IBytesPool
    {
        ArrayPool<byte> Array { get; }

        MemoryPool<byte> Memory { get; }
    }
}
