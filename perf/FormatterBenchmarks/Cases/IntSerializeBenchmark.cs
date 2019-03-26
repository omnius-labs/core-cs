using BenchmarkDotNet.Attributes;
using Omnix.Base;
using FormatterBenchmarks.Internal;
using System.Collections.Generic;
using System;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class IntSerializeBenchmark
    {
        static MessagePack_IntPropertiesListMessage _messagePack_Message;
        static RocketPack_IntPropertiesListMessage _rocketPack_Message;

        static IntSerializeBenchmark()
        {
            {
                var random = new Random(0);

                var items = new List<MessagePack_IntPropertiesMessage>();
                for (int i = 0; i < 100000; i++)
                {
                    var message = new MessagePack_IntPropertiesMessage()
                    {
                        MyProperty1 = random.Next(),
                        MyProperty2 = random.Next(),
                        MyProperty3 = random.Next(),
                        MyProperty4 = random.Next(),
                        MyProperty5 = random.Next(),
                        MyProperty6 = random.Next(),
                        MyProperty7 = random.Next(),
                        MyProperty8 = random.Next(),
                        MyProperty9 = random.Next(),
                    };

                    items.Add(message);
                }

                _messagePack_Message = new MessagePack_IntPropertiesListMessage()
                {
                    List = items.ToArray(),
                };
            }

            {
                var random = new Random(0);

                var items = new List<RocketPack_IntPropertiesMessage>();
                for (int i = 0; i < 100000; i++)
                {
                    var message = new RocketPack_IntPropertiesMessage(
                        (uint)random.Next(),
                        (uint)random.Next(),
                        (uint)random.Next(),
                        (uint)random.Next(),
                        (uint)random.Next(),
                        (uint)random.Next(),
                        (uint)random.Next(),
                        (uint)random.Next(),
                        (uint)random.Next());

                    items.Add(message);
                }

                _rocketPack_Message = new RocketPack_IntPropertiesListMessage(items.ToArray());
            }
        }

        [Benchmark(Baseline = true)]
        public byte[] MessagePack_IntPropertiesMessage_SerializeTest()
        {
            return MessagePack.MessagePackSerializer.Serialize(_messagePack_Message);
        }

        [Benchmark]
        public Hub RocketPack_IntPropertiesMessage_SerializeTest()
        {
            var hub = new Hub();
            _rocketPack_Message.Export(hub.Writer, BufferPool.Shared);
            return hub;
        }
    }
}
