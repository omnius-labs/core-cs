using System;
using System.Buffers;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using FormatterBenchmarks.Internal;
using Omnix.Base;

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

            using (var hub = new Hub())
            {
                var random = new Random(0);
                var bufferPool = BufferPool.Shared;

                var elementsList = new List<OmniPack_BytesElements>();

                for (int i = 0; i < 10; i++)
                {
                    var X0 = bufferPool.Rent(random.Next(0, 1024 * 256));
                    var X1 = bufferPool.Rent(random.Next(0, 1024 * 256));
                    var X2 = bufferPool.Rent(random.Next(0, 1024 * 256));
                    var X3 = bufferPool.Rent(random.Next(0, 1024 * 256));
                    var X4 = bufferPool.Rent(random.Next(0, 1024 * 256));
                    var X5 = bufferPool.Rent(random.Next(0, 1024 * 256));
                    var X6 = bufferPool.Rent(random.Next(0, 1024 * 256));
                    var X7 = bufferPool.Rent(random.Next(0, 1024 * 256));
                    var X8 = bufferPool.Rent(random.Next(0, 1024 * 256));
                    var X9 = bufferPool.Rent(random.Next(0, 1024 * 256));

                    var elements = new OmniPack_BytesElements(X0, X1, X2, X3, X4, X5, X6, X7, X8, X9);
                    elementsList.Add(elements);
                }

                var message = new OmniPack_BytesElementsList(elementsList.ToArray());

                message.Export(hub.Writer, BufferPool.Shared);
                hub.Writer.Complete();

                _rocketPack_Bytes = new byte[hub.Writer.BytesWritten];
                hub.Reader.GetSequence().CopyTo(_rocketPack_Bytes);

                hub.Reader.Complete();
            }
        }

        [Benchmark(Baseline = true)]
        public object MessagePack_StringPropertiesMessage_DeserializeTest()
        {
            return MessagePack.MessagePackSerializer.Deserialize<MessagePack_BytesElementsList>(_messagePack_Bytes);
        }

        [Benchmark]
        public object OmniPack_StringPropertiesMessage_DeserializeTest()
        {
            return OmniPack_BytesElementsList.Import(new ReadOnlySequence<byte>(_rocketPack_Bytes), BufferPool.Shared);
        }
    }
}
