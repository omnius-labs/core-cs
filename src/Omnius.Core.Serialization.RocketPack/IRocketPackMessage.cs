using System;
using System.Buffers;
using Omnius.Core;

namespace Omnius.Core.Serialization.RocketPack
{
    public interface IRocketPackMessage<T> : IEquatable<T>
    {
        public static IRocketPackFormatter<T> Formatter { get; }

        public static T Empty { get; }

        public static T Import(ReadOnlySequence<byte> sequence, IBufferPool<byte> bufferPool)
        {
            var reader = new RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }

        public void Export(IBufferWriter<byte> bufferWriter, IBufferPool<byte> bufferPool)
        {
            var writer = new RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, (T)this, 0);
        }
    }
}
