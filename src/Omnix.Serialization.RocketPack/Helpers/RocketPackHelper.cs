using System.IO;
using Omnix.Base;
using Omnix.Io;

namespace Omnix.Serialization.RocketPack.Helpers
{
    public static class RocketPackHelper
    {
        public static T StreamToMessage<T>(Stream inStream)
            where T : RocketPackMessageBase<T>
        {
            using var hub = new Hub();

            const int bufferSize = 4096;

            while (inStream.Position < inStream.Length)
            {
                var readLength = inStream.Read(hub.Writer.GetSpan(bufferSize));
                if (readLength < 0) break;

                hub.Writer.Advance(readLength);
            }

            hub.Writer.Complete();

            return RocketPackMessageBase<T>.Import(hub.Reader.GetSequence(), BufferPool.Shared);
        }

        public static void MessageToStream<T>(T message, Stream stream)
            where T : RocketPackMessageBase<T>
        {
            using var hub = new Hub();

            message.Export(hub.Writer, BufferPool.Shared);
            hub.Writer.Complete();

            var sequence = hub.Reader.GetSequence();
            var position = sequence.Start;

            while (sequence.TryGet(ref position, out var memory))
            {
                stream.Write(memory.Span);
            }

            hub.Reader.Complete();
        }
    }
}
