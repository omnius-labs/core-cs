using System.Buffers;

namespace Omnius.Core.Omnikit.Converters;

public interface IBytesToUtf8StringConverter
{
    bool TryEncode(ReadOnlySequence<byte> sequence, out byte[] text, bool includePrefix = false);
    bool TryDecode(ReadOnlySpan<byte> text, IBufferWriter<byte> bufferWriter);
}
