using System;
using System.Buffers;
using Omnius.Core;

namespace Omnius.Core.Serialization.RocketPack
{
    public interface IRocketPackObject<T> : IEquatable<T>
    {
        public static IRocketPackFormatter<T> Formatter { get; protected set; }
        public static T Empty { get; protected set; }

        public static T Import(ReadOnlySequence<byte> sequence, IBytesPool bytesPool)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
            var reader = new RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }

        public virtual void Export(IBufferWriter<byte> bufferWriter, IBytesPool bytesPool)
        {
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
            var writer = new RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, (T)this, 0);
        }
    }
}
