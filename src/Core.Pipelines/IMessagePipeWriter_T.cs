namespace Omnius.Core.Pipelines;

public interface IMessagePipeWriter<T>
{
    ValueTask WaitToWriteAsync(CancellationToken cancellationToken = default);
    ValueTask WriteAsync(Func<T> func, CancellationToken cancellationToken = default);
    ValueTask WriteAsync(T message, CancellationToken cancellationToken = default);
    bool TryWrite(Func<T> func);
    bool TryWrite(T message);
}
