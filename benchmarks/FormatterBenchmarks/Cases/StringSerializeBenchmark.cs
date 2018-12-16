using BenchmarkDotNet.Attributes;
using Omnix.Base;
using FormatterBenchmarks.Internal;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class StringSerializeBenchmark
    {
        static MessagePack_StringPropertiesMessage _messagePack_Message;
        static RocketPack_StringPropertiesMessage _rocketPack_Message;

        static StringSerializeBenchmark()
        {
            _messagePack_Message = new MessagePack_StringPropertiesMessage()
            {
                MyProperty1 = "0",
                MyProperty2 = "0000000000",
                MyProperty3 = "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
            };

            _rocketPack_Message = new RocketPack_StringPropertiesMessage("0", "0000000000", "0000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000");
        }

        [Benchmark(Baseline = true)]
        public void MessagePack_StringPropertiesMessage_SerializeTest()
        {
            MessagePack.MessagePackSerializer.Serialize<MessagePack_StringPropertiesMessage>(_messagePack_Message);
        }

        [Benchmark]
        public void RocketPack_StringPropertiesMessage_SerializeTest()
        {
            var hub = new Hub();
            _rocketPack_Message.Export(hub.Writer, BufferPool.Shared);
        }
    }
}
