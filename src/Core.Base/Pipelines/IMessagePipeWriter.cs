namespace Omnius.Core.Base.Pipelines;

public interface IMessagePipeWriter
{
    ValueTask WaitToWriteAsync(CancellationToken cancellationToken = default);
    ValueTask WriteAsync(CancellationToken cancellationToken = default);
    bool TryWrite();
}
