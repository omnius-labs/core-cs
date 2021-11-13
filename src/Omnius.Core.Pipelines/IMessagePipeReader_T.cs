namespace Omnius.Core.Pipelines;

public interface IMessagePipeReader<T>
{
    ValueTask WaitToReadAsync(CancellationToken cancellationToken = default);

    ValueTask<T> ReadAsync(CancellationToken cancellationToken = default);

    bool TryRead(out T message);
}