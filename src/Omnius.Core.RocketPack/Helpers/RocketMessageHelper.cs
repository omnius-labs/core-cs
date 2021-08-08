using System.IO;
using Omnius.Core.Pipelines;

namespace Omnius.Core.RocketPack.Helpers
{
    public static class RocketMessageHelper
    {
        public static T FromStream<T>(Stream inStream)
            where T : IRocketMessage<T>
        {
            using var bytesPipe = new BytesPipe();

            const int bufferSize = 4096;

            while (inStream.Position < inStream.Length)
            {
                var readLength = inStream.Read(bytesPipe.Writer.GetSpan(bufferSize));
                if (readLength < 0) break;

                bytesPipe.Writer.Advance(readLength);
            }

            return IRocketMessage<T>.Import(bytesPipe.Reader.GetSequence(), BytesPool.Shared);
        }

        public static void ToStream<T>(T message, Stream stream)
            where T : IRocketMessage<T>
        {
            using var bytesPipe = new BytesPipe();

            message.Export(bytesPipe.Writer, BytesPool.Shared);

            var sequence = bytesPipe.Reader.GetSequence();
            var position = sequence.Start;

            while (sequence.TryGet(ref position, out var memory))
            {
                stream.Write(memory.Span);
            }
        }
    }
}
