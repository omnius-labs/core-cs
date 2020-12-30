using System;
using System.Buffers;

namespace Omnius.Core
{
    public partial class BytesHub
    {
        public sealed class HubWriter : IBufferWriter<byte>
        {
            private readonly BufferWriter _bufferWriter;

            internal HubWriter(BufferWriter bufferWriter)
            {
                _bufferWriter = bufferWriter;
            }

            public void Reset()
            {
            }

            public long WrittenBytes => _bufferWriter.WrittenCount;

            public void Advance(int count)
            {
                _bufferWriter.Advance(count);
            }

            public Memory<byte> GetMemory(int sizeHint = 0)
            {
                return _bufferWriter.GetMemory(sizeHint);
            }

            public Span<byte> GetSpan(int sizeHint = 0)
            {
                return _bufferWriter.GetSpan(sizeHint);
            }
        }
    }
}
