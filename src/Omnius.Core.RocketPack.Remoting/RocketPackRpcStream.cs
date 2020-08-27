using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Network.Connections;
using Omnius.Core.RocketPack.Remoting.Internal;

namespace Omnius.Core.RocketPack.Remoting
{
    public sealed partial class RocketPackRpcStream : DisposableBase
    {
        private readonly IConnection _connection;
        private readonly IBytesPool _bytesPool;

        internal RocketPackRpcStream(IConnection connection, IBytesPool bytesPool)
        {
            _connection = connection;
            _bytesPool = bytesPool;
        }

        public async ValueTask<TResult> RequestFunctionAsync<TParam, TResult>(TParam param, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
            where TResult : IRocketPackObject<TResult>
        {
            await this.SendCompletedAsync(param, cancellationToken);

            var receivedResult = await this.ReceiveAsync<TResult>(cancellationToken);

            if (receivedResult.IsCompleted)
            {
                return receivedResult.Message!;
            }
            else if (receivedResult.IsError)
            {
                throw ThrowHelper.CreateRocketPackRpcApplicationException(receivedResult.ErrorMessage!);
            }

            throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
        }

        public async ValueTask ResponceFunctionAsync<TParam, TResult>(Func<TParam, ValueTask<TResult>> func, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
            where TResult : IRocketPackObject<TResult>
        {
            var receivedParam = await this.ReceiveAsync<TParam>(cancellationToken);

            if (receivedParam.IsCompleted)
            {
                try
                {
                    var result = await func.Invoke(receivedParam.Message!);
                    await this.SendCompletedAsync(result, cancellationToken);
                }
                catch (Exception e)
                {
                    await this.SendErrorAsync(this.CreateErrorMessage(e));
                }
            }

            throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
        }

        public async ValueTask<TResult> RequestFunctionAsync<TResult>(CancellationToken cancellationToken = default)
            where TResult : IRocketPackObject<TResult>
        {
            var receivedResult = await this.ReceiveAsync<TResult>(cancellationToken);

            if (receivedResult.IsCompleted)
            {
                return receivedResult.Message!;
            }
            else if (receivedResult.IsError)
            {
                throw ThrowHelper.CreateRocketPackRpcApplicationException(receivedResult.ErrorMessage!);
            }

            throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
        }

        public async ValueTask ResponceFunctionAsync<TResult>(Func<ValueTask<TResult>> func, CancellationToken cancellationToken = default)
            where TResult : IRocketPackObject<TResult>
        {
            try
            {
                var result = await func.Invoke();
                await this.SendCompletedAsync(result, cancellationToken);
            }
            catch (Exception e)
            {
                await this.SendErrorAsync(this.CreateErrorMessage(e));
            }
        }

        public async ValueTask RequestActionAsync<TParam>(TParam param, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
        {
            await this.SendCompletedAsync(param, cancellationToken);

            var receivedResult = await this.ReceiveAsync(cancellationToken);

            if (receivedResult.IsCompleted)
            {
                return;
            }
            else if (receivedResult.IsError)
            {
                throw ThrowHelper.CreateRocketPackRpcApplicationException(receivedResult.ErrorMessage!);
            }

            throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
        }

        public async ValueTask ResponceActionAsync<TParam>(Func<TParam, ValueTask> func, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
        {
            var receivedParam = await this.ReceiveAsync<TParam>(cancellationToken);

            if (receivedParam.IsCompleted)
            {
                try
                {
                    await func.Invoke(receivedParam.Message!);
                    await this.SendCompletedAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    await this.SendErrorAsync(this.CreateErrorMessage(e));
                }
            }

            throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
        }

        public async ValueTask RequestActionAsync(CancellationToken cancellationToken = default)
        {
            var receivedResult = await this.ReceiveAsync(cancellationToken);

            if (receivedResult.IsCompleted)
            {
                return;
            }
            else if (receivedResult.IsError)
            {
                throw ThrowHelper.CreateRocketPackRpcApplicationException(receivedResult.ErrorMessage!);
            }

            throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
        }

        public async ValueTask ResponceActionAsync<TParam>(Func<ValueTask> func, CancellationToken cancellationToken = default)
        {
            var receivedParam = await this.ReceiveAsync(cancellationToken);

            if (receivedParam.IsCompleted)
            {
                try
                {
                    await func.Invoke();
                    await this.SendCompletedAsync(cancellationToken);
                }
                catch (Exception e)
                {
                    await this.SendErrorAsync(this.CreateErrorMessage(e));
                }
            }
            else
            {
                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }
        }

        private RocketPackRpcErrorMessage CreateErrorMessage(Exception e)
        {
            return new RocketPackRpcErrorMessage(e.GetType()?.FullName ?? string.Empty, e.Message, e.StackTrace);
        }

        private enum PacketType : byte
        {
            None = 0,
            Completed = 1,
            Continue = 2,
            Error = 3,
        }

        public async ValueTask SendCompletedAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                var type = (byte)PacketType.Completed;
                Varint.SetUInt8(in type, in bufferWriter);
                var writer = new RocketPackObjectWriter(bufferWriter, _bytesPool);
                IRocketPackObject<TMessage>.Formatter.Serialize(ref writer, message, 0);
            }, cancellationToken);
        }

        public async ValueTask SendCompletedAsync(CancellationToken cancellationToken = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                var type = (byte)PacketType.Completed;
                Varint.SetUInt8(in type, in bufferWriter);
            }, cancellationToken);
        }

