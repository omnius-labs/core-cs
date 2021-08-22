using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Omnius.Core.Helpers;

namespace Omnius.Core.Pipelines
{
    public partial class BytesPipe
    {
        internal partial class BytesState : DisposableBase, IBufferWriter<byte>
        {
            private readonly IBytesPool _bytesPool;

            private readonly List<byte[]> _arrays = new();
            private readonly List<Memory<byte>> _memories = new();
            private long _totalWrittenBytes = 0;
            private Memory<byte> _currentMemory = Memory<byte>.Empty;
            private int _currentMemoryWrittenBytes = 0;

            public BytesState(IBytesPool bytesPool)
            {
                _bytesPool = bytesPool;
            }

            protected override void OnDispose(bool disposing)
            {
                if (disposing)
                {
                    this.Reset();
                }
            }

            public void Reset()
            {
                foreach (var array in _arrays)
                {
                    _bytesPool.Array.Return(array);
                }

                _arrays.Clear();
                _memories.Clear();
                _totalWrittenBytes = 0;
                _currentMemory = Memory<byte>.Empty;
                _currentMemoryWrittenBytes = 0;
            }

            public long WrittenBytes => _totalWrittenBytes;

            public void Advance(int count)
            {
                _currentMemoryWrittenBytes += count;
                _totalWrittenBytes += count;
            }

            public Memory<byte> GetMemory(int sizeHint = 0)
            {
                if (sizeHint <= 0) sizeHint = 1;

                int length = _currentMemory.Length - _currentMemoryWrittenBytes;
                if (length >= sizeHint) return _currentMemory[_currentMemoryWrittenBytes..];

                _memories.Add(_currentMemory.Slice(0, _currentMemoryWrittenBytes));

                var byteArray = _bytesPool.Array.Rent(Math.Max(sizeHint, 1024 * 32));
                _arrays.Add(byteArray);

                _currentMemory = byteArray;
                _currentMemoryWrittenBytes = 0;

                return _currentMemory;
            }

            public Span<byte> GetSpan(int sizeHint = 0)
            {
                return this.GetMemory(sizeHint).Span;
            }

            public ReadOnlySequence<byte> GetSequence()
            {
                if (_currentMemory.IsEmpty) return ReadOnlySequence<byte>.Empty;

                var memories = new List<ReadOnlyMemory<byte>>();
                foreach (var memory in _memories.Skip(1))
                {
                    memories.Add(memory);
                }

                memories.Add(_currentMemory.Slice(0, _currentMemoryWrittenBytes));

                return ReadOnlySequenceHelper.Create(memories);
            }
        }
    }
}
