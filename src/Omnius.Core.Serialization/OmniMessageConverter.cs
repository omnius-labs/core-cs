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
        using var encoder = new BrotliEncoder(11, 24);
        var crc32 = new Crc32_Castagnoli();

        var crc32Buffer = writer.GetSpan(4).Slice(0, 4);
        writer.Advance(crc32Buffer.Length);

        for (; ; )
        {
            var source = reader.UnreadSpan;
            var destination = writer.GetSpan();
            var status = encoder.Compress(source, destination, out var consumed, out var written, false);
            if (status == OperationStatus.InvalidData)
            {
                _logger.Warn("invalid data");
                return false;
            }

            reader.Advance(consumed);

            crc32.Compute(destination.Slice(0, written));
            writer.Advance(written);

            if (status == OperationStatus.Done) break;
        }

        for (; ; )
        {
            var destination = writer.GetSpan();
            var status = encoder.Compress(ReadOnlySpan<byte>.Empty, destination, out _, out var written, true);
            if (status == OperationStatus.InvalidData)
            {
                _logger.Warn("invalid data");
                return false;
            }

            writer.Advance(written);
            crc32.Compute(destination.Slice(0, written));

            if (written == 0) break;
        }

        BinaryPrimitives.WriteUInt32BigEndian(crc32Buffer, crc32.GetResult());

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

            var unreadSequence = reader.UnreadSequence;

            // Check CRC32
            var decodedCrc32 = BinaryPrimitives.ReadUInt32BigEndian(unreadSequence.Slice(0, 4).ToArray());
            var crc32 = new Crc32_Castagnoli();
            foreach (var buffer in unreadSequence.Slice(4, unreadSequence.Length - 4))
            {
                crc32.Compute(buffer.Span);
            }

            var computedCrc32 = crc32.GetResult();
            if (decodedCrc32 != computedCrc32) return false;

            reader = new SequenceReader<byte>(unreadSequence.Slice(4, unreadSequence.Length - 4));

            if (compressionAlgorithm == CompressionAlgorithm.Brotli)
            {
                using var decoder = new BrotliDecoder();

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

                    if (bytesWritten == 0) return true;
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
    public OmniMessageConverterException() : base() { }
    public OmniMessageConverterException(string? message) : base(message) { }
    public OmniMessageConverterException(string? message, Exception? innerException) : base(message, innerException) { }
}
