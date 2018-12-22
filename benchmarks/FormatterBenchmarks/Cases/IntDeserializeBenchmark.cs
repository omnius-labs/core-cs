using System;
using BenchmarkDotNet.Attributes;
using Omnix.Base;
using System.Buffers;
using FormatterBenchmarks.Internal;
using System.Collections.Generic;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class IntDeserializeBenchmark
    {
        private static byte[] _messagePack_Bytes;
        private static byte[] _rocketPack_Bytes;

        static IntDeserializeBenchmark()
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

                var list = new MessagePack_IntPropertiesListMessage()
                {
                    List = items.ToArray(),
                };

                _messagePack_Bytes = MessagePack.MessagePackSerializer.Serialize(list);
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

                var list = new RocketPack_IntPropertiesListMessage(items);

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
        public MessagePack_IntPropertiesListMessage MessagePack_IntPropertiesMessage_DeserializeTest()
        {
            return MessagePack.MessagePackSerializer.Deserialize<MessagePack_IntPropertiesListMessage>(_messagePack_Bytes);
        }

        [Benchmark]
        public RocketPack_IntPropertiesListMessage RocketPack_IntPropertiesMessage_DeserializeTest()
        {
           return RocketPack_IntPropertiesListMessage.Import(new ReadOnlySequence<byte>(_rocketPack_Bytes), BufferPool.Shared);
        }
    }
}
