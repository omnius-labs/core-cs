using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using Omnix.Base;
using Omnix.Base.Extensions;

namespace Omnix.Serialization.Extensions
{
    public static class BytesToUtf8StringConverterExtension
    {
        private static readonly Lazy<UTF8Encoding> _utf8Encoding = new Lazy<UTF8Encoding>(() => new UTF8Encoding(false));

        public static bool TryEncode(this IBytesToUtf8StringConverter converter, ReadOnlySequence<byte> sequence, out string text, bool includePrefix = false)
        {
            if (!converter.TryEncode(sequence, out var utf8string, includePrefix)) throw new FormatException(nameof(sequence));

            text = _utf8Encoding.Value.GetString(utf8string.Span);

            return true;
        }

        public static bool TryDecode(this IBytesToUtf8StringConverter converter, string text, IBufferWriter<byte> bufferWriter)
        {
            using (var recyclableMemory = BufferPool.Shared.Rent(_utf8Encoding.Value.GetMaxByteCount(text.Length)))
            {
                var length = _utf8Encoding.Value.GetBytes(text, recyclableMemory.Memory.Span);

                if (!converter.TryDecode(recyclableMemory.Memory.Span.Slice(0, length), bufferWriter)) throw new FormatException(nameof(text));
            }

            return true;
        }
    }
}
