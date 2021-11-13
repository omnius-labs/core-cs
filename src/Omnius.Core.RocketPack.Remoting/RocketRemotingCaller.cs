using Omnius.Core.Net.Connections;
using Omnius.Core.RocketPack.Remoting.Internal;

namespace Omnius.Core.RocketPack.Remoting;

internal sealed class RocketRemotingCaller<TError> : AsyncDisposableBase, IRocketRemotingCaller<TError>
    where TError : IRocketMessage<TError>
{
    private readonly IConnection _connection;
    private readonly IBytesPool _bytesPool;

    public RocketRemotingCaller(IConnection connection, uint functionId, IBytesPool bytesPool)
    {
        _connection = connection;
        this.FunctionId = functionId;
        _bytesPool = bytesPool;
    }

    protected override async ValueTask OnDisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    public uint FunctionId { get; }

    public async ValueTask<TResult> CallFunctionAsync<TParam, TResult>(TParam param, CancellationToken cancellationToken = default)
        where TParam : IRocketMessage<TParam>
        where TResult : IRocketMessage<TResult>
    {
        await _connection.Sender.SendAsync(
            bufferWriter =>
            {
                var builder = new PacketBuilder(bufferWriter, _bytesPool);
                builder.WriteCompleted(param);
            }, cancellationToken);

        var result = ParsedPacketMessage<TResult, TError>.CreateUnknown();

        await _connection.Receiver.ReceiveAsync(
            sequence =>
            {
                PacketParser.TryParse(ref sequence, out result, _bytesPool);
            }, cancellationToken);

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
        where TResult : IRocketMessage<TResult>
    {
        var result = ParsedPacketMessage<TResult, TError>.CreateUnknown();

        await _connection.Receiver.ReceiveAsync(
            sequence =>
            {
                PacketParser.TryParse(ref sequence, out result, _bytesPool);
            }, cancellationToken);

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
        where TParam : IRocketMessage<TParam>
    {
        await _connection.Sender.SendAsync(
            bufferWriter =>
            {
                var builder = new PacketBuilder(bufferWriter, _bytesPool);
                builder.WriteCompleted(param);
            }, cancellationToken);

        var result = ParsedPacketMessage<TError>.CreateUnknown();

        await _connection.Receiver.ReceiveAsync(
            sequence =>
            {
                PacketParser.TryParse(ref sequence, out result, _bytesPool);
            }, cancellationToken);

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
        var result = ParsedPacketMessage<TError>.CreateUnknown();

        await _connection.Receiver.ReceiveAsync(
            sequence =>
            {
                PacketParser.TryParse(ref sequence, out result, _bytesPool);
            }, cancellationToken);

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