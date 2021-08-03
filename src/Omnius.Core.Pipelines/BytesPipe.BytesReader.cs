using System.Buffers;

namespace Omnius.Core.Pipelines
{
    public partial class BytesPipe
    {
        public sealed class BytesReader : IBytesReader
        {
            private readonly BytesState _state;
            private long _position = 0;

            internal BytesReader(BytesState state)
            {
                _state = state;
            }

            internal void Reset()
            {
                _position = 0;
            }

            public long RemainBytes => _state.WrittenCount - _position;

            public void Advance(int count)
            {
                _position += count;
            }

            public ReadOnlySequence<byte> GetSequence()
            {
                return _state.GetSequence().Slice(_position);
            }
        }
    }
}
