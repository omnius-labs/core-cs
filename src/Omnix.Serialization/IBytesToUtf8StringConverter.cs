using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Omnix.Serialization
{
    public interface IBytesToUtf8StringConverter
    {
        bool TryEncode(ReadOnlySequence<byte> sequence, out ReadOnlyMemory<byte> text, bool includePrefix = false);
        bool TryDecode(ReadOnlySpan<byte> text, IBufferWriter<byte> bufferWriter);
    }
}
