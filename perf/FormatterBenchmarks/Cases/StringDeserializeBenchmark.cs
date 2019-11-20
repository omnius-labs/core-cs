using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;
using FormatterBenchmarks.Internal;
using Omnius.Core;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class StringDeserializeBenchmark
    {
        private static readonly byte[] _messagePack_Bytes;
        private static readonly byte[] _rocketPack_Bytes;

        static StringDeserializeBenchmark()
        {
            var charList = new char[] { 'A', 'B', 'C', 'D', 'E', '安', '以', '宇', '衣', '於' };

            string GetRandomString(Random random)
            {
                var sb = new StringBuilder();

                for (int i = random.Next(32, 256) - 1; i >= 0; i--)
                {
                    sb.Append(charList[random.Next(0, charList.Length)]);
                }

                return sb.ToString();
            }

            {
                var random = new Random(0);

                var elementsList = new List<MessagePack_StringElements>();

                for (int i = 0; i < 32 * 1024; i++)
                {
                    var elements = new MessagePack_StringElements()
                    {
                        X0 = GetRandomString(random),
                        X1 = GetRandomString(random),
                        X2 = GetRandomString(random),
                        X3 = GetRandomString(random),
                        X4 = GetRandomString(random),
                        X5 = GetRandomString(random),
                        X6 = GetRandomString(random),
                        X7 = GetRandomString(random),
                        X8 = GetRandomString(random),
                        X9 = GetRandomString(random),
                    };

                    elementsList.Add(elements);
                }

                _messagePack_Bytes = MessagePack.MessagePackSerializer.Serialize(new MessagePack_StringElementsList() { List = elementsList.ToArray() });
            }

            using (var hub = new Hub())
            {
                var random = new Random(0);
                var bufferPool = BufferPool<byte>.Shared;

                var elementsList = new List<RocketPack_StringElements>();

                for (int i = 0; i < 32 * 1024; i++)
                {
                    var X0 = GetRandomString(random);
                    var X1 = GetRandomString(random);
                    var X2 = GetRandomString(random);
                    var X3 = GetRandomString(random);
                    var X4 = GetRandomString(random);
                    var X5 = GetRandomString(random);
                    var X6 = GetRandomString(random);
                    var X7 = GetRandomString(random);
                    var X8 = GetRandomString(random);
                    var X9 = GetRandomString(random);

                    var elements = new RocketPack_StringElements(X0, X1, X2, X3, X4, X5, X6, X7, X8, X9);
                    elementsList.Add(elements);
                }

                var message = new RocketPack_StringElementsList(elementsList.ToArray());

                message.Export(hub.Writer, BufferPool<byte>.Shared);
                hub.Writer.Complete();

                _rocketPack_Bytes = new byte[hub.Writer.BytesWritten];
                hub.Reader.GetSequence().CopyTo(_rocketPack_Bytes);

                hub.Reader.Complete();
            }
        }

        [Benchmark(Baseline = true)]
        public object MessagePack_StringPropertiesMessage_DeserializeTest()
        {
            return MessagePack.MessagePackSerializer.Deserialize<MessagePack_StringElementsList>(_messagePack_Bytes);
        }

        [Benchmark]
        public object RocketPack_StringPropertiesMessage_DeserializeTest()
        {
            return RocketPack_StringElementsList.Import(new ReadOnlySequence<byte>(_rocketPack_Bytes), BufferPool<byte>.Shared);
        }
    }
}
