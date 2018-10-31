using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;

namespace Omnix.Network.Connection.Internal
{
    partial class Hub
    {
        public sealed class HubReader
        {
            private Pipe _pipe;
            private PipeReader _pipeReader;
            private ReadOnlySequence<byte>? _sequence;
            private SequencePosition? _sequencePosition;
            private int _position = 0;
            private bool _isCompleted = false;

            internal HubReader(Pipe pipe)
            {
                _pipe = pipe;
                _pipeReader = pipe.Reader;
            }

            public int BytesConsumed => _position;
            public bool IsCompleted => _isCompleted;

            public void Advance(int count)
            {
                _position += count;
                _sequencePosition = _sequence.Value.GetPosition(_position);
            }

            public ReadOnlySequence<byte> GetSequence()
            {
                if (_sequence == null)
                {
                    if (!_pipeReader.TryRead(out var readResult)) throw new Exception();

                    _sequence = readResult.Buffer;
                    _sequencePosition = _sequence.Value.Start;
                }

                return _sequence.Value.Slice(_sequencePosition.Value);
            }

            public void Complete()
            {
                _pipeReader.Complete();
                _isCompleted = true;
            }

            internal void Reset()
            {
                _sequence = null;
                _sequencePosition = null;
                _position = 0;
                _isCompleted = false;
            }
        }
    }
}
