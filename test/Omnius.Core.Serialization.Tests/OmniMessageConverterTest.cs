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

            OmniMessageConverter.TryWrite(inVersion, new ReadOnlySequence<byte>(inBody), inHub.Writer);
            OmniMessageConverter.TryRead(inHub.Reader.GetSequence(), out var outVersion, outHub.Writer);

            var outBody = outHub.Reader.GetSequence().ToArray();
            Assert.True(BytesOperations.Equals(inBody.AsSpan(), outBody.AsSpan()));
        }
    }
}
