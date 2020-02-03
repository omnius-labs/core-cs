using System;
using System.Buffers;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using FormatterBenchmarks.Internal;
using Omnius.Core;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class IntDeserializeBenchmark
    {
        private static readonly byte[] _messagePack_Bytes;
        private static readonly byte[] _rocketPack_Bytes;

        static IntDeserializeBenchmark()
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

                _messagePack_Bytes = MessagePack.MessagePackSerializer.Serialize(new MessagePack_IntElementsList() { List = elementsList.ToArray() });
            }

            {
                var random = new Random(0);
                var bytesPool = BytesPool.Shared;

                using (var hub = new Hub(bytesPool))
                {
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

                    var message = new RocketPack_IntElementsList(elementsList.ToArray());

                    message.Export(hub.Writer, BytesPool.Shared);

                    _rocketPack_Bytes = new byte[hub.Writer.WrittenBytes];
                    hub.Reader.GetSequence().CopyTo(_rocketPack_Bytes);
                }
            }
        }

        [Benchmark(Baseline = true)]
        public object MessagePack_IntElementsList_DeserializeTest()
        {
            return MessagePack.MessagePackSerializer.Deserialize<MessagePack_IntElementsList>(_messagePack_Bytes);
        }

        [Benchmark]
        public object RocketPack_IntElementsList_DeserializeTest()
        {
            return RocketPack_IntElementsList.Import(new ReadOnlySequence<byte>(_rocketPack_Bytes), BytesPool.Shared);
        }
    }
}