        public async ValueTask SendContinueAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                var type = (byte)PacketType.Continue;
                Varint.SetUInt8(in type, in bufferWriter);
                var writer = new RocketPackObjectWriter(bufferWriter, _bytesPool);
                IRocketPackObject<TMessage>.Formatter.Serialize(ref writer, message, 0);
            }, cancellationToken);
        }

        public async ValueTask SendContinueAsync(CancellationToken cancellationToken = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                var type = (byte)PacketType.Continue;
                Varint.SetUInt8(in type, in bufferWriter);
            }, cancellationToken);
        }

        public async ValueTask SendErrorAsync(RocketPackRpcErrorMessage errorMessage, CancellationToken cancellationToken = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                var type = (byte)PacketType.Error;
                Varint.SetUInt8(in type, in bufferWriter);
                errorMessage.Export(bufferWriter, _bytesPool);
            }, cancellationToken);
        }

        public async ValueTask<RocketPackRpcStreamReceiveResult> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            RocketPackRpcStreamReceiveResult receiveResult = default;

            await _connection.DequeueAsync((sequence) =>
            {
                var sequenceReader = new SequenceReader<byte>(sequence);

                if (!Varint.TryGetUInt8(ref sequenceReader, out var type)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                switch ((PacketType)type)
                {
                    case PacketType.Completed:
                        if (sequenceReader.Remaining == 0)
                        {
                            receiveResult = RocketPackRpcStreamReceiveResult.CreateCompleted();
                            return;
                        }
                        break;
                    case PacketType.Continue:
                        if (sequenceReader.Remaining == 0)
                        {
                            receiveResult = RocketPackRpcStreamReceiveResult.CreateContinue();
                            return;
                        }
                        break;
                    case PacketType.Error:
                        var errorMessage = RocketPackRpcErrorMessage.Import(sequenceReader.Sequence, _bytesPool);
                        receiveResult = RocketPackRpcStreamReceiveResult.CreateError(errorMessage);
                        return;
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }, cancellationToken);

            return receiveResult;
        }

        public async ValueTask<RocketPackRpcStreamReceiveResult<TMessage>> ReceiveAsync<TMessage>(CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            RocketPackRpcStreamReceiveResult<TMessage> receiveResult = default;

            await _connection.DequeueAsync((sequence) =>
            {
                var sequenceReader = new SequenceReader<byte>(sequence);

                if (!Varint.TryGetUInt8(ref sequenceReader, out var type)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                switch ((PacketType)type)
                {
                    case PacketType.Completed:
                        if (sequenceReader.Remaining > 0)
                        {
                            var reader = new RocketPackObjectReader(sequenceReader.Sequence, _bytesPool);
                            var message = IRocketPackObject<TMessage>.Formatter.Deserialize(ref reader, 0);
                            receiveResult = RocketPackRpcStreamReceiveResult<TMessage>.CreateCompleted(message);
                            return;
                        }
                        else
                        {
                            receiveResult = RocketPackRpcStreamReceiveResult<TMessage>.CreateCompleted();
                            return;
                        }
                    case PacketType.Continue:
                        if (sequenceReader.Remaining > 0)
                        {
                            var reader = new RocketPackObjectReader(sequenceReader.Sequence, _bytesPool);
                            var message = IRocketPackObject<TMessage>.Formatter.Deserialize(ref reader, 0);
                            receiveResult = RocketPackRpcStreamReceiveResult<TMessage>.CreateContinue(message);
                            return;
                        }
                        else
                        {
                            receiveResult = RocketPackRpcStreamReceiveResult<TMessage>.CreateContinue();
                            return;
                        }
                    case PacketType.Error:
                        var errorMessage = RocketPackRpcErrorMessage.Import(sequenceReader.Sequence, _bytesPool);
                        receiveResult = RocketPackRpcStreamReceiveResult<TMessage>.CreateError(errorMessage);
                        return;
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }, cancellationToken);

            return receiveResult;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }
        }
    }
}
