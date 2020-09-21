using System.Buffers;

namespace Omnius.Core
{
    public partial class BytesHub
    {
        public sealed class HubReader
        {
            private BufferWriter _bufferWriter;
            private long _position = 0;

            internal HubReader(BufferWriter bufferWriter)
            {
                _bufferWriter = bufferWriter;
            }

            public void Reset()
            {
                _position = 0;
            }

            public long RemainBytes => _bufferWriter.WrittenCount - _position;

            public void Advance(int count)
            {
                _position += count;
            }

            public ReadOnlySequence<byte> GetSequence()
            {
                return _bufferWriter.GetSequence().Slice(_position);
            }
        }
    }
}
