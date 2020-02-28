using System;
using System.Buffers;
using Omnius.Core;

namespace Omnius.Core.Serialization.RocketPack
{
    public interface IRocketPackObject<T> : IEquatable<T>
    {
        private static IRocketPackFormatter<T> _formatter;
        private static T _empty;

        public static IRocketPackFormatter<T> Formatter
        {
            get
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
                return _formatter;
            }
            protected set
            {
                _formatter = value;
            }
        }

        public static T Empty
        {
            get
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
                return _empty;
            }
            protected set
            {
                _empty = value;
            }
        }

        public static T Import(ReadOnlySequence<byte> sequence, IBytesPool bytesPool)
        {
            var reader = new RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }

        public virtual void Export(IBufferWriter<byte> bufferWriter, IBytesPool bytesPool)
        {
            var writer = new RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, (T)this, 0);
        }
    }
}
