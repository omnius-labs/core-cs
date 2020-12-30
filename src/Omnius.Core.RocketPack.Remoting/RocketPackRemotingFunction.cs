using System;
using System.Buffers;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Omnius.Core.RocketPack.Remoting.Internal;

namespace Omnius.Core.RocketPack.Remoting
{
    public sealed partial class RocketPackRemotingFunction : DisposableBase
    {
        private readonly RocketPackRemoting.DataStream _stream;
        private readonly IBytesPool _bytesPool;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        internal RocketPackRemotingFunction(RocketPackRemoting.DataStream stream, IBytesPool bytesPool)
        {
            _stream = stream;
            _bytesPool = bytesPool;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
            }
        }

        public uint Id => _stream.FunctionId;

        public async ValueTask<TResult> CallFunctionAsync<TParam, TResult>(TParam param, CancellationToken cancellationToken = default)
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
                await _stream.SendCloseAsync();
                throw;
            }
        }

        public async ValueTask ListenFunctionAsync<TParam, TResult>(Func<TParam, CancellationToken, ValueTask<TResult>> func, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
            where TResult : IRocketPackObject<TResult>
        {
            this.ThrowIfDisposingRequested();

            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            var receivedParam = await this.ReceiveAsync<TParam>(linkedTokenSource.Token);

            if (receivedParam.IsCompleted)
            {
                try
                {
                    var result = await func.Invoke(receivedParam.Message!, linkedTokenSource.Token);
                    await this.SendCompletedAsync(result, linkedTokenSource.Token);
                    return;
                }
                catch (Exception e)
                {
                    await this.SendErrorAsync(this.CreateErrorMessage(e));
                }
            }

            throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
        }

        public async ValueTask<TResult> CallFunctionAsync<TResult>(CancellationToken cancellationToken = default)
            where TResult : IRocketPackObject<TResult>
        {
            this.ThrowIfDisposingRequested();

            try
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
            catch (OperationCanceledException)
            {
                await _stream.SendCloseAsync();
                throw;
            }
        }

        public async ValueTask ListenFunctionAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> func, CancellationToken cancellationToken = default)
            where TResult : IRocketPackObject<TResult>
        {
            this.ThrowIfDisposingRequested();

            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

            try
            {
                var result = await func.Invoke(linkedTokenSource.Token);
                await this.SendCompletedAsync(result, linkedTokenSource.Token);
                return;
            }
            catch (Exception e)
            {
                await this.SendErrorAsync(this.CreateErrorMessage(e));
            }
        }

        public async ValueTask CallActionAsync<TParam>(TParam param, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
        {
            this.ThrowIfDisposingRequested();

            try
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
            catch (OperationCanceledException)
            {
                await _stream.SendCloseAsync();
                throw;
            }
        }

        public async ValueTask ListenActionAsync<TParam>(Func<TParam, CancellationToken, ValueTask> func, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
        {
            this.ThrowIfDisposingRequested();

            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            var receivedParam = await this.ReceiveAsync<TParam>(linkedTokenSource.Token);

            if (receivedParam.IsCompleted)
            {
                try
                {
                    await func.Invoke(receivedParam.Message!, linkedTokenSource.Token);
                    await this.SendCompletedAsync(linkedTokenSource.Token);
                    return;
                }
                catch (Exception e)
                {
                    await this.SendErrorAsync(this.CreateErrorMessage(e));
                }
            }

            throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
        }

        public async ValueTask CallActionAsync(CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposingRequested();

            try
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
            catch (OperationCanceledException)
            {
                await _stream.SendCloseAsync();
                throw;
            }
        }

        public async ValueTask ListenActionAsync(Func<CancellationToken, ValueTask> func, CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposingRequested();

            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

            try
            {
                await func.Invoke(linkedTokenSource.Token);
                await this.SendCompletedAsync(linkedTokenSource.Token);
                return;
            }
            catch (Exception e)
            {
                await this.SendErrorAsync(this.CreateErrorMessage(e));
            }
        }

        private RocketPackRpcErrorMessage CreateErrorMessage(Exception e)
        {
            return new RocketPackRpcErrorMessage(e.GetType()?.FullName ?? string.Empty, e.Message, e.StackTrace);
        }

        private enum FunctionPacketType : byte
        {
            Unknown = 0,
            Completed = 1,
            Continue = 2,
            Error = 3,
        }

        private async ValueTask SendCompletedAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            await _stream.SendMessageAsync(
                (bufferWriter) =>
                {
                    var type = (byte)FunctionPacketType.Completed;
                    Varint.SetUInt8(in type, in bufferWriter);
                    message.Export(bufferWriter, _bytesPool);
                }, cancellationToken);
        }

        private async ValueTask SendCompletedAsync(CancellationToken cancellationToken = default)
        {
            await _stream.SendMessageAsync(
                (bufferWriter) =>
                {
                    var type = (byte)FunctionPacketType.Completed;
                    Varint.SetUInt8(in type, in bufferWriter);
                }, cancellationToken);
        }

        private async ValueTask SendContinueAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            await _stream.SendMessageAsync(
                (bufferWriter) =>
                {
                    var type = (byte)FunctionPacketType.Continue;
                    Varint.SetUInt8(in type, in bufferWriter);
                    message.Export(bufferWriter, _bytesPool);
                }, cancellationToken);
        }

        private async ValueTask SendContinueAsync(CancellationToken cancellationToken = default)
        {
            await _stream.SendMessageAsync(
                (bufferWriter) =>
                {
                    var type = (byte)FunctionPacketType.Continue;
                    Varint.SetUInt8(in type, in bufferWriter);
                }, cancellationToken);
        }

        private async ValueTask SendErrorAsync(RocketPackRpcErrorMessage errorMessage, CancellationToken cancellationToken = default)
        {
            await _stream.SendMessageAsync(
                (bufferWriter) =>
                {
                    var type = (byte)FunctionPacketType.Error;
                    Varint.SetUInt8(in type, in bufferWriter);
                    errorMessage.Export(bufferWriter, _bytesPool);
                }, cancellationToken);
        }

        private async ValueTask<ReceiveResult> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            var receivedMessage = await _stream.ReceiveAsync(cancellationToken);

            try
            {
                var sequence = new ReadOnlySequence<byte>(receivedMessage);
                if (sequence.Length == 0)
                {
                    throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                }

                if (!Varint.TryGetUInt8(ref sequence, out var type))
                {
                    throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                }

                return ((FunctionPacketType)type) switch
                {
                    FunctionPacketType.Completed when (sequence.Length == 0) => ReceiveResult.CreateCompleted(),
                    FunctionPacketType.Continue when (sequence.Length == 0) => ReceiveResult.CreateContinue(),
                    FunctionPacketType.Error => ReceiveResult.CreateError(RocketPackRpcErrorMessage.Import(sequence, _bytesPool)),
                    _ => throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol(),
                };
            }
            finally
            {
                _bytesPool.Array.Return(receivedMessage.Array!);
            }
        }

        private async ValueTask<ReceiveResult<TMessage>> ReceiveAsync<TMessage>(CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            var receivedMessage = await _stream.ReceiveAsync(cancellationToken);

            try
            {
                var sequence = new ReadOnlySequence<byte>(receivedMessage);
                if (sequence.Length == 0)
                {
                    throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                }

                if (!Varint.TryGetUInt8(ref sequence, out var type))
                {
                    throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                }

                return ((FunctionPacketType)type) switch
                {
                    FunctionPacketType.Completed when (sequence.Length == 0) => ReceiveResult<TMessage>.CreateCompleted(),
                    FunctionPacketType.Completed when (sequence.Length != 0) => ReceiveResult<TMessage>.CreateCompleted(IRocketPackObject<TMessage>.Import(sequence, _bytesPool)),
                    FunctionPacketType.Continue when (sequence.Length == 0) => ReceiveResult<TMessage>.CreateContinue(),
                    FunctionPacketType.Continue when (sequence.Length != 0) => ReceiveResult<TMessage>.CreateContinue(IRocketPackObject<TMessage>.Import(sequence, _bytesPool)),
                    FunctionPacketType.Error => ReceiveResult<TMessage>.CreateError(RocketPackRpcErrorMessage.Import(sequence, _bytesPool)),
                    _ => throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol(),
                };
            }
            finally
            {
                _bytesPool.Array.Return(receivedMessage.Array!);
            }
        }
    }
}
