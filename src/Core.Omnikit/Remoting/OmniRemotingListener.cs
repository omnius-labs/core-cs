using Omnius.Core.Base;
using Omnius.Core.Omnikit.Connections.Codec;
using Omnius.Core.Omnikit.Remoting.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public sealed class OmniRemotingListener<TError> : AsyncDisposableBase
    where TError : RocketMessage<TError>
{
    private readonly Stream _stream;
    private readonly FramedSender _sender;
    private readonly FramedReceiver _receiver;
    private readonly IOmniRemotingErrorMessageFactory<TError> _errorMessageFactory;
    private readonly IBytesPool _bytesPool;

    private readonly CancellationTokenSource _listenerCancellationTokenSource = new();

    public OmniRemotingListener(Stream stream, int maxFrameLength, IOmniRemotingErrorMessageFactory<TError> errorMessageFactory, IBytesPool bytesPool)
    {
        _stream = stream;
        _sender = new FramedSender(stream, maxFrameLength, bytesPool);
        _receiver = new FramedReceiver(stream, maxFrameLength, bytesPool);
        _errorMessageFactory = errorMessageFactory;
        _bytesPool = bytesPool;
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

    internal async ValueTask HandshakeAsync(CancellationToken cancellationToken = default)
    {
        using var receivedMemoryOwner = await _receiver.ReceiveAsync(cancellationToken);
        var helloMessage = HelloMessage.Import(receivedMemoryOwner.Memory, _bytesPool);

        if (helloMessage.Version == OmniRemotingVersion.V1)
        {
            this.FunctionId = helloMessage.FunctionId;
        }

        throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
    }

    public async ValueTask ListenFunctionAsync<TParam, TResult>(Func<TParam, CancellationToken, ValueTask<TResult>> callback, CancellationToken cancellationToken = default)
        where TParam : RocketMessage<TParam>
        where TResult : RocketMessage<TResult>
    {
        cancellationToken = this.GetMixedCancellationToken(cancellationToken);

        using var receivedMemoryOwner = await _receiver.ReceiveAsync(cancellationToken);
        var param = PacketMessage<TParam, TError>.Import(receivedMemoryOwner.Memory, _bytesPool);

        if (param.IsCompleted)
        {
            try
            {
                var result = await callback.Invoke(param.Message, cancellationToken);

                using var sendMemoryOwner = PacketMessage<TResult, TError>.CreateCompleted(result).Export(_bytesPool);
                await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);

                if (result is IDisposable disposable) disposable.Dispose();
                else if (result is IAsyncDisposable asyncDisposable) await asyncDisposable.DisposeAsync();

                return;
            }
            catch (Exception e)
            {
                var errorMessage = _errorMessageFactory.Create(e);

                using var sendMemoryOwner = PacketMessage<TResult, TError>.CreateError(errorMessage).Export(_bytesPool);
                await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);

                throw;
            }
        }

        throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
    }

    public async ValueTask ListenFunctionAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> callback, CancellationToken cancellationToken = default)
        where TResult : RocketMessage<TResult>
    {
        cancellationToken = this.GetMixedCancellationToken(cancellationToken);

        try
        {
            var result = await callback.Invoke(cancellationToken);

            using var sendMemoryOwner = PacketMessage<TResult, TError>.CreateCompleted(result).Export(_bytesPool);
            await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);

            if (result is IDisposable disposable) disposable.Dispose();
            else if (result is IAsyncDisposable asyncDisposable) await asyncDisposable.DisposeAsync();

            return;
        }
        catch (Exception e)
        {
            var errorMessage = _errorMessageFactory.Create(e);

            using var sendMemoryOwner = PacketMessage<TResult, TError>.CreateError(errorMessage).Export(_bytesPool);
            await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);

            throw;
        }

        throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
    }

    public async ValueTask ListenActionAsync<TParam>(Func<TParam, CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
        where TParam : RocketMessage<TParam>
    {
        cancellationToken = this.GetMixedCancellationToken(cancellationToken);

        using var receivedMemoryOwner = await _receiver.ReceiveAsync(cancellationToken);
        var param = PacketMessage<TParam, TError>.Import(receivedMemoryOwner.Memory, _bytesPool);

        if (param.IsCompleted)
        {
            try
            {
                await callback.Invoke(param.Message, cancellationToken);

                using var sendMemoryOwner = PacketMessage<UnitRocketMessage, TError>.CreateCompleted(UnitRocketMessage.Empty).Export(_bytesPool);
                await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);

                return;
            }
            catch (Exception e)
            {
                var errorMessage = _errorMessageFactory.Create(e);

                using var sendMemoryOwner = PacketMessage<UnitRocketMessage, TError>.CreateError(errorMessage).Export(_bytesPool);
                await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);

                throw;
            }
        }

        throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
    }

    public async ValueTask ListenActionAsync(Func<CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
    {
        cancellationToken = this.GetMixedCancellationToken(cancellationToken);

        try
        {
            await callback.Invoke(cancellationToken);

            using var sendMemoryOwner = PacketMessage<UnitRocketMessage, TError>.CreateCompleted(UnitRocketMessage.Empty).Export(_bytesPool);
            await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);

            return;
        }
        catch (Exception e)
        {
            var errorMessage = _errorMessageFactory.Create(e);

            using var sendMemoryOwner = PacketMessage<UnitRocketMessage, TError>.CreateError(errorMessage).Export(_bytesPool);
            await _sender.SendAsync(sendMemoryOwner.Memory, cancellationToken);

            throw;
        }

        throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
    }
}