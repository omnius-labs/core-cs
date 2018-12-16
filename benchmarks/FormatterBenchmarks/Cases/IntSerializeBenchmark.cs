using BenchmarkDotNet.Attributes;
using Omnix.Base;
using FormatterBenchmarks.Internal;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class IntSerializeBenchmark
    {
        static MessagePack_IntPropertiesMessage _messagePack_Message;
        static RocketPack_IntPropertiesMessage _rocketPack_Message;

        static IntSerializeBenchmark()
        {
            _messagePack_Message = new MessagePack_IntPropertiesMessage()
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

            _rocketPack_Message = new RocketPack_IntPropertiesMessage(1, 10, 100, 1000, 10000, 100000, 1000000, 10000000, 100000000);
        }

        [Benchmark(Baseline = true)]
        public void MessagePack_IntPropertiesMessage_SerializeTest()
        {
            MessagePack.MessagePackSerializer.Serialize<MessagePack_IntPropertiesMessage>(_messagePack_Message);
        }

        [Benchmark]
        public void RocketPack_IntPropertiesMessage_SerializeTest()
        {
            var hub = new Hub();
            _rocketPack_Message.Export(hub.Writer, BufferPool.Shared);
        }
    }
}
