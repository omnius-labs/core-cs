namespace Core.Pipelines;

public sealed partial class BytesPipe
{
    public sealed class BytesWriter : IBytesWriter
    {
        private readonly BytesState _bufferState;

        internal BytesWriter(BytesState bufferState)
        {
            _bufferState = bufferState;
        }

        internal void Reset()
        {
            _bufferState.Reset();
        }

        public long WrittenBytes => _bufferState.WrittenBytes;

        public void Advance(int count)
        {
            _bufferState.Advance(count);
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            return _bufferState.GetMemory(sizeHint);
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            return _bufferState.GetSpan(sizeHint);
        }
    }
}
