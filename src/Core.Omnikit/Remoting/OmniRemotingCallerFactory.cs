using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public sealed partial class OmniRemotingCallerFactory<TError>
    where TError : RocketMessage<TError>
{
    private readonly Func<ValueTask<Stream>> _connectFuncAsync;
    private readonly int _maxFrameLength;
    private readonly IBytesPool _bytesPool;

    public OmniRemotingCallerFactory(Func<ValueTask<Stream>> connectFuncAsync, int maxFrameLength, IBytesPool bytesPool)
    {
        _connectFuncAsync = connectFuncAsync;
        _maxFrameLength = maxFrameLength;
        _bytesPool = bytesPool;
    }

    public async ValueTask<OmniRemotingCaller<TError>> CreateAsync(uint functionId, CancellationToken cancellationToken = default)
    {
        var stream = await _connectFuncAsync();

        var caller = new OmniRemotingCaller<TError>(stream, functionId, _maxFrameLength, _bytesPool);
        await caller.HandshakeAsync(cancellationToken);

        return caller;
    }
}
