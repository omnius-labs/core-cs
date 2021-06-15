using System;
using System.Buffers;
using Xunit;

namespace Omnius.Core.Serialization
{
    public class OmniMessageConverterTest
    {
        [Fact]
        public void WriteAndReadTest()
        {
            var random = new Random(0);
            using var inHub = new BytesHub();
            using var outHub = new BytesHub();

            var inVersion = (uint)random.Next();
            var inBody = new byte[random.Next(0, 1024 * 64)];
            random.NextBytes(inBody);

            Assert.True(TryEncode(inVersion, inBody, out var message));
            Assert.True(TryDecode(message!, out var outVersion, out var outBody));

            Assert.Equal(inVersion, outVersion);
            Assert.True(BytesOperations.Equals(inBody.AsSpan(), outBody.AsSpan()));
        }

        private static bool TryEncode(uint version, byte[] body, out byte[]? message)
        {
            message = null;
            using var hub = new BytesHub();
            if (!OmniMessageConverter.TryWrite(version, new ReadOnlySequence<byte>(body), hub.Writer)) return false;

            message = hub.Reader.GetSequence().ToArray();
            return true;
        }

        private static bool TryDecode(byte[] message, out uint version, out byte[]? body)
        {
            body = null;
            using var hub = new BytesHub();
            if (!OmniMessageConverter.TryRead(new ReadOnlySequence<byte>(message), out version, hub.Writer)) return false;

            body = hub.Reader.GetSequence().ToArray();
            return true;
        }
    }
}
