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

                var elementsList = new List<MessagePack_IntElements>();

                for (int i = 0; i < 32; i++)
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

                _messagePack_Bytes = MessagePack.MessagePackSerializer.Serialize(new MessagePack_IntElementsList() { List = elementsList.ToArray() });
            }

            using (var hub = new Hub())
            {
                var random = new Random(0);
                var bufferPool = BufferPool.Shared;

                var elementsList = new List<RocketPack_IntElements>();

                for (int i = 0; i < 32; i++)
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

                var message = new RocketPack_IntElementsList(elementsList.ToArray());

                message.Export(hub.Writer, BufferPool.Shared);
                hub.Writer.Complete();

                _rocketPack_Bytes = new byte[hub.Writer.BytesWritten];
                hub.Reader.GetSequence().CopyTo(_rocketPack_Bytes);

                hub.Reader.Complete();
            }
        }

        [Benchmark(Baseline = true)]
        public object MessagePack_IntPropertiesMessage_DeserializeTest()
        {
            return MessagePack.MessagePackSerializer.Deserialize<MessagePack_IntElementsList>(_messagePack_Bytes);
        }

        [Benchmark]
        public object RocketPack_IntPropertiesMessage_DeserializeTest()
        {
           return RocketPack_IntElementsList.Import(new ReadOnlySequence<byte>(_rocketPack_Bytes), BufferPool.Shared);
        }
    }
}
