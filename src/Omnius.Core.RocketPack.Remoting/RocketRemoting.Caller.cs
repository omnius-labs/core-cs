using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Connections;
using Omnius.Core.RocketPack.Remoting.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.RocketPack.Remoting
{
    public partial class RocketRemoting
    {
        internal sealed class Caller : IRocketRemotingCaller
        {
            private readonly IConnection _connection;
            private readonly IBytesPool _bytesPool;

            public Caller(IConnection connection, int functionId, IBytesPool bytesPool)
            {
                _connection = connection;
                this.FunctionId = functionId;
                _bytesPool = bytesPool;
            }

            public int FunctionId { get; }

            public async ValueTask<TResult> CallFunctionAsync<TParam, TResult, TError>(TParam param, CancellationToken cancellationToken = default)
                where TParam : IRocketMessage<TParam>
                where TResult : IRocketMessage<TResult>
                where TError : IRocketMessage<TError>
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
                        _ = PacketParser.TryParse(ref sequence, out result, _bytesPool);
                    }, cancellationToken);

                if (result.IsCompleted)
                {
                    return result.Message;
                }
                else if (result.IsError)
                {
                    throw ThrowHelper.CreateRocketPackRpcApplicationException(result.ErrorMessage);
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }

            public async ValueTask<TResult> CallFunctionAsync<TResult, TError>(CancellationToken cancellationToken = default)
                where TResult : IRocketMessage<TResult>
                where TError : IRocketMessage<TError>
            {
                var result = ParsedPacketMessage<TResult, TError>.CreateUnknown();

                await _connection.Receiver.ReceiveAsync(
                    sequence =>
                    {
                        _ = PacketParser.TryParse(ref sequence, out result, _bytesPool);
                    }, cancellationToken);

                if (result.IsCompleted)
                {
                    return result.Message;
                }
                else if (result.IsError)
                {
                    throw ThrowHelper.CreateRocketPackRpcApplicationException(result.ErrorMessage);
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }

            public async ValueTask CallActionAsync<TParam, TError>(TParam param, CancellationToken cancellationToken = default)
                where TParam : IRocketMessage<TParam>
                where TError : IRocketMessage<TError>
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
                        _ = PacketParser.TryParse(ref sequence, out result, _bytesPool);
                    }, cancellationToken);

                if (result.IsCompleted)
                {
                    return;
                }
                else if (result.IsError)
                {
                    throw ThrowHelper.CreateRocketPackRpcApplicationException(result.ErrorMessage);
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }

            public async ValueTask CallActionAsync<TError>(CancellationToken cancellationToken = default)
                where TError : IRocketMessage<TError>
            {
                var result = ParsedPacketMessage<TError>.CreateUnknown();

                await _connection.Receiver.ReceiveAsync(
                    sequence =>
                    {
                        _ = PacketParser.TryParse(ref sequence, out result, _bytesPool);
                    }, cancellationToken);

                if (result.IsCompleted)
                {
                    return;
                }
                else if (result.IsError)
                {
                    throw ThrowHelper.CreateRocketPackRpcApplicationException(result.ErrorMessage);
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }
        }
    }
}
