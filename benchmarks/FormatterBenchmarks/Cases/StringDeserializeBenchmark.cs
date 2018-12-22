using System;
using BenchmarkDotNet.Attributes;
using Omnix.Base;
using System.Buffers;
using FormatterBenchmarks.Internal;
using System.Collections.Generic;
using System.Text;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class StringDeserializeBenchmark
    {
        private static byte[] _messagePack_Bytes;
        private static byte[] _rocketPack_Bytes;

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

                var items = new List<MessagePack_StringPropertiesMessage>();
                for (int i = 0; i < 100000; i++)
                {
                    var message = new MessagePack_StringPropertiesMessage()
                    {
                        MyProperty1 = GetRandomString(random),
                        MyProperty2 = GetRandomString(random),
                        MyProperty3 = GetRandomString(random),
                    };
                    items.Add(message);
                }

                var list = new MessagePack_StringPropertiesListMessage()
                {
                    List = items.ToArray(),
                };

                _messagePack_Bytes = MessagePack.MessagePackSerializer.Serialize(list);
            }

            {
                var random = new Random(0);

                var items = new List<RocketPack_StringPropertiesMessage>();
                for (int i = 0; i < 100000; i++)
                {
                    var message = new RocketPack_StringPropertiesMessage(
                         GetRandomString(random),
                         GetRandomString(random),
                         GetRandomString(random));
                    items.Add(message);
                }

                var list = new RocketPack_StringPropertiesListMessage(items);

                var hub = new Hub();

                list.Export(hub.Writer, BufferPool.Shared);
                hub.Writer.Complete();

                _rocketPack_Bytes = new byte[hub.Writer.BytesWritten];
                hub.Reader.GetSequence().CopyTo(_rocketPack_Bytes);

                hub.Reader.Complete();
                hub.Reset();
            }
        }

        [Benchmark(Baseline = true)]
        public MessagePack_StringPropertiesListMessage MessagePack_StringPropertiesMessage_DeserializeTest()
        {
            return MessagePack.MessagePackSerializer.Deserialize<MessagePack_StringPropertiesListMessage>(_messagePack_Bytes);
        }

        [Benchmark]
        public RocketPack_StringPropertiesListMessage RocketPack_StringPropertiesMessage_DeserializeTest()
        {
            return RocketPack_StringPropertiesListMessage.Import(new ReadOnlySequence<byte>(_rocketPack_Bytes), BufferPool.Shared);
        }
    }
}
