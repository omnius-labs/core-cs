using System;
using System.Buffers;
using System.IO.Pipelines;

namespace Omnius.Core
{
    public partial class Hub
    {
        public sealed class HubReader
        {
            private readonly Pipe _pipe;
            private readonly PipeReader _pipeReader;
            private ReadOnlySequence<byte>? _sequence;
            private long _position = 0;
            private bool _isCompleted = false;

            internal HubReader(Pipe pipe)
            {
                _pipe = pipe;
                _pipeReader = pipe.Reader;
            }

            public long BytesConsumed => _position;
            public bool IsCompleted => _isCompleted;

            public void Advance(int count)
            {
                _position += count;
            }

            public ReadOnlySequence<byte> GetSequence()
            {
                if (_sequence == null)
                {
                    if (!_pipeReader.TryRead(out var readResult))
                    {
                        throw new HubReaderException("Read failed.");
                    }

                    _sequence = readResult.Buffer;
                }

                return _sequence.Value.Slice(_position);
            }

            public void Complete()
            {
                _pipeReader.Complete();
                _isCompleted = true;
            }

            internal void Reset()
            {
                _sequence = null;
                _position = 0;
                _isCompleted = false;
            }
        }
    }

    public sealed class HubReaderException : Exception
    {
        public HubReaderException() { }
        public HubReaderException(string message) : base(message) { }
        public HubReaderException(string message, Exception innerException) : base(message, innerException) { }
    }
}
