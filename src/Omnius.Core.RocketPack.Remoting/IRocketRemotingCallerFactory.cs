namespace Omnius.Core.RocketPack.Remoting;

public interface IRocketRemotingCallerFactory<TError>
    where TError : IRocketMessage<TError>
{
    ValueTask<IRocketRemotingCaller<TError>> CreateAsync(uint functionId, CancellationToken cancellationToken = default);
}
