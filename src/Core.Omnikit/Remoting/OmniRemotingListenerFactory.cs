using Microsoft.Extensions.Logging;
using Omnius.Core.Base;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public sealed partial class RocketRemotingListenerFactory<TError>
    where TError : RocketMessage<TError>
{
    private readonly Func<ValueTask<Stream>> _acceptFuncAsync;
    private readonly int _maxFrameLength;
    private readonly IOmniRemotingErrorMessageFactory<TError> _errorMessageFactory;
    private readonly IBytesPool _bytesPool;

    public RocketRemotingListenerFactory(Func<ValueTask<Stream>> acceptFuncAsync, int maxFrameLength, IOmniRemotingErrorMessageFactory<TError> errorMessageFactory, IBytesPool bytesPool)
    {
        _acceptFuncAsync = acceptFuncAsync;
        _maxFrameLength = maxFrameLength;
        _errorMessageFactory = errorMessageFactory;
        _bytesPool = bytesPool;
    }

    public async ValueTask<OmniRemotingListener<TError>> AcceptAsync(CancellationToken cancellationToken = default)
    {
        var stream = await _acceptFuncAsync();

        var listener = new OmniRemotingListener<TError>(stream, _maxFrameLength, _errorMessageFactory, _bytesPool);
        await listener.HandshakeAsync(cancellationToken);

        return listener;
    }
}
