using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Pipelines;

public interface IMessagePipeWriter
{
    ValueTask WaitToWriteAsync(CancellationToken cancellationToken = default);

    ValueTask WriteAsync(CancellationToken cancellationToken = default);

    bool TryWrite();
}