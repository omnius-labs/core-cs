using System;
using System.Buffers;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using FormatterBenchmarks.Internal;
using Omnix.Base;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class IntSerializeBenchmark
    {
        private static readonly MessagePack_IntElementsList _messagePack_Message;
        private static readonly RocketPack_IntElementsList _rocketPack_Message;

        static IntSerializeBenchmark()
        {
            {
                var random = new Random(0);

                var elementsList = new List<MessagePack_IntElements>();

                for (int i = 0; i < 32 * 1024; i++)
                {
                    var elements = new MessagePack_IntElements()
                    {
                        X0 = (uint)random.Next(0, 1024 * 256),
                        X1 = (uint)random.Next(0, 1024 * 256),
                        X2 = (uint)random.Next(0, 1024 * 256),
                        X3 = (uint)random.Next(0, 1024 * 256),
                        X4 = (uint)random.Next(0, 1024 * 256),
                        X5 = (uint)random.Next(0, 1024 * 256),
                        X6 = (uint)random.Next(0, 1024 * 256),
                        X7 = (uint)random.Next(0, 1024 * 256),
                        X8 = (uint)random.Next(0, 1024 * 256),
                        X9 = (uint)random.Next(0, 1024 * 256),
                    };

                    elementsList.Add(elements);
                }

                _messagePack_Message = new MessagePack_IntElementsList() { List = elementsList.ToArray() };
            }

            using (var hub = new Hub())
            {
                var random = new Random(0);
                var bufferPool = BufferPool.Shared;

                var elementsList = new List<RocketPack_IntElements>();

                for (int i = 0; i < 32 * 1024; i++)
                {
                    var X0 = (uint)random.Next(0, 1024 * 256);
                    var X1 = (uint)random.Next(0, 1024 * 256);
                    var X2 = (uint)random.Next(0, 1024 * 256);
                    var X3 = (uint)random.Next(0, 1024 * 256);
                    var X4 = (uint)random.Next(0, 1024 * 256);
                    var X5 = (uint)random.Next(0, 1024 * 256);
                    var X6 = (uint)random.Next(0, 1024 * 256);
                    var X7 = (uint)random.Next(0, 1024 * 256);
                    var X8 = (uint)random.Next(0, 1024 * 256);
                    var X9 = (uint)random.Next(0, 1024 * 256);

                    var elements = new RocketPack_IntElements(X0, X1, X2, X3, X4, X5, X6, X7, X8, X9);
                    elementsList.Add(elements);
                }

                _rocketPack_Message = new RocketPack_IntElementsList(elementsList.ToArray());
            }
        }

        [Benchmark(Baseline = true)]
        public object MessagePack_IntPropertiesMessage_SerializeTest()
        {
            return MessagePack.MessagePackSerializer.Serialize(_messagePack_Message);
        }

        [Benchmark]
        public object RocketPack_IntPropertiesMessage_SerializeTest()
        {
            var writer = new ArrayBufferWriter<byte>();
            _rocketPack_Message.Export(writer, BufferPool.Shared);
            return writer;
        }
    }
}
