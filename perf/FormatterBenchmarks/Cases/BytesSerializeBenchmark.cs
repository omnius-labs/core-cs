using System;
using System.Buffers;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using FormatterBenchmarks.Internal;
using Omnius.Core;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class BytesSerializeBenchmark
    {
        private static readonly MessagePack_BytesElementsList _messagePack_Message;
        private static readonly RocketPack_BytesElementsList _rocketPack_Message;

        static BytesSerializeBenchmark()
        {
            {
                var random = new Random(0);

                var elementsList = new List<MessagePack_BytesElements>();

                for (int i = 0; i < 10; i++)
                {
                    var elements = new MessagePack_BytesElements()
                    {
                        X0 = new byte[random.Next(0, 1024 * 256)],
                        X1 = new byte[random.Next(0, 1024 * 256)],
                        X2 = new byte[random.Next(0, 1024 * 256)],
                        X3 = new byte[random.Next(0, 1024 * 256)],
                        X4 = new byte[random.Next(0, 1024 * 256)],
                        X5 = new byte[random.Next(0, 1024 * 256)],
                        X6 = new byte[random.Next(0, 1024 * 256)],
                        X7 = new byte[random.Next(0, 1024 * 256)],
                        X8 = new byte[random.Next(0, 1024 * 256)],
                        X9 = new byte[random.Next(0, 1024 * 256)],
                    };

                    elementsList.Add(elements);
                }

                _messagePack_Message = new MessagePack_BytesElementsList() { List = elementsList.ToArray() };
            }

            {
                var random = new Random(0);
                var bufferPool = BufferPool<byte>.Shared;

                using (var hub = new Hub(bufferPool))
                {
                    var elementsList = new List<RocketPack_BytesElements>();

                    for (int i = 0; i < 10; i++)
                    {
                        var X0 = bufferPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X1 = bufferPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X2 = bufferPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X3 = bufferPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X4 = bufferPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X5 = bufferPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X6 = bufferPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X7 = bufferPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X8 = bufferPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X9 = bufferPool.Memory.Rent(random.Next(0, 1024 * 256));

                        var elements = new RocketPack_BytesElements(X0, X1, X2, X3, X4, X5, X6, X7, X8, X9);
                        elementsList.Add(elements);
                    }

                    _rocketPack_Message = new RocketPack_BytesElementsList(elementsList.ToArray());
                }
            }
        }

        [Benchmark(Baseline = true)]
        public object MessagePack_BytesElementsList_SerializeTest()
        {
            return MessagePack.MessagePackSerializer.Serialize(_messagePack_Message);
        }

        [Benchmark]
        public object RocketPack_BytesElementsList_SerializeTest()
        {
            var writer = new ArrayBufferWriter<byte>();
            _rocketPack_Message.Export(writer, BufferPool<byte>.Shared);
            return writer;
        }
    }
}
