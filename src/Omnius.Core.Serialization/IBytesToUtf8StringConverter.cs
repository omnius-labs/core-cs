using System;
using System.Buffers;

namespace Omnius.Core.Serialization
{
    public interface IBytesToUtf8StringConverter
    {
        bool TryEncode(ReadOnlySequence<byte> sequence, out byte[] text, bool includePrefix = false);

        bool TryDecode(ReadOnlySpan<byte> text, IBufferWriter<byte> bufferWriter);
    }
}
