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
    public class BytesDeserializeBenchmark
    {
        private static byte[] _messagePack_Bytes;
        private static byte[] _rocketPack_Bytes;

        static BytesDeserializeBenchmark()
        {
            {
                var message = new MessagePack_BytesMessage()
                {
                    Bytes = new byte[1024 * 1024],
                };

                _messagePack_Bytes = MessagePack.MessagePackSerializer.Serialize(message);
            }

            using (var hub = new Hub())
            {
                var memoryOwner = BufferPool.Create().Rent(1024 * 1024);
                var message = new RocketPack_BytesMessage(memoryOwner);

                message.Export(hub.Writer, BufferPool.Shared);
                hub.Writer.Complete();

                _rocketPack_Bytes = new byte[hub.Writer.BytesWritten];
                hub.Reader.GetSequence().CopyTo(_rocketPack_Bytes);

                hub.Reader.Complete();
            }
        }

        [Benchmark(Baseline = true)]
        public MessagePack_BytesMessage MessagePack_StringPropertiesMessage_DeserializeTest()
        {
            return MessagePack.MessagePackSerializer.Deserialize<MessagePack_BytesMessage>(_messagePack_Bytes);
        }

        [Benchmark]
        public RocketPack_BytesMessage RocketPack_StringPropertiesMessage_DeserializeTest()
        {
            return RocketPack_BytesMessage.Import(new ReadOnlySequence<byte>(_rocketPack_Bytes), BufferPool.Shared);
        }
    }
}
