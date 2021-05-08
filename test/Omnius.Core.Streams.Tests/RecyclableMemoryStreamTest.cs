using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Omnius.Core.Streams
{
    public class RecyclableMemoryStreamTest
    {
        public static IEnumerable<object[]> GetRandomReadAndWriteTestCases()
        {
            var results = new List<(int, int, int)>{
                (0, 0, 0),
                (1, 1, 10),
                (2, 10, 100),
                (3, 100, 1000),
            };
            return results.Select(n => new object[] { n.Item1, n.Item2, n.Item3 });
        }

        [Theory]
        [MemberData(nameof(GetRandomReadAndWriteTestCases))]
        public void RandomReadAndWriteTest(int seed, int loopCount, int bufferLength)
        {
            var random = new Random(seed);
            var bytesPool = BytesPool.Shared;
            using var buffer1 = bytesPool.Memory.Rent(bufferLength);
            using var buffer2 = bytesPool.Memory.Rent(bufferLength);

            using var recyclableMemoryStream = new RecyclableMemoryStream(BytesPool.Shared);
            using var memoryStream = new MemoryStream();

            // Write
            foreach (var i in Enumerable.Range(0, loopCount))
            {
                random.NextBytes(buffer1.Memory.Span);
                var s = buffer1.Memory.Span.Slice(0, random.Next(0, buffer1.Memory.Length));
                recyclableMemoryStream.Write(s);
                memoryStream.Write(s);
            }

            Assert.Equal(recyclableMemoryStream.Position, memoryStream.Position);
            Assert.Equal(recyclableMemoryStream.Length, memoryStream.Length);

            recyclableMemoryStream.Seek(0, SeekOrigin.Begin);
            memoryStream.Seek(0, SeekOrigin.Begin);

            // Read
            while (memoryStream.Position < memoryStream.Length)
            {
                var length = random.Next(0, buffer1.Memory.Length);
                var s1 = buffer1.Memory.Span.Slice(0, length);
                var s2 = buffer1.Memory.Span.Slice(0, length);

                recyclableMemoryStream.Read(s1);
                memoryStream.Read(s2);

                Assert.Equal(recyclableMemoryStream.Position, memoryStream.Position);
                Assert.Equal(recyclableMemoryStream.Length, memoryStream.Length);
                Assert.True(BytesOperations.Equals(s1, s2));
            }

            using var m1 = recyclableMemoryStream.ToMemoryOwner();
            var m2 = memoryStream.ToArray();
            Assert.True(BytesOperations.Equals(m1.Memory.Span, m2));
        }
    }
}
