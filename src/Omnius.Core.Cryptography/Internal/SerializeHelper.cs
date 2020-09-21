using System;
using System.Buffers;
using System.Collections.Generic;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Internal
{
    internal static class SerializeHelper
    {
        public static byte[] Encode(IEnumerable<ReadOnlyMemory<byte>> collection)
        {
            var hub = new BytesHub();

            {
                foreach (var value in collection)
                {
                    Varint.SetUInt64((uint)value.Length, hub.Writer);
                    BytesOperations.Copy(value.Span, hub.Writer.GetSpan(value.Length), value.Length);
                    hub.Writer.Advance(value.Length);
                }
            }

            var result = hub.Reader.GetSequence().ToArray();

            return result;
        }

        public static IEnumerable<byte[]> Decode(ReadOnlyMemory<byte> memory)
        {
            var list = new List<byte[]>();

            var sequence = new ReadOnlySequence<byte>(memory);
            var reader = new SequenceReader<byte>(sequence);

            while (reader.Remaining > 0)
            {
                if (!Varint.TryGetUInt64(ref reader, out var length))
                {
                    throw new FormatException();
                }

                var buffer = new byte[length];
                if (!reader.TryCopyTo(buffer))
                {
                    throw new FormatException();
                }
                reader.Advance((long)length);

                list.Add(buffer);
            }

            return list.ToArray();
        }
    }
}
