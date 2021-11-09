using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Pipelines;

public interface IMessagePipeReader
{
    ValueTask WaitToReadAsync(CancellationToken cancellationToken = default);

    ValueTask ReadAsync(CancellationToken cancellationToken = default);

    bool TryRead();
}