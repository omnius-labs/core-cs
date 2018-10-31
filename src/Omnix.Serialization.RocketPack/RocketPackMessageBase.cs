using System;
using System.Buffers;
using System.IO;
using System.Runtime.Serialization;
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

        public virtual void Initialize() { }

        public static T Import(ReadOnlySequence<byte> sequence, BufferPool bufferPool)
        {
            return Formatter.Deserialize(new RocketPackReader(sequence, bufferPool), 0);
        }

        public void Export(IBufferWriter<byte> bufferWriter, BufferPool bufferPool)
        {
            Formatter.Serialize(new RocketPackWriter(bufferWriter, bufferPool), (T)this, 0);
        }

        public static bool operator ==(RocketPackMessageBase<T> x, RocketPackMessageBase<T> y)
        {
            if ((object)x == null)
            {
                if ((object)y == null)
                {
                    return true;
                }
                else
                {
                    return ((T)y).Equals((T)x);
                }
            }
            else
            {
                return ((T)x).Equals((T)y);
            }
        }

        public static bool operator !=(RocketPackMessageBase<T> x, RocketPackMessageBase<T> y)
        {
            return !(x == y);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is T)) return false;
            return this.Equals((T)other);
        }

        public abstract bool Equals(T other);
    }
}
