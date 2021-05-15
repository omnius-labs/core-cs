using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;

namespace Omnius.Core.Streams
{
    /// <summary>
    /// <see cref="Omnius.Core.IBytesPool"/>を利用した<see cref="MemoryStream"/>の機能を提供します。
    /// </summary>
    public class RecyclableMemoryStream : Stream
    {
        private readonly IBytesPool _bytesPool;

        private long _position;
        private long _length;

        private readonly List<byte[]> _buffers = new List<byte[]>();
        private int _bufferSize = 32;

        private int _currentBufferIndex;
        private int _currentBufferPosition;

        private bool _disposed;

        public RecyclableMemoryStream(IBytesPool bytesPool)
        {
            _bytesPool = bytesPool;
        }

        public override bool CanRead
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

                return true;
            }
        }

        public override long Position
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

                return _position;
            }

            set
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
                if (value < 0 || this.Length < value) throw new ArgumentOutOfRangeException(nameof(value));

                if (_position == value) return;

                _currentBufferIndex = -1;
                _currentBufferPosition = -1;

                _position = value;
            }
        }

        public override long Length
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

                return _length;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

            if (origin == SeekOrigin.Begin)
            {
                return this.Position = offset;
            }
            else if (origin == SeekOrigin.Current)
            {
                return this.Position += offset;
            }
            else if (origin == SeekOrigin.End)
            {
                return this.Position = this.Length + offset;
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public override void SetLength(long value)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

            this.Position = Math.Min(_position, value);
            _length = value;
        }

        private void Search()
        {
            if (_currentBufferIndex == -1 && _currentBufferPosition == -1)
            {
                _currentBufferIndex = 0;
                _currentBufferPosition = 0;

                for (long position = 0; _currentBufferIndex < _buffers.Count; _currentBufferIndex++)
                {
                    position += _buffers[_currentBufferIndex].Length;

                    if (_position < position)
                    {
                        _currentBufferPosition = _buffers[_currentBufferIndex].Length - (int)(position - _position);
                        break;
                    }
                }
            }
        }

        public override int Read(Span<byte> buffer)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

            int offset = 0;
            int count = (int)Math.Min(buffer.Length, this.Length - this.Position);
            if (count == 0)
            {
                return 0;
            }

            int readSumLength = 0;

            this.Search();

            while (count > 0)
            {
                int length = Math.Min(_buffers[_currentBufferIndex].Length - _currentBufferPosition, count);

                BytesOperations.Copy(_buffers[_currentBufferIndex].AsSpan(_currentBufferPosition), buffer.Slice(offset), length);
                _currentBufferPosition += length;

                offset += length;
                count -= length;
                readSumLength += length;

                if (_currentBufferPosition == _buffers[_currentBufferIndex].Length)
                {
                    _currentBufferIndex++;
                    _currentBufferPosition = 0;
                }
            }

            _position += readSumLength;
            return readSumLength;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
            if (offset < 0 || buffer.Length < offset) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0 || (buffer.Length - offset) < count) throw new ArgumentOutOfRangeException(nameof(count));

            return this.Read(buffer.AsSpan(offset, count));
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

            if (buffer.Length == 0) return;

            this.Search();

            int offset = 0;
            int count = buffer.Length;
            int writeSumLength = 0;

            while (count > 0)
            {
                if (_currentBufferIndex >= _buffers.Count)
                {
                    var tempBuffer = _bytesPool.Array.Rent(_bufferSize);
                    if (_bufferSize < 1024 * 32)
                    {
                        _bufferSize *= 2;
                    }

                    _buffers.Add(tempBuffer);
                }

                int length = Math.Min(_buffers[_currentBufferIndex].Length - _currentBufferPosition, count);

                BytesOperations.Copy(buffer.Slice(offset), _buffers[_currentBufferIndex].AsSpan(_currentBufferPosition), length);
                _currentBufferPosition += length;

                offset += length;
                count -= length;
                writeSumLength += length;

                if (_currentBufferPosition == _buffers[_currentBufferIndex].Length)
                {
                    _currentBufferIndex++;
                    _currentBufferPosition = 0;
                }
            }

            _position += writeSumLength;
            _length = Math.Max(_length, _position);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
            if (offset < 0 || buffer.Length < offset) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0 || (buffer.Length - offset) < count) throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0) return;

            this.Write(buffer.AsSpan(offset, count));
        }

        public override void Flush()
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_disposed) return;

                _disposed = true;

                if (disposing)
                {
                    for (int i = 0; i < _buffers.Count; i++)
                    {
                        _bytesPool.Array.Return(_buffers[i]);
                    }

                    _buffers.Clear();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public IMemoryOwner<byte> ToMemoryOwner()
        {
            var bufferLength = (int)this.Length;
            var buffer = _bytesPool.Array.Rent(bufferLength);

            long position = this.Position;

            try
            {
                this.Seek(0, SeekOrigin.Begin);
                this.Read(buffer.AsSpan().Slice(0, bufferLength));
            }
            finally
            {
                this.Position = position;
            }

            var memoryOwner = new MemoryOwner<byte>(buffer.AsMemory().Slice(0, bufferLength), () => _bytesPool.Array.Return(buffer));
            return memoryOwner;
        }
    }
}
