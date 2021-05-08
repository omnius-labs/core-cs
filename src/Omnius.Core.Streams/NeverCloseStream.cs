using System;
using System.IO;

namespace Omnius.Core.Streams
{
    /// <summary>
    /// DisposeとCloseの呼び出しを無効化する<see cref="Stream"/>のラッパークラスです。
    /// </summary>
    public class NeverCloseStream : Stream
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Stream _stream;

        private bool _disposed;

        public NeverCloseStream(Stream stream)
        {
            _stream = stream;
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

                return _stream.Position;
            }

            set
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

                _stream.Position = value;
            }
        }

        public override long Length
        {
            get
            {
                if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

                return _stream.Length;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

            return _stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_disposed) throw new ObjectDisposedException(this.GetType().FullName);

            _stream.Write(buffer, offset, count);
        }

        public override void Flush()
        {
            try
            {
                _stream.Flush();
            }
            catch (Exception e)
            {
                _logger.Debug(e);
            }
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
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
