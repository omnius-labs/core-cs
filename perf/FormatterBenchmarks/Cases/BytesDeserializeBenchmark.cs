using System;
using System.Buffers;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using FormatterBenchmarks.Internal;
using Omnius.Core;

namespace FormatterBenchmarks.Cases
{
    [Config(typeof(BenchmarkConfig))]
    public class BytesDeserializeBenchmark
    {
        private static readonly byte[] _messagePack_Bytes;
        private static readonly byte[] _rocketPack_Bytes;

        static BytesDeserializeBenchmark()
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

                _messagePack_Bytes = MessagePack.MessagePackSerializer.Serialize(new MessagePack_BytesElementsList() { List = elementsList.ToArray() });
            }

            {
                var bytesPool = BytesPool.Shared;
                var random = new Random(0);
                using (var bytesPipe = new BytesPipe(bytesPool))
                {

                    var elementsList = new List<RocketPack_BytesElements>();

                    for (int i = 0; i < 10; i++)
                    {
                        var X0 = bytesPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X1 = bytesPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X2 = bytesPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X3 = bytesPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X4 = bytesPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X5 = bytesPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X6 = bytesPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X7 = bytesPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X8 = bytesPool.Memory.Rent(random.Next(0, 1024 * 256));
                        var X9 = bytesPool.Memory.Rent(random.Next(0, 1024 * 256));

                        var elements = new RocketPack_BytesElements(X0, X1, X2, X3, X4, X5, X6, X7, X8, X9);
                        elementsList.Add(elements);
                    }

                    var message = new RocketPack_BytesElementsList(elementsList.ToArray());

                    message.Export(bytesPipe.Writer, BytesPool.Shared);

                    _rocketPack_Bytes = new byte[bytesPipe.Writer.WrittenBytes];
                    bytesPipe.Reader.GetSequence().CopyTo(_rocketPack_Bytes);
                }
            }
        }

        [Benchmark(Baseline = true)]
        public object MessagePack_BytesElementsList_DeserializeTest()
        {
            return MessagePack.MessagePackSerializer.Deserialize<MessagePack_BytesElementsList>(_messagePack_Bytes);
        }

        [Benchmark]
        public object RocketPack_BytesElementsList_DeserializeTest()
        {
            return RocketPack_BytesElementsList.Import(new ReadOnlySequence<byte>(_rocketPack_Bytes), BytesPool.Shared);
        }
    }
}
