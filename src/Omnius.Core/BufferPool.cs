using System;
using System.Buffers;

namespace Omnius.Core
{
    /// <summary>
    /// <see cref="ArrayPool{T}"/>を利用した<see cref="IBufferPool{T}"/>の実装です。
    /// </summary>
    public sealed class BufferPool<T> : IBufferPool<T>
    {
        private readonly ArrayPool<T> _arrayPool;

        private BufferPool(ArrayPool<T> arrayPool)
        {
            _arrayPool = arrayPool;
        }

        public static BufferPool<T> Shared { get; } = new BufferPool<T>(ArrayPool<T>.Shared);

        public static BufferPool<T> Create()
        {
            return new BufferPool<T>(ArrayPool<T>.Create());
        }

        public int MaxBufferSize => int.MaxValue;

        public IMemoryOwner<T> RentMemory(int size)
        {
            if (size <= 0)
            {
                return SimpleMemoryOwner<T>.Empty;
            }

            return new BufferPoolMemoryOwner(_arrayPool, size);
        }

        public T[] RentArray(int minimumLength)
        {
            if (minimumLength <= 0)
            {
                return Array.Empty<T>();
            }

            return _arrayPool.Rent(minimumLength);
        }

        public void ReturnArray(T[] array)
        {
            _arrayPool.Return(array);
        }

        private sealed class BufferPoolMemoryOwner : IMemoryOwner<T>
        {
            private readonly ArrayPool<T> _arrayPool;
            private T[]? _bytes;
            private readonly int _size;

            public BufferPoolMemoryOwner(ArrayPool<T> arrayPool, int size)
            {
                _arrayPool = arrayPool;
                _bytes = _arrayPool.Rent(size);
                _size = size;
            }

            public Memory<T> Memory
            {
                get
                {
                    var bytes = _bytes;

                    if (bytes == null)
                    {
                        throw new ObjectDisposedException(nameof(BufferPoolMemoryOwner));
                    }

                    return new Memory<T>(bytes, 0, _size);
                }
            }

            public void Dispose()
            {
                var bytes = _bytes;

                if (bytes != null)
                {
                    _bytes = null;
                    _arrayPool.Return(bytes);
                }
            }
        }
    }
}
