using System;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.RocketPack;

namespace Omnius.Core.RocketPack.Remoting
{
    public interface IRocketRemotingListener
    {
        public int FunctionId { get; }

        ValueTask ListenFunctionAsync<TParam, TResult, TError>(Func<TParam, CancellationToken, ValueTask<TResult>> callback, IErrorMessageFactory<TError> _errorMessageFactory, CancellationToken cancellationToken = default)
            where TParam : IRocketMessage<TParam>
            where TResult : IRocketMessage<TResult>
            where TError : IRocketMessage<TError>;

        ValueTask ListenFunctionAsync<TResult, TError>(Func<CancellationToken, ValueTask<TResult>> callback, IErrorMessageFactory<TError> _errorMessageFactory, CancellationToken cancellationToken = default)
            where TResult : IRocketMessage<TResult>
            where TError : IRocketMessage<TError>;

        ValueTask ListenActionAsync<TParam, TError>(Func<TParam, CancellationToken, ValueTask> callback, IErrorMessageFactory<TError> _errorMessageFactory, CancellationToken cancellationToken = default)
            where TParam : IRocketMessage<TParam>
            where TError : IRocketMessage<TError>;

        ValueTask ListenActionAsync<TError>(Func<CancellationToken, ValueTask> callback, IErrorMessageFactory<TError> _errorMessageFactory, CancellationToken cancellationToken = default) where TError : IRocketMessage<TError>;
    }
}
