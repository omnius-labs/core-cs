using System;
using System.Buffers;

namespace Omnius.Core
{
    public sealed class EmptyMemoryOwner<T> : IMemoryOwner<T>
    {
        public static IMemoryOwner<T> Empty { get; } = new EmptyMemoryOwner<T>();

        public EmptyMemoryOwner() => this.Memory = Memory<T>.Empty;
        public Memory<T> Memory { get; }

        public void Dispose() { }
    }
}
