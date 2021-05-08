using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Omnius.Core.Streams
{
    /// <summary>
    /// IOバッファを無効化した<see cref="FileStream"/>の機能を提供します。 (OSがWindowsの時のみFILE_FLAG_NO_BUFFERINGフラグを有効化します)
    /// </summary>
    public class UnbufferedFileStream : Stream
    {
        private readonly string _path;
        private readonly IBytesPool _bytesPool;

        private long _position;
        private long _length;
        private readonly FileStream _stream;

        private bool _blockIsUpdated;
        private long _blockPosition;
        private int _blockOffset;
        private int _blockCount;
        private readonly byte[] _blockBuffer;

        private bool _disposed;

        private const int SectorSize = 1024 * 256;

        public UnbufferedFileStream(string path, FileMode mode, FileAccess access, FileShare share, FileOptions options, IBytesPool bytesPool)
        {
            _path = path;
            _bytesPool = bytesPool;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                const FileOptions FileFlagNoBuffering = (FileOptions)0x20000000;

                _stream = new FileStream(path, mode, access, share, 8, options | FileFlagNoBuffering);
            }
            else
            {
                _stream = new FileStream(path, mode, access, share, 8, options);
            }

            _blockIsUpdated = false;
            _blockPosition = -1;
            _blockOffset = 0;
            _blockCount = 0;
            _blockBuffer = _bytesPool.Array.Rent(SectorSize);

            _position = _stream.Position;
            _length = _stream.Length;
        }

        public string Name
        {
            get
            {
                return _stream.Name;
            }
        }

        public override bool CanRead
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

                return _stream.CanRead;
            }
        }

        public override bool CanWrite
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

                return _stream.CanWrite;
            }
        }

        public override bool CanSeek
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

                return _stream.CanSeek;
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

                if (_position == value) return;

                if (_blockIsUpdated)
                {
                    this.Flush();
                }
                else
                {
                    long p = (value / SectorSize) * SectorSize;

                    if (_blockPosition != p)
                    {
                        _blockPosition = -1;
                        _blockOffset = 0;
                        _blockCount = 0;
                    }
                }

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

            _length = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
            if (offset < 0 || buffer.Length < offset) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0 || (buffer.Length - offset) < count) throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0)
            {
                return 0;
            }

            if (_blockIsUpdated)
            {
                this.Flush();
            }

            count = (int)Math.Min(count, _length - _position);

            int readSumLength = 0;

            while (count > 0)
            {
                long p = (_position / SectorSize) * SectorSize;

                if (_blockPosition != p)
                {
                    _blockPosition = p;
                    _blockOffset = 0;

                    _stream.Seek(_blockPosition, SeekOrigin.Begin);

                    int readLength = _stream.Read(_blockBuffer, 0, SectorSize);
                    readLength = (int)Math.Min(_length - _blockPosition, readLength);

                    _blockCount = readLength;
                }

                int blockReadPosition = (int)(_position - p);
                int length = Math.Min(SectorSize - blockReadPosition, count);
                BytesOperations.Copy(_blockBuffer.AsSpan(blockReadPosition), buffer.AsSpan(offset), length);

                offset += length;
                count -= length;

                _position += length;

                readSumLength += length;
            }

            return readSumLength;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);
            if (offset < 0 || buffer.Length < offset) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0 || (buffer.Length - offset) < count) throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0) return;

            while (count > 0)
            {
                long p = (_position / SectorSize) * SectorSize;

                if (_blockPosition != p)
                {
                    this.Flush();
                }

                _stream.Seek(p, SeekOrigin.Begin);

                int blockWritePosition = (int)(_position - p);
                int length = Math.Min(SectorSize - blockWritePosition, count);

                _blockPosition = p;
                BytesOperations.Copy(buffer.AsSpan(offset), _blockBuffer.AsSpan(blockWritePosition), length);
                if (_blockCount == 0)
                {
                    _blockOffset = blockWritePosition;
                }

                _blockCount = (length + blockWritePosition) - _blockOffset;

                _blockIsUpdated = true;

                offset += length;
                count -= length;

                _position += length;
            }
        }

        public override void Flush()
        {
            if (_blockPosition == -1) return;

            if (_blockIsUpdated)
            {
                _length = Math.Max(_length, _blockPosition + _blockOffset + _blockCount);

                if (_blockOffset != 0 || _blockCount != SectorSize)
                {
                    using (var memoryOwner = _bytesPool.Memory.Rent(SectorSize))
                    {
                        _stream.Seek(_blockPosition, SeekOrigin.Begin);

                        int readLength = _stream.Read(memoryOwner.Memory.Span.Slice(0, SectorSize));
                        readLength = (int)Math.Min(_length - _blockPosition, readLength);

                        BytesOperations.Zero(memoryOwner.Memory.Span.Slice(readLength, SectorSize - readLength));
                        BytesOperations.Copy(_blockBuffer.AsSpan(_blockOffset), memoryOwner.Memory.Span.Slice(_blockOffset), _blockCount);

                        _stream.Seek(_blockPosition, SeekOrigin.Begin);
                        _stream.Write(memoryOwner.Memory.Span.Slice(0, SectorSize));
                    }
                }
                else
                {
                    _stream.Seek(_blockPosition, SeekOrigin.Begin);
                    _stream.Write(_blockBuffer.AsSpan().Slice(0, _blockCount));
                }
            }

            _blockIsUpdated = false;
            _blockPosition = -1;
            _blockOffset = 0;
            _blockCount = 0;

            _stream.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_disposed) return;

                _disposed = true;

                if (disposing)
                {
                    this.Flush();

                    _stream.Dispose();
                    _bytesPool.Array.Return(_blockBuffer);

                    using (var stream = new FileStream(_path, FileMode.Open))
                    {
                        if (stream.Length != _length)
                        {
                            stream.SetLength(_length);
                        }
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
