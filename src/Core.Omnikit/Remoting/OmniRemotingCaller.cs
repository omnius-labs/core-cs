using Omnius.Core.Base;
using Omnius.Core.Omnikit.Connections.Codec;
using Omnius.Core.Omnikit.Remoting.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public sealed class OmniRemotingCaller : AsyncDisposableBase
{
    private readonly FramedSender _sender;
    private readonly FramedReceiver _receiver;
    private readonly IBytesPool _bytesPool;

    public static async ValueTask<OmniRemotingCaller> Create(Stream stream, uint functionId, int maxFrameLength, IBytesPool bytesPool, CancellationToken cancellationToken = default)
    {
        var value = new OmniRemotingCaller(stream, functionId, maxFrameLength, bytesPool);
        await value.HandshakeAsync(cancellationToken);
        return value;
    }

    private OmniRemotingCaller(Stream stream, uint functionId, int maxFrameLength, IBytesPool bytesPool)
    {
        _sender = new FramedSender(stream, maxFrameLength, bytesPool);
        _receiver = new FramedReceiver(stream, maxFrameLength, bytesPool);
        this.FunctionId = functionId;
        _bytesPool = bytesPool;
    }

    private async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
    {
        var helloMessage = new HelloMessage { Version = OmniRemotingVersion.V1, FunctionId = this.FunctionId };
        using var sendingMemoryOwner = helloMessage.Export(_bytesPool);
        await _sender.SendAsync(sendingMemoryOwner.Memory, cancellationToken);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await _sender.DisposeAsync();
        await _receiver.DisposeAsync();
    }

    public uint FunctionId { get; }

    public OmniRemotingStream<TInput, TOutput> CallStream<TInput, TOutput>()
        where TInput : RocketMessage<TInput>
        where TOutput : RocketMessage<TOutput>
    {
        return new OmniRemotingStream<TInput, TOutput>(_sender, _receiver, _bytesPool);
    }
}
