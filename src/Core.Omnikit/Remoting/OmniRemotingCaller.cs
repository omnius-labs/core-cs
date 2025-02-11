using Omnius.Core.Base;
using Omnius.Core.Omnikit.Connections.Codec;
using Omnius.Core.Omnikit.Remoting.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public sealed class OmniRemotingCaller<TError> : AsyncDisposableBase
    where TError : RocketMessage<TError>
{
    private readonly Stream _stream;
    private readonly FramedSender _sender;
    private readonly FramedReceiver _receiver;
    private readonly IBytesPool _bytesPool;

    public OmniRemotingCaller(Stream stream, uint functionId, int maxFrameLength, IBytesPool bytesPool)
    {
        _stream = stream;
        _sender = new FramedSender(stream, maxFrameLength, bytesPool);
        _receiver = new FramedReceiver(stream, maxFrameLength, bytesPool);
        this.FunctionId = functionId;
        _bytesPool = bytesPool;
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await _sender.DisposeAsync();
        await _receiver.DisposeAsync();
    }

    public uint FunctionId { get; }

    public async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
    {
        var helloMessage = new HelloMessage { Version = OmniRemotingVersion.V1, FunctionId = this.FunctionId };
        using var sendingMemoryOwner = helloMessage.Export(_bytesPool);
        await _sender.SendAsync(sendingMemoryOwner.Memory, cancellationToken);
    }

    public async ValueTask<TResult> CallAsync<TParam, TResult>(TParam param, CancellationToken cancellationToken = default)
        where TParam : RocketMessage<TParam>
        where TResult : RocketMessage<TResult>
    {
        using var sendingMemoryOwner = PacketMessage<TParam, TError>.CreateCompleted(param).Export(_bytesPool);
        await _sender.SendAsync(sendingMemoryOwner.Memory, cancellationToken);

        using var receivedMemoryOwner = await _receiver.ReceiveAsync(cancellationToken);
        var result = PacketMessage<TResult, TError>.Import(receivedMemoryOwner.Memory, _bytesPool);

        if (result.IsCompleted)
        {
            return result.Message;
        }
        else if (result.IsError)
        {
            throw ThrowHelper.CreateRocketRemotingApplicationException(result.ErrorMessage);
        }

        throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
    }
}
