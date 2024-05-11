namespace Core.RocketPack.Remoting;

public interface IRocketRemotingCaller<TError> : IAsyncDisposable
    where TError : IRocketMessage<TError>
{
    public uint FunctionId { get; }

    ValueTask<TResult> CallFunctionAsync<TParam, TResult>(TParam param, CancellationToken cancellationToken = default)
        where TParam : IRocketMessage<TParam>
        where TResult : IRocketMessage<TResult>;

    ValueTask<TResult> CallFunctionAsync<TResult>(CancellationToken cancellationToken = default)
        where TResult : IRocketMessage<TResult>;

    ValueTask CallActionAsync<TParam>(TParam param, CancellationToken cancellationToken = default)
        where TParam : IRocketMessage<TParam>;

    ValueTask CallActionAsync(CancellationToken cancellationToken = default);
}
