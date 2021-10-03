using System;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Connections;
using Omnius.Core.RocketPack.Remoting.Internal;

namespace Omnius.Core.RocketPack.Remoting
{
    internal sealed class RocketRemotingListener<TError> : AsyncDisposableBase, IRocketRemotingListener<TError>
        where TError : IRocketMessage<TError>
    {
        private readonly IConnection _connection;
        private readonly IErrorMessageFactory<TError> _errorMessageFactory;
        private readonly IBytesPool _bytesPool;

        public RocketRemotingListener(IConnection connection, uint functionId, IErrorMessageFactory<TError> errorMessageFactory, IBytesPool bytesPool)
        {
            _connection = connection;
            this.FunctionId = functionId;
            _errorMessageFactory = errorMessageFactory;
            _bytesPool = bytesPool;
        }

        protected override async ValueTask OnDisposeAsync()
        {
            await _connection.DisposeAsync();
        }

        public uint FunctionId { get; }

        public async ValueTask ListenFunctionAsync<TParam, TResult>(Func<TParam, CancellationToken, ValueTask<TResult>> callback, CancellationToken cancellationToken = default)
            where TParam : IRocketMessage<TParam>
            where TResult : IRocketMessage<TResult>
        {
            using var cancellationTokenSource = this.CreateCancellationTokenSource(cancellationToken);
            var connectionCloseWaitTask = this.ConnectionCloseWaitAsync(cancellationTokenSource.Token);

            var param = ParsedPacketMessage<TParam, TError>.CreateUnknown();

            await _connection.Receiver.ReceiveAsync(
                sequence =>
                {
                    PacketParser.TryParse(ref sequence, out param, _bytesPool);
                }, cancellationTokenSource.Token);

            if (param.IsCompleted)
            {
                try
                {
                    var result = await callback.Invoke(param.Message, cancellationTokenSource.Token);

                    await _connection.Sender.SendAsync(
                        bufferWriter =>
                        {
                            var builder = new PacketBuilder(bufferWriter, _bytesPool);
                            builder.WriteCompleted(result);
                        }, cancellationTokenSource.Token);

                    await connectionCloseWaitTask;

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

                    throw;
                }
            }

            throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
        }

        public async ValueTask ListenFunctionAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> callback, CancellationToken cancellationToken = default)
            where TResult : IRocketMessage<TResult>
        {
            using var cancellationTokenSource = this.CreateCancellationTokenSource(cancellationToken);
            var connectionCloseWaitTask = this.ConnectionCloseWaitAsync(cancellationTokenSource.Token);

            try
            {
                var result = await callback.Invoke(cancellationTokenSource.Token);

                await _connection.Sender.SendAsync(
                    bufferWriter =>
                    {
                        var builder = new PacketBuilder(bufferWriter, _bytesPool);
                        builder.WriteCompleted(result);
                    }, cancellationTokenSource.Token);

                await connectionCloseWaitTask;

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

                throw;
            }

            throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
        }

        public async ValueTask ListenActionAsync<TParam>(Func<TParam, CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
            where TParam : IRocketMessage<TParam>
        {
            using var cancellationTokenSource = this.CreateCancellationTokenSource(cancellationToken);
            var connectionCloseWaitTask = this.ConnectionCloseWaitAsync(cancellationTokenSource.Token);

            var param = ParsedPacketMessage<TParam, TError>.CreateUnknown();

            await _connection.Receiver.ReceiveAsync(
                sequence =>
                {
                    PacketParser.TryParse(ref sequence, out param, _bytesPool);
                }, cancellationTokenSource.Token);

            if (param.IsCompleted)
            {
                try
                {
                    await callback.Invoke(param.Message, cancellationTokenSource.Token);

                    await _connection.Sender.SendAsync(
                        bufferWriter =>
                        {
                            var builder = new PacketBuilder(bufferWriter, _bytesPool);
                            builder.WriteCompleted();
                        }, cancellationTokenSource.Token);

                    await connectionCloseWaitTask;

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

                    throw;
                }
            }

            throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
        }

        public async ValueTask ListenActionAsync(Func<CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
        {
            using var cancellationTokenSource = this.CreateCancellationTokenSource(cancellationToken);
            var connectionCloseWaitTask = this.ConnectionCloseWaitAsync(cancellationTokenSource.Token);

            try
            {
                await callback.Invoke(cancellationTokenSource.Token);

                await _connection.Sender.SendAsync(
                    bufferWriter =>
                    {
                        var builder = new PacketBuilder(bufferWriter, _bytesPool);
                        builder.WriteCompleted();
                    }, cancellationTokenSource.Token);

                await connectionCloseWaitTask;

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

                throw;
            }

            throw ThrowHelper.CreateRocketRemotingProtocolException_UnexpectedProtocol();
        }

        private CancellationTokenSource CreateCancellationTokenSource(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            using var onCloseSubscriber = _connection.Events.OnClosed.Subscribe(() => linkedTokenSource.Cancel());
            return linkedTokenSource;
        }

        private async ValueTask ConnectionCloseWaitAsync(CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource();
            using var register = cancellationToken.Register(() => tcs.TrySetCanceled());
            using var onCloseSubscriber = _connection.Events.OnClosed.Subscribe(() => tcs.SetResult());
            await tcs.Task;
        }
    }
}
