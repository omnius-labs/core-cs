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
    public class BytesSerializeBenchmark
    {
        private static MessagePack_BytesMessage _messagePack_Message;
        private static RocketPack_BytesMessage _rocketPack_Message;

        static BytesSerializeBenchmark()
        {
            {
                _messagePack_Message = new MessagePack_BytesMessage()
                {
                    Bytes = new byte[1024 * 1024],
                };
            }

            {
                var memoryOwner = BufferPool.Create().Rent(1024 * 1024);
                _rocketPack_Message = new RocketPack_BytesMessage(memoryOwner);
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
