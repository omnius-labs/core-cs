using System;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.RocketPack;

namespace Omnius.Core.RocketPack.Remoting
{
    public interface IRocketRemotingListener<TError> : IAsyncDisposable
        where TError : IRocketMessage<TError>
    {
        public uint FunctionId { get; }

        ValueTask ListenFunctionAsync<TParam, TResult>(Func<TParam, CancellationToken, ValueTask<TResult>> callback, CancellationToken cancellationToken = default)
            where TParam : IRocketMessage<TParam>
            where TResult : IRocketMessage<TResult>;

        ValueTask ListenFunctionAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> callback, CancellationToken cancellationToken = default)
            where TResult : IRocketMessage<TResult>;

        ValueTask ListenActionAsync<TParam>(Func<TParam, CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
            where TParam : IRocketMessage<TParam>;

        ValueTask ListenActionAsync(Func<CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default);
    }
}
