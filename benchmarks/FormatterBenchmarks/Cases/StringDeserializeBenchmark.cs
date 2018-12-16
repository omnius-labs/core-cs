using System;
using BenchmarkDotNet.Attributes;
using Omnix.Base;
using System.Buffers;
using FormatterBenchmarks.Internal;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class StringDeserializeBenchmark
    {
        private static byte[] _messagePack_Bytes;
        private static byte[] _rocketPack_Bytes;

        static StringDeserializeBenchmark()
        {
            {
                var message = new MessagePack_StringPropertiesMessage()
                {
                    MyProperty1 = "0",
                    MyProperty2 = "0000000000",
                    MyProperty3 = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                };

                _messagePack_Bytes = MessagePack.MessagePackSerializer.Serialize(message);
            }

            {
                var message = new RocketPack_StringPropertiesMessage("0", "0000000000", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");

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
        public void MessagePack_StringPropertiesMessage_DeserializeTest()
        {
            MessagePack.MessagePackSerializer.Deserialize<MessagePack_StringPropertiesMessage>(_messagePack_Bytes);
        }

        [Benchmark]
        public void RocketPack_StringPropertiesMessage_DeserializeTest()
        {
            RocketPack_StringPropertiesMessage.Import(new ReadOnlySequence<byte>(_rocketPack_Bytes), BufferPool.Shared);
        }
    }
}
