using System;
using System.Buffers;
using System.IO.Pipelines;

namespace Omnius.Core
{
    public partial class Hub
    {
        public sealed class HubWriter : IBufferWriter<byte>
        {
            private readonly PipeWriter _pipeWriter;
            private long _bytesWritten = 0;
            private bool _isCompleted = false;

            internal HubWriter(Pipe pipe)
            {
                _pipeWriter = pipe.Writer;
            }

            public long BytesWritten => _bytesWritten;
            public bool IsCompleted => _isCompleted;

            public void Advance(int count)
            {
                _pipeWriter.Advance(count);
                _bytesWritten += count;
            }

            public Memory<byte> GetMemory(int sizeHint = 0)
            {
                return _pipeWriter.GetMemory(sizeHint);
            }

            public Span<byte> GetSpan(int sizeHint = 0)
            {
                return _pipeWriter.GetSpan(sizeHint);
            }

            public void Complete()
            {
                _pipeWriter.Complete();
                _isCompleted = true;
            }

            internal void Reset()
            {
                _bytesWritten = 0;
                _isCompleted = false;
            }
        }
    }
}
