using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Omnix.Serialization
{
    public interface IBytesToUtf8StringConverter
    {
        bool TryEncode(ReadOnlySpan<byte> span, out byte[] text, bool includePrefix = false);
        bool TryEncode(ReadOnlySequence<byte> sequence, out byte[] text, bool includePrefix = false);
        bool TryDecode(ReadOnlySpan<byte> text, IBufferWriter<byte> bufferWriter);
    }
}
