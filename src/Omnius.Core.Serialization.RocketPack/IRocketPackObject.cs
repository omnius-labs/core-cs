using System;
using System.Buffers;
using Omnius.Core;

namespace Omnius.Core.Serialization.RocketPack
{
    public interface IRocketPackObject<T> : IEquatable<T>
    {
        public static IRocketPackFormatter<T> Formatter { get; }

        public static T Empty { get; }

        public static T Import(ReadOnlySequence<byte> sequence, IBytesPool bytesPool)
        {
            var reader = new RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }

        public void Export(IBufferWriter<byte> bufferWriter, IBytesPool bytesPool)
        {
            var writer = new RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, (T)this, 0);
        }
    }
}
