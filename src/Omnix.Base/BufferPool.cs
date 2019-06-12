using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace Omnix.Base
{
    /// <summary>
    /// <see cref="ArrayPool{T}"/>を利用した<see cref="MemoryPool{T}"/>の<see cref="byte"/>型の実装です。
    /// </summary>
    public sealed class BufferPool : MemoryPool<byte>
    {
        private readonly ArrayPool<byte> _arrayPool;

        private BufferPool(ArrayPool<byte> arrayPool)
        {
            _arrayPool = arrayPool;
        }

        public static new BufferPool Shared { get; } = new BufferPool(ArrayPool<byte>.Shared);

        public static BufferPool Create()
        {
            return new BufferPool(ArrayPool<byte>.Create());
        }

        public ArrayPool<byte> GetArrayPool() => _arrayPool;

        public override int MaxBufferSize => int.MaxValue;

        public override IMemoryOwner<byte> Rent(int bufferSize)
        {
            if (bufferSize <= 0)
            {
                return SimpleMemoryOwner<byte>.Empty;
            }

            return new BufferPoolMemoryOwner(_arrayPool, bufferSize);
        }

        private sealed class BufferPoolMemoryOwner : IMemoryOwner<byte>
        {
            private readonly ArrayPool<byte> _arrayPool;
            private byte[]? _bytes;
            private readonly int _size;

            public BufferPoolMemoryOwner(ArrayPool<byte> arrayPool, int size)
            {
                _arrayPool = arrayPool;
                _bytes = _arrayPool.Rent(size);
                _size = size;
            }

            public Memory<byte> Memory
            {
                get
                {
                    var bytes = _bytes;
                    if (bytes == null)
                    {
                        throw new ObjectDisposedException(nameof(BufferPoolMemoryOwner));
                    }

                    return new Memory<byte>(bytes, 0, _size);
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

        protected override void Dispose(bool disposing)
        {

        }
    }
}
