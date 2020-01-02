using System.IO;
using Omnius.Core;

namespace Omnius.Core.Serialization.RocketPack.Helpers
{
    public static class RocketPackHelper
    {
        public static T StreamToMessage<T>(Stream inStream)
            where T : IRocketPackMessage<T>
        {
            using var hub = new Hub(BufferPool<byte>.Shared);

            const int bufferSize = 4096;

            while (inStream.Position < inStream.Length)
            {
                var readLength = inStream.Read(hub.Writer.GetSpan(bufferSize));
                if (readLength < 0)
                {
                    break;
                }

                hub.Writer.Advance(readLength);
            }

            return IRocketPackMessage<T>.Import(hub.Reader.GetSequence(), BufferPool<byte>.Shared);
        }

        public static void MessageToStream<T>(T message, Stream stream)
            where T : IRocketPackMessage<T>
        {
            using var hub = new Hub(BufferPool<byte>.Shared);

            message.Export(hub.Writer, BufferPool<byte>.Shared);

            var sequence = hub.Reader.GetSequence();
            var position = sequence.Start;

            while (sequence.TryGet(ref position, out var memory))
            {
                stream.Write(memory.Span);
            }
        }
    }
}
