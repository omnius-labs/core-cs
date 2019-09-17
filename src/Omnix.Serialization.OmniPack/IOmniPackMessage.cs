using System;
using System.Buffers;
using Omnix.Base;

namespace Omnix.Serialization.OmniPack
{
    public interface IOmniPackMessage<T> : IEquatable<T>
    {
        public static IOmniPackFormatter<T> Formatter { get; }

        public static T Empty { get; }

        public static T Import(ReadOnlySequence<byte> sequence, BufferPool bufferPool)
        {
            var reader = new OmniPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }

        public void Export(IBufferWriter<byte> bufferWriter, BufferPool bufferPool)
        {
            var writer = new OmniPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, (T)this, 0);
        }
    }
}
