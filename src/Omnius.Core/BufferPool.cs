using System;
using System.Buffers;

namespace Omnius.Core
{
    public sealed class BufferPool<T> : IBufferPool<T>
    {
        private readonly ArrayPool<T> _arrayPool;
        private readonly MemoryPool<T> _memoryPool;

        private BufferPool()
        {
            _arrayPool = ArrayPool<T>.Shared;
            _memoryPool = MemoryPool<T>.Shared;
        }

        public static BufferPool<T> Shared { get; } = new BufferPool<T>();

        public static BufferPool<T> Create()
        {
            return new BufferPool<T>();
        }

        public ArrayPool<T> Array => _arrayPool;
        public MemoryPool<T> Memory => _memoryPool;
    }
}
