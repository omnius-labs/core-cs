using System;
using System.Buffers;
using System.IO.Compression;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Serialization
{
    public static class OmniMessageConverter
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private enum ConvertCompressionAlgorithm : uint
        {
            None = 0,
            Brotli = 1,
        }

        public static void Read(ReadOnlySequence<byte> sequence, out uint version, IBufferWriter<byte> writer)
        {
            var reader = new SequenceReader<byte>(sequence);

            if (!Varint.TryGetUInt32(ref reader, out version)) throw new Exception();
            if (!Varint.TryGetUInt32(ref reader, out var convertCompressionAlgorithmValue)) throw new Exception();
            var convertCompressionAlgorithm = (ConvertCompressionAlgorithm)convertCompressionAlgorithmValue;

            if (convertCompressionAlgorithm == ConvertCompressionAlgorithm.Brotli)
            {
                using var decoder = default(BrotliDecoder);

                for (; ; )
                {
                    var status = decoder.Decompress(reader.UnreadSpan, writer.GetSpan(), out var bytesConsumed, out var bytesWritten);

                    if (status == OperationStatus.InvalidData)
                    {
                        throw new OmniMessageConverterException("invalid data");
                    }

                    reader.Advance(bytesConsumed);
                    writer.Advance(bytesWritten);

                    if (status == OperationStatus.Done || (bytesConsumed == 0 && bytesWritten == 0))
                    {
                        break;
                    }
                }
            }
            else
            {
                throw new OmniMessageConverterException("not supported format");
            }
        }

        public static void Write(uint version, ReadOnlySequence<byte> sequence, IBufferWriter<byte> writer)
        {
            Varint.SetUInt32(version, writer);
            Varint.SetUInt32((uint)ConvertCompressionAlgorithm.Brotli, writer);

            using var encoder = new BrotliEncoder(0, 10);

            var reader = new SequenceReader<byte>(sequence);

            for (; ; )
            {
                var status = encoder.Compress(reader.UnreadSpan, writer.GetSpan(), out var bytesConsumed, out var bytesWritten, false);

                if (status == OperationStatus.InvalidData)
                {
                    throw new OmniMessageConverterException("invalid data");
                }

                reader.Advance(bytesConsumed);
                writer.Advance(bytesWritten);

                if (status == OperationStatus.Done)
                {
                    break;
                }
            }
        }
    }

    public class OmniMessageConverterException : Exception
    {
        public OmniMessageConverterException()
            : base()
        {
        }

        public OmniMessageConverterException(string message)
            : base(message)
        {
        }

        public OmniMessageConverterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
