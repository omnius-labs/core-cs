using Omnius.Core.Base;
using Omnius.Core.Omnikit.Connections.Codec;
using Omnius.Core.Omnikit.Remoting.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public sealed class OmniRemotingListener : AsyncDisposableBase
{
    private readonly Stream _stream;
    private readonly FramedSender _sender;
    private readonly FramedReceiver _receiver;
    private readonly IBytesPool _bytesPool;

    private readonly CancellationTokenSource _listenerCancellationTokenSource = new();

    public static async ValueTask<OmniRemotingListener> Create(Stream stream, int maxFrameLength, IBytesPool bytesPool, CancellationToken cancellationToken = default)
    {
        var value = new OmniRemotingListener(stream, maxFrameLength, bytesPool);
        await value.HandshakeAsync(cancellationToken);
        return value;
    }

    private OmniRemotingListener(Stream stream, int maxFrameLength, IBytesPool bytesPool)
    {
        _stream = stream;
        _sender = new FramedSender(stream, maxFrameLength, bytesPool);
        _receiver = new FramedReceiver(stream, maxFrameLength, bytesPool);
        _bytesPool = bytesPool;
    }

    private async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
    {
        using var receivedMemoryOwner = await _receiver.ReceiveAsync(cancellationToken);
        var helloMessage = RocketPackStruct.Import<HelloMessage>(receivedMemoryOwner.Memory, _bytesPool);

        if (helloMessage.Version == OmniRemotingVersion.V1)
        {
            this.FunctionId = helloMessage.FunctionId;
            return;
        }

        throw new OmniRemotingProtocolException("Unsupported version");
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await _stream.DisposeAsync();
    }

    public uint FunctionId { get; private set; }

    private CancellationToken GetMixedCancellationToken(CancellationToken cancellationToken)
    {
        var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _listenerCancellationTokenSource.Token).Token;
        return linkedTokenSource;
    }

    public async ValueTask ListenStreamAsync(Func<OmniRemotingStream, CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
    {
        await callback(new OmniRemotingStream(_sender, _receiver, _bytesPool), this.GetMixedCancellationToken(cancellationToken));
    }
}
