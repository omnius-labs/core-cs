using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace Omnius.Core
{
    partial class Hub
    {
        internal class BufferWriter : DisposableBase, IBufferWriter<byte>
        {
            private readonly IBufferPool<byte> _bufferPool;

            private readonly List<byte[]> _arrays = new List<byte[]>();
            private readonly List<Memory<byte>> _memories = new List<Memory<byte>>();
            private long _totalWrittenCount = 0;
            private Memory<byte> _currentMemory = Memory<byte>.Empty;
            private int _currentMemoryWrittenCount = 0;

            public BufferWriter(IBufferPool<byte> bufferPool)
            {
                _bufferPool = bufferPool;
            }

            public void Reset()
            {
                foreach (var array in _arrays)
                {
                    _bufferPool.ReturnArray(array);
                }

                _arrays.Clear();
                _memories.Clear();
                _totalWrittenCount = 0;
                _currentMemory = Memory<byte>.Empty;
                _currentMemoryWrittenCount = 0;
            }

            public long WrittenCount => _totalWrittenCount;

            public void Advance(int count)
            {
                _currentMemoryWrittenCount += count;
                _totalWrittenCount += count;
            }

            public Memory<byte> GetMemory(int sizeHint = 0)
            {
                if (sizeHint <= 0)
                {
                    sizeHint = 1;
                }

                int length = _currentMemory.Length - _currentMemoryWrittenCount;

                if (length >= sizeHint)
                {
                    return _currentMemory.Slice(_currentMemoryWrittenCount);
                }

                _memories.Add(_currentMemory.Slice(0, _currentMemoryWrittenCount));

                var byteArray = _bufferPool.RentArray(Math.Max(sizeHint, 1024 * 32));
                _arrays.Add(byteArray);

                _currentMemory = byteArray;
                _currentMemoryWrittenCount = 0;

                return _currentMemory;
            }

            public Span<byte> GetSpan(int sizeHint = 0)
            {
                return this.GetMemory(sizeHint).Span;
            }

            public ReadOnlySequence<byte> GetSequence()
            {
                if (_currentMemory.IsEmpty)
                {
                    return ReadOnlySequence<byte>.Empty;
                }

                MyReadOnlySequenceSegment firstSegment, lastSegment;

                firstSegment = new MyReadOnlySequenceSegment(_currentMemory.Slice(0, _currentMemoryWrittenCount), null, _totalWrittenCount - _currentMemoryWrittenCount);
                lastSegment = firstSegment;

                for (int i = _memories.Count - 1; i >= 1; i--)
                {
                    firstSegment = new MyReadOnlySequenceSegment(_memories[i], firstSegment, firstSegment.RunningIndex - _memories[i].Length);
                }

                return new ReadOnlySequence<byte>(firstSegment, 0, lastSegment, lastSegment.Memory.Length);
            }

            protected override void OnDispose(bool disposing)
            {
                if (!disposing)
                {
                    return;
                }

                this.Reset();
            }

            class MyReadOnlySequenceSegment : ReadOnlySequenceSegment<byte>
            {
                public MyReadOnlySequenceSegment(ReadOnlyMemory<byte> memory, MyReadOnlySequenceSegment? next, long runningIndex)
                {
                    this.Memory = memory;
                    this.Next = next;
                    this.RunningIndex = runningIndex;
                }
            }
        }
    }
}
