using Omnius.Core.Base;
using Omnius.Core.Omnikit.Connections.Codec;
using Omnius.Core.Omnikit.Remoting.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public sealed class OmniRemotingStream
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

    public async ValueTask SendAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : RocketMessage<T>
    {
        using var sendingMemoryOwner = message.Export(_bytesPool);
        await _sender.SendAsync(sendingMemoryOwner.Memory, cancellationToken);
    }

    public async ValueTask<T> ReceiveAsync<T>(CancellationToken cancellationToken = default)
        where T : RocketMessage<T>
    {
        using var receivedMemoryOwner = await _receiver.ReceiveAsync(cancellationToken);
        var message = RocketMessage<T>.Import(receivedMemoryOwner.Memory, _bytesPool);
        return message;
    }
}
