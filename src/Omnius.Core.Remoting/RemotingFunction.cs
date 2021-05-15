using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Remoting.Internal;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Remoting
{
    public interface IRemotingFunctionFactory
    {
        IRemotingFunction Create(IRemotingSession session, IBytesPool bytesPool);
    }

    public interface IRemotingSession : IDisposable
    {
        uint Id { get; }

        uint FunctionId { get; }

        ValueTask SendAbortMessageAsync();

        event Action ReceiveAbortMessageEvent;

        ValueTask SendDataMessageAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default);

        ValueTask ReceiveDataMessageAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default);
    }

    public interface IRemotingFunction : IDisposable
    {
        uint Id { get; }

        ValueTask CallActionAsync(CancellationToken cancellationToken = default);

        ValueTask CallActionAsync<TParam>(TParam param, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>;

        ValueTask<TResult> CallFunctionAsync<TResult>(CancellationToken cancellationToken = default)
            where TResult : IRocketPackObject<TResult>;

        ValueTask<TResult> CallFunctionAsync<TParam, TResult>(TParam param, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
            where TResult : IRocketPackObject<TResult>;

        ValueTask ListenActionAsync(Func<CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default);

        ValueTask ListenActionAsync<TParam>(Func<TParam, CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>;

        ValueTask ListenFunctionAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> callback, CancellationToken cancellationToken = default)
            where TResult : IRocketPackObject<TResult>;

        ValueTask ListenFunctionAsync<TParam, TResult>(Func<TParam, CancellationToken, ValueTask<TResult>> callback, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
            where TResult : IRocketPackObject<TResult>;
    }

    public sealed partial class RemotingFunction : DisposableBase, IRemotingFunction
    {
        private readonly IRemotingSession _session;
        private readonly IBytesPool _bytesPool;

        private readonly CancellationTokenSource _cancellationTokenSource = new();

        internal sealed class RemotingFunctionFactory : IRemotingFunctionFactory
        {
            public IRemotingFunction Create(IRemotingSession session, IBytesPool bytesPool)
            {
                var result = new RemotingFunction(session, bytesPool);
                return result;
            }
        }

        public static IRemotingFunctionFactory Factory { get; } = new RemotingFunctionFactory();

        internal RemotingFunction(IRemotingSession session, IBytesPool bytesPool)
        {
            _session = session;
            _session.ReceiveAbortMessageEvent += this.Abort;
            _bytesPool = bytesPool;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _session.ReceiveAbortMessageEvent -= this.Abort;
                _session.Dispose();
            }
        }

        public uint Id => _session.FunctionId;

        private void Abort()
        {
            _cancellationTokenSource.Cancel();
        }

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
                await _session.SendAbortMessageAsync();
                throw;
            }
        }

        public async ValueTask ListenFunctionAsync<TParam, TResult>(Func<TParam, CancellationToken, ValueTask<TResult>> callback, CancellationToken cancellationToken = default)
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
                    var result = await callback.Invoke(receivedParam.Message!, linkedTokenSource.Token);
                    await this.SendCompletedAsync(result, linkedTokenSource.Token);
                    return;
                }
                catch (Exception e)
                {
                    await this.SendErrorAsync(this.CreateErrorMessage(e), cancellationToken);
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
                await _session.SendAbortMessageAsync();
                throw;
            }
        }

        public async ValueTask ListenFunctionAsync<TResult>(Func<CancellationToken, ValueTask<TResult>> callback, CancellationToken cancellationToken = default)
            where TResult : IRocketPackObject<TResult>
        {
            this.ThrowIfDisposingRequested();

            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

            try
            {
                var result = await callback.Invoke(linkedTokenSource.Token);
                await this.SendCompletedAsync(result, linkedTokenSource.Token);
                return;
            }
            catch (Exception e)
            {
                await this.SendErrorAsync(this.CreateErrorMessage(e), cancellationToken);
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
                await _session.SendAbortMessageAsync();
                throw;
            }
        }

        public async ValueTask ListenActionAsync<TParam>(Func<TParam, CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
            where TParam : IRocketPackObject<TParam>
        {
            this.ThrowIfDisposingRequested();

            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
            var receivedParam = await this.ReceiveAsync<TParam>(linkedTokenSource.Token);

            if (receivedParam.IsCompleted)
            {
                try
                {
                    await callback.Invoke(receivedParam.Message!, linkedTokenSource.Token);
                    await this.SendCompletedAsync(linkedTokenSource.Token);
                    return;
                }
                catch (Exception e)
                {
                    await this.SendErrorAsync(this.CreateErrorMessage(e), cancellationToken);
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
                await _session.SendAbortMessageAsync();
                throw;
            }
        }

        public async ValueTask ListenActionAsync(Func<CancellationToken, ValueTask> callback, CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposingRequested();

            using var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

            try
            {
                await callback.Invoke(linkedTokenSource.Token);
                await this.SendCompletedAsync(linkedTokenSource.Token);
                return;
            }
            catch (Exception e)
            {
                await this.SendErrorAsync(this.CreateErrorMessage(e), cancellationToken);
            }
        }

        private RocketPackRpcErrorMessage CreateErrorMessage(Exception e)
        {
            return new RocketPackRpcErrorMessage(e.GetType()?.FullName ?? string.Empty, e.Message, e.StackTrace);
        }

        private enum FunctionPacketType : byte
        {
            Unknown = 0,
            Cancel = 1,
            Completed = 2,
            Continue = 3,
            Error = 4,
        }

        private async ValueTask SendCompletedAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            await _session.SendDataMessageAsync(
                (bufferWriter) =>
                {
                    var type = (byte)FunctionPacketType.Completed;
                    Varint.SetUInt8(in type, in bufferWriter);
                    message.Export(bufferWriter, _bytesPool);
                }, cancellationToken);
        }

        private async ValueTask SendCompletedAsync(CancellationToken cancellationToken = default)
        {
            await _session.SendDataMessageAsync(
                (bufferWriter) =>
                {
                    var type = (byte)FunctionPacketType.Completed;
                    Varint.SetUInt8(in type, in bufferWriter);
                }, cancellationToken);
        }

        private async ValueTask SendContinueAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            await _session.SendDataMessageAsync(
                (bufferWriter) =>
                {
                    var type = (byte)FunctionPacketType.Continue;
                    Varint.SetUInt8(in type, in bufferWriter);
                    message.Export(bufferWriter, _bytesPool);
                }, cancellationToken);
        }

        private async ValueTask SendContinueAsync(CancellationToken cancellationToken = default)
        {
            await _session.SendDataMessageAsync(
                (bufferWriter) =>
                {
                    var type = (byte)FunctionPacketType.Continue;
                    Varint.SetUInt8(in type, in bufferWriter);
                }, cancellationToken);
        }

        private async ValueTask SendErrorAsync(RocketPackRpcErrorMessage errorMessage, CancellationToken cancellationToken = default)
        {
            await _session.SendDataMessageAsync(
                (bufferWriter) =>
                {
                    var type = (byte)FunctionPacketType.Error;
                    Varint.SetUInt8(in type, in bufferWriter);
                    errorMessage.Export(bufferWriter, _bytesPool);
                }, cancellationToken);
        }

        private async ValueTask<ReceiveResult<TMessage>> ReceiveAsync<TMessage>(CancellationToken cancellationToken = default)
            where TMessage : IRocketPackObject<TMessage>
        {
            ReceiveResult<TMessage>? result = null;

            await _session.ReceiveDataMessageAsync(
                (sequence) =>
                {
                    if (sequence.Length == 0) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                    if (!Varint.TryGetUInt8(ref sequence, out var type)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                    result = ((FunctionPacketType)type) switch
                    {
                        FunctionPacketType.Completed when (sequence.Length == 0) => ReceiveResult<TMessage>.CreateCompleted(),
                        FunctionPacketType.Completed when (sequence.Length != 0) => ReceiveResult<TMessage>.CreateCompleted(IRocketPackObject<TMessage>.Import(sequence, _bytesPool)),
                        FunctionPacketType.Continue when (sequence.Length == 0) => ReceiveResult<TMessage>.CreateContinue(),
                        FunctionPacketType.Continue when (sequence.Length != 0) => ReceiveResult<TMessage>.CreateContinue(IRocketPackObject<TMessage>.Import(sequence, _bytesPool)),
                        FunctionPacketType.Error => ReceiveResult<TMessage>.CreateError(RocketPackRpcErrorMessage.Import(sequence, _bytesPool)),
                        _ => null,
                    };
                }, cancellationToken);

            return result ?? throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
        }

        private async ValueTask<ReceiveResult> ReceiveAsync(CancellationToken cancellationToken = default)
        {
            ReceiveResult? result = null;

            await _session.ReceiveDataMessageAsync(
                (sequence) =>
                {
                    if (sequence.Length == 0) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                    if (!Varint.TryGetUInt8(ref sequence, out var type)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                    result = ((FunctionPacketType)type) switch
                    {
                        FunctionPacketType.Completed when (sequence.Length == 0) => ReceiveResult.CreateCompleted(),
                        FunctionPacketType.Continue when (sequence.Length == 0) => ReceiveResult.CreateContinue(),
                        FunctionPacketType.Error => ReceiveResult.CreateError(RocketPackRpcErrorMessage.Import(sequence, _bytesPool)),
                        _ => null,
                    };
                }, cancellationToken);

            return result ?? throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
        }
    }
}
