namespace Omnius.Core.Pipelines;

public interface IMessagePipeReader
{
    ValueTask WaitToReadAsync(CancellationToken cancellationToken = default);

    ValueTask ReadAsync(CancellationToken cancellationToken = default);

    bool TryRead();
}
