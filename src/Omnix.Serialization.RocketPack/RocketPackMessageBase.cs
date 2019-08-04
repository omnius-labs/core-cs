using System;
using System.Buffers;
using Omnix.Base;

namespace Omnix.Serialization.RocketPack
{
    public abstract class RocketPackMessageBase<T> : IEquatable<T>
        where T : RocketPackMessageBase<T>
    {
        static RocketPackMessageBase()
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
        }

        public static IRocketPackFormatter<T> Formatter { get; protected set; }

        public static T Empty { get; protected set; }

        public static T Import(ReadOnlySequence<byte> sequence, BufferPool bufferPool)
        {
            var reader = new RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }

        public void Export(IBufferWriter<byte> bufferWriter, BufferPool bufferPool)
        {
            var writer = new RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, (T)this, 0);
        }

        public static bool operator ==(RocketPackMessageBase<T>? left, RocketPackMessageBase<T>? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }

        public static bool operator !=(RocketPackMessageBase<T>? left, RocketPackMessageBase<T>? right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object? other)
        {
            if (!(other is T))
            {
                return false;
            }

            return this.Equals((T)other);
        }

        public abstract bool Equals(T other);
    }
}
