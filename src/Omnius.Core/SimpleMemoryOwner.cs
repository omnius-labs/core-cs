using System;
using System.Buffers;

namespace Omnius.Core
{
    public sealed class SimpleMemoryOwner<T> : IMemoryOwner<T>
    {
        public static IMemoryOwner<T> Empty { get; } = new SimpleMemoryOwner<T>();

        public SimpleMemoryOwner() => this.Memory = Memory<T>.Empty;
        public SimpleMemoryOwner(Memory<T> memory) => this.Memory = memory;

        public Memory<T> Memory { get; }

        public void Dispose() { }
    }
}
