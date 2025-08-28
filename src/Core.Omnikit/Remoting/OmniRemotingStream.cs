using Omnius.Core.Base;
using Omnius.Core.Omnikit.Connections.Codec;
using Omnius.Core.Omnikit.Remoting.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public sealed class OmniRemotingStream<TInput, TOutput, TError> : AsyncDisposableBase
    where TInput : RocketMessage<TInput>
    where TOutput : RocketMessage<TOutput>
    where TError : RocketMessage<TError>
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

    public async ValueTask SendContinueAsync(TInput message, CancellationToken cancellationToken = default)
    {
        using var sendingMemoryOwner = PacketMessage<TInput, TError>.CreateContinue(message).Export(_bytesPool);
        await _sender.SendAsync(sendingMemoryOwner.Memory, cancellationToken);
    }

    public async ValueTask SendCompletedAsync(TInput message, CancellationToken cancellationToken = default)
    {
        using var sendingMemoryOwner = PacketMessage<TInput, TError>.CreateCompleted(message).Export(_bytesPool);
        await _sender.SendAsync(sendingMemoryOwner.Memory, cancellationToken);
    }

    public async ValueTask SendErrorAsync(TError errorMessage, CancellationToken cancellationToken = default)
    {
        using var sendingMemoryOwner = PacketMessage<EmptyRocketMessage, TError>.CreateError(errorMessage).Export(_bytesPool);
        await _sender.SendAsync(sendingMemoryOwner.Memory, cancellationToken);
    }

    public async ValueTask<PacketMessage<TOutput, TError>> ReceiveAsync(CancellationToken cancellationToken = default)
    {
        using var receivedMemoryOwner = await _receiver.ReceiveAsync(cancellationToken);
        var message = PacketMessage<TOutput, TError>.Import(receivedMemoryOwner.Memory, _bytesPool);
        return message;
    }
}
