using System.IO;

namespace Omnius.Core.RocketPack.Helpers
{
    public static class RocketPackHelper
    {
        public static T StreamToMessage<T>(Stream inStream)
            where T : IRocketPackObject<T>
        {
            using var hub = new BytesHub();

            const int bufferSize = 4096;

            while (inStream.Position < inStream.Length)
            {
                var readLength = inStream.Read(hub.Writer.GetSpan(bufferSize));
                if (readLength < 0) break;

                hub.Writer.Advance(readLength);
            }

            return IRocketPackObject<T>.Import(hub.Reader.GetSequence(), BytesPool.Shared);
        }

        public static void MessageToStream<T>(T message, Stream stream)
            where T : IRocketPackObject<T>
        {
            using var hub = new BytesHub();

            message.Export(hub.Writer, BytesPool.Shared);

            var sequence = hub.Reader.GetSequence();
            var position = sequence.Start;

            while (sequence.TryGet(ref position, out var memory))
            {
                stream.Write(memory.Span);
            }
        }
    }
}
