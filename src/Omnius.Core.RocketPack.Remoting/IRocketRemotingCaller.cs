using System;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.RocketPack;

namespace Omnius.Core.RocketPack.Remoting
{
    public interface IRocketRemotingCaller
    {
        public int FunctionId { get; }

        ValueTask<TResult> CallFunctionAsync<TParam, TResult, TError>(TParam param, CancellationToken cancellationToken = default)
            where TParam : IRocketMessage<TParam>
            where TResult : IRocketMessage<TResult>
            where TError : IRocketMessage<TError>;

        ValueTask<TResult> CallFunctionAsync<TResult, TError>(CancellationToken cancellationToken = default)
            where TResult : IRocketMessage<TResult>
            where TError : IRocketMessage<TError>;

        ValueTask CallActionAsync<TParam, TError>(TParam param, CancellationToken cancellationToken = default)
            where TParam : IRocketMessage<TParam>
            where TError : IRocketMessage<TError>;

        ValueTask CallActionAsync<TError>(CancellationToken cancellationToken = default)
            where TError : IRocketMessage<TError>;
    }
}
