using Omnius.Core.Base;
using Omnius.Core.Omnikit.Connections.Codec;
using Omnius.Core.Omnikit.Remoting.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public sealed class OmniRemotingStream<TInput, TOutput> : AsyncDisposableBase
    where TInput : RocketMessage<TInput>
    where TOutput : RocketMessage<TOutput>
{
    private readonly FramedSender _sender;
    private readonly FramedReceiver _receiver;
    private readonly IBytesPool _bytesPool;

    public OmniRemotingStream(FramedSender sender, FramedReceiver receiver, IBytesPool bytesPool)
    {
        _sender = sender;
        _receiver = receiver;
        _bytesPool = bytesPool;
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await _sender.DisposeAsync();
        await _receiver.DisposeAsync();
    }

    public async ValueTask SendAsync(TInput message, CancellationToken cancellationToken = default)
    {
        using var sendingMemoryOwner = message.Export(_bytesPool);
        await _sender.SendAsync(sendingMemoryOwner.Memory, cancellationToken);
    }

    public async ValueTask<TOutput> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        using var receivedMemoryOwner = await _receiver.ReceiveAsync(cancellationToken);
        var message = RocketMessage<TOutput>.Import(receivedMemoryOwner.Memory, _bytesPool);
        return message;
    }
}
