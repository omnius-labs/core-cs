namespace Omnius.Core.RocketPack.Remoting;

public interface IRocketRemotingListenerFactory<TError>
    where TError : IRocketMessage<TError>
{
    ValueTask<IRocketRemotingListener<TError>> CreateAsync(CancellationToken cancellationToken = default);
}