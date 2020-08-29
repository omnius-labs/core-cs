using System.Runtime.InteropServices.ComTypes;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Network.Connections;
using Omnius.Core.RocketPack.Remoting.Internal;
using System.Threading.Channels;

namespace Omnius.Core.RocketPack.Remoting
{
    public sealed partial class RocketPackRpcStream : DisposableBase
    {
        private readonly RocketPackRpc _rpc;
        private readonly IBytesPool _bytesPool;

        private readonly Channel<ArraySegment<byte>> _receivedMessageChannel = Channel.CreateBounded<ArraySegment<byte>>(10);
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        internal RocketPackRpcStream(uint id, uint callId, RocketPackRpc rpc, IBytesPool bytesPool)
        {
            this.Id = id;
            this.CallId = callId;
            _rpc = rpc;
            _bytesPool = bytesPool;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _receivedMessageChannel.Writer.Complete();

                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
        }

        internal uint Id { get; }
        internal uint CallId { get; }

        internal async ValueTask OnReceivedMessageAsync(ArraySegment<byte> receivedMessage)
        {
            this.ThrowIfDisposingRequested();

            await _receivedMessageChannel.Writer.WriteAsync(receivedMessage);
        }

        internal void OnReceivedClose()
        {
            this.ThrowIfDisposingRequested();

            _cancellationTokenSource.Cancel();
        }

        public async ValueTask<TResult> RequestFunctionAsync<TParam, TResult>(TParam param, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
            where TResult : IRocketPackObject<TResult>
        {
            this.ThrowIfDisposingRequested();

            try
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
            catch (OperationCanceledException)
            {
                await _rpc.SendCloseAsync(this.Id);
                throw;
            }
        }

        public async ValueTask ResponceFunctionAsync<TParam, TResult>(Func<TParam, ValueTask<TResult>> func, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
            where TResult : IRocketPackObject<TResult>
        {
            this.ThrowIfDisposingRequested();

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
            this.ThrowIfDisposingRequested();

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
            this.ThrowIfDisposingRequested();

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
            this.ThrowIfDisposingRequested();

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
            this.ThrowIfDisposingRequested();

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
            this.ThrowIfDisposingRequested();

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
            this.ThrowIfDisposingRequested();

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

        private async ValueTask SendCompletedAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            await _rpc.SendMessageAsync(this.Id, (bufferWriter) =>
            {
                var type = (byte)PacketType.Completed;
                Varint.SetUInt8(in type, in bufferWriter);
                message.Export(bufferWriter, _bytesPool);
            }, cancellationToken);
        }

        private async ValueTask SendCompletedAsync(CancellationToken cancellationToken = default)
        {
            await _rpc.SendMessageAsync(this.Id, (bufferWriter) =>
            {
                var type = (byte)PacketType.Completed;
                Varint.SetUInt8(in type, in bufferWriter);
            }, cancellationToken);
        }

        private async ValueTask SendContinueAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            await _rpc.SendMessageAsync(this.Id, (bufferWriter) =>
            {
                var type = (byte)PacketType.Continue;
                Varint.SetUInt8(in type, in bufferWriter);
                message.Export(bufferWriter, _bytesPool);
            }, cancellationToken);
        }

        private async ValueTask SendContinueAsync(CancellationToken cancellationToken = default)
        {
            await _rpc.SendMessageAsync(this.Id, (bufferWriter) =>
            {
                var type = (byte)PacketType.Continue;
                Varint.SetUInt8(in type, in bufferWriter);
            }, cancellationToken);
        }

        private async ValueTask SendErrorAsync(RocketPackRpcErrorMessage errorMessage, CancellationToken cancellationToken = default)
        {
            await _rpc.SendMessageAsync(this.Id, (bufferWriter) =>
            {
                var type = (byte)PacketType.Error;
                Varint.SetUInt8(in type, in bufferWriter);
                errorMessage.Export(bufferWriter, _bytesPool);
            }, cancellationToken);
        }

        private async ValueTask<RocketPackRpcStreamReceiveResult> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            var receivedMessage = await _receivedMessageChannel.Reader.ReadAsync(cancellationToken);

            try
            {
                var sequence = new ReadOnlySequence<byte>(receivedMessage);
                if (sequence.Length == 0) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                if (!Varint.TryGetUInt8(ref sequence, out var type)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                switch ((PacketType)type)
                {
                    case PacketType.Completed:
                        if (sequence.Length == 0)
                        {
                            return RocketPackRpcStreamReceiveResult.CreateCompleted();
                        }
                        break;
                    case PacketType.Continue:
                        if (sequence.Length == 0)
                        {
                            return RocketPackRpcStreamReceiveResult.CreateContinue();
                        }
                        break;
                    case PacketType.Error:
                        var errorMessage = RocketPackRpcErrorMessage.Import(sequence, _bytesPool);
                        return RocketPackRpcStreamReceiveResult.CreateError(errorMessage);
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }
            finally
            {
                _bytesPool.Array.Return(receivedMessage.Array!);
            }
        }

        public async ValueTask<RocketPackRpcStreamReceiveResult<TMessage>> ReceiveAsync<TMessage>(CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            var receivedMessage = await _receivedMessageChannel.Reader.ReadAsync(cancellationToken);

            try
            {
                var sequence = new ReadOnlySequence<byte>(receivedMessage);
                if (sequence.Length == 0) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                if (!Varint.TryGetUInt8(ref sequence, out var type)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                switch ((PacketType)type)
                {
                    case PacketType.Completed:
                        if (sequence.Length == 0)
                        {
                            var message = IRocketPackObject<TMessage>.Import(sequence, _bytesPool);
                            return RocketPackRpcStreamReceiveResult<TMessage>.CreateCompleted(message);
                        }
                        else
                        {
                            return RocketPackRpcStreamReceiveResult<TMessage>.CreateCompleted();
                        }
                    case PacketType.Continue:
                        if (sequence.Length == 0)
                        {
                            var message = IRocketPackObject<TMessage>.Import(sequence, _bytesPool);
                            return RocketPackRpcStreamReceiveResult<TMessage>.CreateContinue(message);
                        }
                        else
                        {
                            return RocketPackRpcStreamReceiveResult<TMessage>.CreateContinue();
                        }
                    case PacketType.Error:
                        var errorMessage = RocketPackRpcErrorMessage.Import(sequence, _bytesPool);
                        return RocketPackRpcStreamReceiveResult<TMessage>.CreateError(errorMessage);
                }

                throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
            }
            finally
            {
                _bytesPool.Array.Return(receivedMessage.Array!);
            }
        }
    }
}
