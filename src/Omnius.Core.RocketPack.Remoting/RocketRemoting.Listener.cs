using System;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Connections;
using Omnius.Core.RocketPack.Remoting.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.RocketPack.Remoting
{
    public partial class RocketRemoting
    {
        internal sealed class Listener : IRocketRemotingListener
        {
            private readonly IConnection _connection;
            private readonly IBytesPool _bytesPool;

            public Listener(IConnection connection, int functionId, IBytesPool bytesPool)
            {
                _connection = connection;
                this.FunctionId = functionId;
                _bytesPool = bytesPool;
            }

            public int FunctionId { get; }

            public async ValueTask ListenFunctionAsync<TParam, TResult, TError>(Func<TParam, CancellationToken, ValueTask<TResult>> callback, IErrorMessageFactory<TError> _errorMessageFactory, CancellationToken cancellationToken = default)
                where TParam : IRocketMessage<TParam>
                where TResult : IRocketMessage<TResult>
                where TError : IRocketMessage<TError>
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                using var onClose = _connection.Subscribers.OnClosed.Subscribe(() => linkedTokenSource.Cancel());

                var param = ParsedPacketMessage<TParam, TError>.CreateUnknown();

                await _connection.Receiver.ReceiveAsync(
                    sequence =>
                    {
                        _ = PacketParser.TryParse(ref sequence, out param, _bytesPool);
                    }, cancellationToken);

                if (param.IsCompleted)
                {
                    try
                    {
                        var result = await callback.Invoke(param.Message, linkedTokenSource.Token);

                        await _connection.Sender.SendAsync(
                            bufferWriter =>
                            {
                                var builder = new PacketBuilder(bufferWriter, _bytesPool);
                                builder.WriteCompleted(result);
                            }, cancellationToken);

                        return;
                    }
                    catch (Exception e)
                    {
                        var errorMessage = _errorMessageFactory.Create(e);

                        await _connection.Sender.SendAsync(
                            bufferWriter =>
                            {
                                var builder = new PacketBuilder(bufferWriter, _bytesPool);
                                builder.WriteError(errorMessage);
                            }, cancellationToken);
                    }
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }

            public async ValueTask ListenFunctionAsync<TResult, TError>(Func<CancellationToken, ValueTask<TResult>> callback, IErrorMessageFactory<TError> _errorMessageFactory, CancellationToken cancellationToken = default)
                where TResult : IRocketMessage<TResult>
                where TError : IRocketMessage<TError>
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                using var onClose = _connection.Subscribers.OnClosed.Subscribe(() => linkedTokenSource.Cancel());

                try
                {
                    var result = await callback.Invoke(linkedTokenSource.Token);

                    await _connection.Sender.SendAsync(
                        bufferWriter =>
                        {
                            var builder = new PacketBuilder(bufferWriter, _bytesPool);
                            builder.WriteCompleted(result);
                        }, cancellationToken);

                    return;
                }
                catch (Exception e)
                {
                    var errorMessage = _errorMessageFactory.Create(e);

                    await _connection.Sender.SendAsync(
                        bufferWriter =>
                        {
                            var builder = new PacketBuilder(bufferWriter, _bytesPool);
                            builder.WriteError(errorMessage);
                        }, cancellationToken);
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }

            public async ValueTask ListenActionAsync<TParam, TError>(Func<TParam, CancellationToken, ValueTask> callback, IErrorMessageFactory<TError> _errorMessageFactory, CancellationToken cancellationToken = default)
                where TParam : IRocketMessage<TParam>
                where TError : IRocketMessage<TError>
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                using var onClose = _connection.Subscribers.OnClosed.Subscribe(() => linkedTokenSource.Cancel());

                var param = ParsedPacketMessage<TParam, TError>.CreateUnknown();

                await _connection.Receiver.ReceiveAsync(
                    sequence =>
                    {
                        _ = PacketParser.TryParse(ref sequence, out param, _bytesPool);
                    }, cancellationToken);

                if (param.IsCompleted)
                {
                    try
                    {
                        await callback.Invoke(param.Message, linkedTokenSource.Token);

                        await _connection.Sender.SendAsync(
                            bufferWriter =>
                            {
                                var builder = new PacketBuilder(bufferWriter, _bytesPool);
                                builder.WriteCompleted();
                            }, cancellationToken);

                        return;
                    }
                    catch (Exception e)
                    {
                        var errorMessage = _errorMessageFactory.Create(e);

                        await _connection.Sender.SendAsync(
                            bufferWriter =>
                            {
                                var builder = new PacketBuilder(bufferWriter, _bytesPool);
                                builder.WriteError(errorMessage);
                            }, cancellationToken);
                    }
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }

            public async ValueTask ListenActionAsync<TError>(Func<CancellationToken, ValueTask> callback, IErrorMessageFactory<TError> _errorMessageFactory, CancellationToken cancellationToken = default)
                where TError : IRocketMessage<TError>
            {
                using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                using var onClose = _connection.Subscribers.OnClosed.Subscribe(() => linkedTokenSource.Cancel());

                try
                {
                    await callback.Invoke(linkedTokenSource.Token);

                    await _connection.Sender.SendAsync(
                        bufferWriter =>
                        {
                            var builder = new PacketBuilder(bufferWriter, _bytesPool);
                            builder.WriteCompleted();
                        }, cancellationToken);

                    return;
                }
                catch (Exception e)
                {
                    var errorMessage = _errorMessageFactory.Create(e);

                    await _connection.Sender.SendAsync(
                        bufferWriter =>
                        {
                            var builder = new PacketBuilder(bufferWriter, _bytesPool);
                            builder.WriteError(errorMessage);
                        }, cancellationToken);
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }
        }
    }
}
