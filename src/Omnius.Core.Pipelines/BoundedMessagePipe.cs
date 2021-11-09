using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Pipelines;

public sealed partial class BoundedMessagePipe : DisposableBase
{
    private readonly SemaphoreSlim _writerSemaphore;
    private readonly SemaphoreSlim _readerSemaphore;

    public BoundedMessagePipe(int capacity)
    {
        _writerSemaphore = new SemaphoreSlim(capacity, capacity);
        _readerSemaphore = new SemaphoreSlim(0, capacity);

        this.Writer = new MessagePipeWriter(this);
        this.Reader = new MessagePipeReader(this);
    }

    protected override void OnDispose(bool disposing)
    {
        if (!disposing) return;
        _writerSemaphore.Dispose();
        _readerSemaphore.Dispose();
    }

    public IMessagePipeWriter Writer { get; }

    public IMessagePipeReader Reader { get; }

    public sealed class MessagePipeWriter : IMessagePipeWriter
    {
        private readonly BoundedMessagePipe _pipe;

        public MessagePipeWriter(BoundedMessagePipe pipe)
        {
            _pipe = pipe;
        }

        public async ValueTask WaitToWriteAsync(CancellationToken cancellationToken = default)
        {
            await _pipe._writerSemaphore.WaitAsync(cancellationToken);
            _pipe._writerSemaphore.Release();
        }

        public async ValueTask WriteAsync(CancellationToken cancellationToken = default)
        {
            await _pipe._writerSemaphore.WaitAsync(cancellationToken);
            _pipe._readerSemaphore.Release();
        }

        public bool TryWrite()
        {
            if (!_pipe._writerSemaphore.Wait(0)) return false;
            _pipe._readerSemaphore.Release();
            return true;
        }
    }

    public sealed class MessagePipeReader : IMessagePipeReader
    {
        private readonly BoundedMessagePipe _pipe;

        public MessagePipeReader(BoundedMessagePipe pipe)
        {
            _pipe = pipe;
        }

        public async ValueTask WaitToReadAsync(CancellationToken cancellationToken = default)
        {
            await _pipe._readerSemaphore.WaitAsync(cancellationToken);
            _pipe._readerSemaphore.Release();
        }

        public async ValueTask ReadAsync(CancellationToken cancellationToken = default)
        {
            await _pipe._readerSemaphore.WaitAsync(cancellationToken);
            _pipe._writerSemaphore.Release();
        }

        public bool TryRead()
        {
            if (!_pipe._readerSemaphore.Wait(0)) return false;
            _pipe._writerSemaphore.Release();
            return true;
        }
    }
}