using BenchmarkDotNet.Attributes;
using Omnix.Base;
using FormatterBenchmarks.Internal;
using System.Collections.Generic;
using System;
using System.Text;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class StringSerializeBenchmark
    {
        static MessagePack_StringPropertiesListMessage _messagePack_Message;
        static RocketPack_StringPropertiesListMessage _rocketPack_Message;

        static StringSerializeBenchmark()
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

                _messagePack_Message = new MessagePack_StringPropertiesListMessage()
                {
                    List = items.ToArray(),
                };
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

                _rocketPack_Message = new RocketPack_StringPropertiesListMessage(items.ToArray());
            }
        }

        [Benchmark(Baseline = true)]
        public byte[] MessagePack_StringPropertiesMessage_SerializeTest()
        {
            return MessagePack.MessagePackSerializer.Serialize(_messagePack_Message);
        }

        [Benchmark]
        public Hub RocketPack_StringPropertiesMessage_SerializeTest()
        {
            var hub = new Hub();
            _rocketPack_Message.Export(hub.Writer, BufferPool.Shared);
            return hub;
        }
    }
}
