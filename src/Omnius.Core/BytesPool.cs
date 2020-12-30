using System.Buffers;

namespace Omnius.Core
{
    public sealed class BytesPool : IBytesPool
    {
        private readonly ArrayPool<byte> _arrayPool;
        private readonly MemoryPool<byte> _memoryPool;

        private BytesPool()
        {
            _arrayPool = ArrayPool<byte>.Shared;
            _memoryPool = MemoryPool<byte>.Shared;
        }

        public static BytesPool Shared { get; } = new BytesPool();

        public static BytesPool Create()
        {
            return new BytesPool();
        }

        public ArrayPool<byte> Array => _arrayPool;

        public MemoryPool<byte> Memory => _memoryPool;
    }
}
