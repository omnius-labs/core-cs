using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using Omnix.Base;
using Omnix.Serialization;
using System.Buffers;

namespace Omnix.Cryptography.Internal
{
    static class SerializeHelper
    {
        public static byte[] Encode(IEnumerable<ReadOnlyMemory<byte>> collection)
        {
            var pipe = new Pipe();

            {
                foreach (var value in collection)
                {
                    Varint.SetUInt64((uint)value.Length, pipe.Writer);
                    BytesOperations.Copy(value.Span, pipe.Writer.GetSpan(value.Length), value.Length);
                    pipe.Writer.Advance(value.Length);
                }

                pipe.Writer.Complete();
            }

            byte[] result;
            {
                pipe.Reader.TryRead(out var readResult);

                result = readResult.Buffer.ToArray();

                pipe.Reader.Complete();
            }

            return result;
        }

        public static IEnumerable<byte[]> Decode(ReadOnlyMemory<byte> memory)
        {
            var list = new List<byte[]>();

            var sequence = new ReadOnlySequence<byte>(memory);
            var position = sequence.GetPosition(0);

            while (sequence.Length > 0)
            {
                if (!Varint.TryGetUInt64(sequence, out var length, out position)) throw new FormatException();
                sequence = sequence.Slice(position);

                list.Add(sequence.Slice(0, (int)length).ToArray());
                sequence = sequence.Slice((long)length);
            }

            return list.ToArray();
        }
    }
}
