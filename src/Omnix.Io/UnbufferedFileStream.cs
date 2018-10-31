using System;
using System.IO;
using System.Runtime.InteropServices;
using Omnix.Base;

namespace Omnix.Io
{
    /// <summary>
    /// IOバッファを無効化した<see cref="FileStream"/>の機能を提供します。 (OSがWindowsの時のみFILE_FLAG_NO_BUFFERINGフラグを有効化します)
    /// </summary>
    public class UnbufferedFileStream : Stream
    {
        private class BlockInfo
        {
            public bool IsUpdated { get; set; }
            public long Position { get; set; }
            public int Offset { get; set; }
            public int Count { get; set; }
            public byte[] Value { get; set; }
        }

        private string _path;
        private BufferPool _bufferPool;

        private long _position;
        private long _length;
        private FileStream _stream;

        private BlockInfo _blockInfo;

        private bool _disposed;

        private const int SectorSize = 1024 * 256;

        public UnbufferedFileStream(string path, FileMode mode, FileAccess access, FileShare share, FileOptions options, BufferPool bufferPool)
        {
            _path = path;
            _bufferPool = bufferPool;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                const FileOptions FileFlagNoBuffering = (FileOptions)0x20000000;

                _stream = new FileStream(path, mode, access, share, 8, options | FileFlagNoBuffering);

            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                _stream = new FileStream(path, mode, access, share, 8, options);
            }

            _blockInfo = new BlockInfo();
            {
                _blockInfo.IsUpdated = false;
                _blockInfo.Position = -1;
                _blockInfo.Offset = 0;
                _blockInfo.Count = 0;
                _blockInfo.Value = _bufferPool.GetArrayPool().Rent(SectorSize);
            }

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

                if (_blockInfo.IsUpdated)
                {
                    this.Flush();
                }
                else
                {
                    long p = (value / SectorSize) * SectorSize;

                    if (_blockInfo.Position != p)
                    {
                        _blockInfo.Position = -1;
                        _blockInfo.Offset = 0;
                        _blockInfo.Count = 0;
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
            if (count == 0) return 0;

            if (_blockInfo.IsUpdated)
            {
                this.Flush();
            }

            count = (int)Math.Min(count, _length - _position);

            int readSumLength = 0;

            while (count > 0)
            {
                long p = (_position / SectorSize) * SectorSize;

                if (_blockInfo.Position != p)
                {
                    _blockInfo.Position = p;
                    _blockInfo.Offset = 0;

                    _stream.Seek(_blockInfo.Position, SeekOrigin.Begin);

                    int readLength = _stream.Read(_blockInfo.Value, 0, SectorSize);
                    readLength = (int)Math.Min(_length - _blockInfo.Position, readLength);

                    _blockInfo.Count = readLength;
                }

                int blockReadPosition = (int)(_position - p);
                int length = Math.Min(SectorSize - blockReadPosition, count);
                BytesOperations.Copy(_blockInfo.Value.AsSpan(blockReadPosition), buffer.AsSpan(offset), length);

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

                if (_blockInfo.Position != p)
                {
                    this.Flush();
                }

                _stream.Seek(p, SeekOrigin.Begin);

                int blockWritePosition = (int)(_position - p);
                int length = Math.Min(SectorSize - blockWritePosition, count);

                _blockInfo.Position = p;
                BytesOperations.Copy(buffer.AsSpan(offset), _blockInfo.Value.AsSpan(blockWritePosition), length);
                if (_blockInfo.Count == 0)
                    _blockInfo.Offset = blockWritePosition;
                _blockInfo.Count = (length + blockWritePosition) - _blockInfo.Offset;

                _blockInfo.IsUpdated = true;

                offset += length;
                count -= length;

                _position += length;
            }
        }

        public override void Flush()
        {
            if (_blockInfo.Position == -1) return;

            if (_blockInfo.IsUpdated)
            {
                _length = Math.Max(_length, _blockInfo.Position + _blockInfo.Offset + _blockInfo.Count);

                if (_blockInfo.Offset != 0 || _blockInfo.Count != SectorSize)
                {
                    using (var memoryOwner = _bufferPool.Rent(SectorSize))
                    {
                        _stream.Seek(_blockInfo.Position, SeekOrigin.Begin);

                        int readLength = _stream.Read(memoryOwner.Memory.Span.Slice(0, SectorSize));
                        readLength = (int)Math.Min(_length - _blockInfo.Position, readLength);

                        BytesOperations.Zero(memoryOwner.Memory.Span.Slice(readLength, SectorSize - readLength));
                        BytesOperations.Copy(_blockInfo.Value.AsSpan(_blockInfo.Offset), memoryOwner.Memory.Span.Slice(_blockInfo.Offset), _blockInfo.Count);

                        _stream.Seek(_blockInfo.Position, SeekOrigin.Begin);
                        _stream.Write(memoryOwner.Memory.Span.Slice(0, SectorSize));
                    }
                }
                else
                {
                    _stream.Seek(_blockInfo.Position, SeekOrigin.Begin);
                    _stream.Write(_blockInfo.Value.AsSpan().Slice(0, _blockInfo.Count));
                }
            }

            _blockInfo.IsUpdated = false;
            _blockInfo.Position = -1;
            _blockInfo.Offset = 0;
            _blockInfo.Count = 0;

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

                    if (_stream != null)
                    {
                        try
                        {
                            _stream.Dispose();
                        }
                        catch (Exception)
                        {

                        }

                        _stream = null;
                    }

                    if (_blockInfo.Value != null)
                    {
                        try
                        {
                            _bufferPool.GetArrayPool().Return(_blockInfo.Value);
                        }
                        catch (Exception)
                        {

                        }

                        _blockInfo.Value = null;
                    }

                    try
                    {
                        using (var stream = new FileStream(_path, FileMode.Open))
                        {
                            if (stream.Length != _length)
                            {
                                stream.SetLength(_length);
                            }
                        }
                    }
                    catch (Exception)
                    {

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
