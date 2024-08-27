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
        await _stream.DisposeAsync();
    }

    public uint FunctionId { get; }

    internal async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
    {
        var helloMessage = new HelloMessage { Version = OmniRemotingVersion.V1, FunctionId = this.FunctionId };
        using var sendMemoryOwner = helloMessage.Export(_bytesPool);
        await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);
    }

    public async ValueTask<TResult> CallFunctionAsync<TParam, TResult>(TParam param, CancellationToken cancellationToken = default)
        where TParam : RocketMessage<TParam>
        where TResult : RocketMessage<TResult>
    {
        using var sendMemoryOwner = PacketMessage<TParam, TError>.CreateCompleted(param).Export(_bytesPool);
        await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);

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

    public async ValueTask<TResult> CallFunctionAsync<TResult>(CancellationToken cancellationToken = default)
        where TResult : RocketMessage<TResult>
    {
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

    public async ValueTask CallActionAsync<TParam>(TParam param, CancellationToken cancellationToken = default)
        where TParam : RocketMessage<TParam>
    {
        using var sendMemoryOwner = PacketMessage<TParam, TError>.CreateCompleted(param).Export(_bytesPool);
        await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);

        using var receivedMemoryOwner = await _receiver.ReceiveAsync(cancellationToken);
        var result = PacketMessage<UnitRocketMessage, TError>.Import(receivedMemoryOwner.Memory, _bytesPool);

        if (result.IsCompleted)
        {
            return;
        }
        else if (result.IsError)
        {
            throw ThrowHelper.CreateRocketRemotingApplicationException(result.ErrorMessage);
        }

        throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
    }

    public async ValueTask CallActionAsync(CancellationToken cancellationToken = default)
    {
        using var receivedMemoryOwner = await _receiver.ReceiveAsync(cancellationToken);
        var result = PacketMessage<UnitRocketMessage, TError>.Import(receivedMemoryOwner.Memory, _bytesPool);

        if (result.IsCompleted)
        {
            return;
        }
        else if (result.IsError)
        {
            throw ThrowHelper.CreateRocketRemotingApplicationException(result.ErrorMessage);
        }

        throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
    }
}
