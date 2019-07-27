using System;
using System.Buffers;

namespace Omnix.Base
{
    public sealed class SimpleMemoryOwner<T> : IMemoryOwner<T>
    {
        public static IMemoryOwner<T> Empty { get; } = new SimpleMemoryOwner<T>();

        public Memory<T> Memory => Memory<T>.Empty;
        public void Dispose() { }
    }
}
