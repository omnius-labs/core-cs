using System;
using BenchmarkDotNet.Attributes;
using Omnix.Base;
using System.Buffers;
using FormatterBenchmarks.Internal;

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
                var message = new MessagePack_IntPropertiesMessage()
                {
                    MyProperty1 = 1,
                    MyProperty2 = 10,
                    MyProperty3 = 100,
                    MyProperty4 = 1000,
                    MyProperty5 = 10000,
                    MyProperty6 = 100000,
                    MyProperty7 = 1000000,
                    MyProperty8 = 10000000,
                    MyProperty9 = 100000000,
                };

                _messagePack_Bytes = MessagePack.MessagePackSerializer.Serialize(message);
            }

            {
                var message = new RocketPack_IntPropertiesMessage(1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000);

                var hub = new Hub();

                message.Export(hub.Writer, BufferPool.Shared);
                hub.Writer.Complete();

                _rocketPack_Bytes = new byte[hub.Writer.BytesWritten];
                hub.Reader.GetSequence().CopyTo(_rocketPack_Bytes);

                hub.Reader.Complete();
                hub.Reset();
            }
        }

        [Benchmark(Baseline = true)]
        public void MessagePack_IntPropertiesMessage_DeserializeTest()
        {
            MessagePack.MessagePackSerializer.Deserialize<MessagePack_IntPropertiesMessage>(_messagePack_Bytes);
        }

        [Benchmark]
        public void RocketPack_IntPropertiesMessage_DeserializeTest()
        {
            RocketPack_IntPropertiesMessage.Import(new ReadOnlySequence<byte>(_rocketPack_Bytes), BufferPool.Shared);
        }
    }
}
