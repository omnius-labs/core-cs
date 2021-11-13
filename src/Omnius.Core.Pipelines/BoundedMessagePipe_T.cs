namespace Omnius.Core.Pipelines;

public sealed partial class BoundedMessagePipe<T> : DisposableBase
{
    private readonly Queue<T> _queue = new();
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

    public IMessagePipeWriter<T> Writer { get; }

    public IMessagePipeReader<T> Reader { get; }

    public sealed class MessagePipeWriter : IMessagePipeWriter<T>
    {
        private readonly BoundedMessagePipe<T> _pipe;

        public MessagePipeWriter(BoundedMessagePipe<T> pipe)
        {
            _pipe = pipe;
        }

        public async ValueTask WaitToWriteAsync(CancellationToken cancellationToken = default)
        {
            await _pipe._writerSemaphore.WaitAsync(cancellationToken);
            _pipe._writerSemaphore.Release();
        }

        public async ValueTask WriteAsync(Func<T> func, CancellationToken cancellationToken = default)
        {
            await _pipe._writerSemaphore.WaitAsync(cancellationToken);
            this.InternalWrite(func);
        }

        public async ValueTask WriteAsync(T message, CancellationToken cancellationToken = default)
        {
            await _pipe._writerSemaphore.WaitAsync(cancellationToken);
            this.InternalWrite(message);
        }

        public bool TryWrite(Func<T> func)
        {
            if (!_pipe._writerSemaphore.Wait(0)) return false;
            this.InternalWrite(func);
            return true;
        }

        public bool TryWrite(T message)
        {
            if (!_pipe._writerSemaphore.Wait(0)) return false;
            this.InternalWrite(message);
            return true;
        }

        private void InternalWrite(Func<T> func)
        {
            var message = func.Invoke();
            _pipe._queue.Enqueue(message);
            _pipe._readerSemaphore.Release();
        }

        private void InternalWrite(T message)
        {
            _pipe._queue.Enqueue(message);
            _pipe._readerSemaphore.Release();
        }
    }

    public sealed class MessagePipeReader : IMessagePipeReader<T>
    {
        private readonly BoundedMessagePipe<T> _pipe;

        public MessagePipeReader(BoundedMessagePipe<T> pipe)
        {
            _pipe = pipe;
        }

        public async ValueTask WaitToReadAsync(CancellationToken cancellationToken = default)
        {
            await _pipe._readerSemaphore.WaitAsync(cancellationToken);
            _pipe._readerSemaphore.Release();
        }

        public async ValueTask<T> ReadAsync(CancellationToken cancellationToken = default)
        {
            await _pipe._readerSemaphore.WaitAsync(cancellationToken);
            return this.InternalRead();
        }

        public bool TryRead(out T message)
        {
            message = default!;
            if (!_pipe._readerSemaphore.Wait(0)) return false;
            message = this.InternalRead();
            return true;
        }

        private T InternalRead()
        {
            var message = _pipe._queue.Dequeue();
            _pipe._writerSemaphore.Release();
            return message;
        }
    }
}