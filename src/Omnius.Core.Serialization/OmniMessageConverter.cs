using System.Buffers;
using System.Buffers.Binary;
using System.IO.Compression;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Serialization;

public static class OmniMessageConverter
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public static bool TryWrite(ReadOnlySequence<byte> sequence, IBufferWriter<byte> writer)
    {
        Varint.SetUInt32((uint)FormatType.Version1, writer);
        Varint.SetUInt32((uint)CompressionAlgorithm.Brotli, writer);

        var reader = new SequenceReader<byte>(sequence);
        using var encoder = new BrotliEncoder(0, 10);
        var crc32 = default(Crc32_Castagnoli);

        for (; ; )
        {
            var source = reader.UnreadSpan;
            var destination = writer.GetSpan();
            var status = encoder.Compress(source, destination, out var bytesConsumed, out var bytesWritten, false);

            if (status == OperationStatus.InvalidData)
            {
                _logger.Warn("invalid data");
                return false;
            }

            reader.Advance(bytesConsumed);

            crc32.Compute(destination.Slice(0, bytesWritten));
            writer.Advance(bytesWritten);

            if (status == OperationStatus.Done)
            {
                break;
            }
        }

        BinaryPrimitives.WriteUInt32BigEndian(writer.GetSpan(4), crc32.GetResult());
        writer.Advance(4);

        return true;
    }

    public static bool TryRead(ReadOnlySequence<byte> sequence, IBufferWriter<byte> writer)
    {
        var reader = new SequenceReader<byte>(sequence);

        if (!Varint.TryGetUInt32(ref reader, out var formatTypeValue)) return false;
        var formatType = (FormatType)formatTypeValue;

        if (formatType == FormatType.Version1)
        {
            if (!Varint.TryGetUInt32(ref reader, out var compressionAlgorithmValue)) return false;
            var compressionAlgorithm = (CompressionAlgorithm)compressionAlgorithmValue;

            // Check CRC32
            var unreadSequence = reader.UnreadSequence;
            var crc32 = default(Crc32_Castagnoli);
            foreach (var buffer in unreadSequence.Slice(0, unreadSequence.Length - 4))
            {
                crc32.Compute(buffer.Span);
            }

            var decodedCrc32 = BinaryPrimitives.ReadUInt32BigEndian(unreadSequence.Slice(unreadSequence.Length - 4).ToArray());
            var computedCrc32 = crc32.GetResult();
            if (decodedCrc32 != computedCrc32) return false;

            reader = new SequenceReader<byte>(unreadSequence.Slice(0, unreadSequence.Length - 4));

            if (compressionAlgorithm == CompressionAlgorithm.Brotli)
            {
                using var decoder = default(BrotliDecoder);

                for (; ; )
                {
                    var source = reader.UnreadSpan;
                    var destination = writer.GetSpan();
                    var status = decoder.Decompress(source, destination, out var bytesConsumed, out var bytesWritten);

                    if (status == OperationStatus.InvalidData)
                    {
                        _logger.Warn("invalid data");
                        return false;
                    }

                    reader.Advance(bytesConsumed);
                    writer.Advance(bytesWritten);

                    if (status == OperationStatus.Done || (bytesConsumed == 0 && bytesWritten == 0))
                    {
                        return true;
                    }
                }
            }
        }

        _logger.Warn("not supported format");
        return false;
    }

    private enum FormatType : uint
    {
        None = 0,
        Version1 = 1,
    }

    private enum CompressionAlgorithm : uint
    {
        None = 0,
        Brotli = 1,
    }
}

public class OmniMessageConverterException : Exception
{
    public OmniMessageConverterException()
        : base()
    {
    }

    public OmniMessageConverterException(string? message)
        : base(message)
    {
    }

    public OmniMessageConverterException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
