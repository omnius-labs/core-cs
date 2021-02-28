using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Omnius.Core.Network.Connections;
using Omnius.Core.RocketPack.Remoting.Internal;

namespace Omnius.Core.RocketPack.Remoting
{
    public interface IRocketPackRemotingMessengerFactory
    {
        IRocketPackRemotingMessenger Create(IConnection connection, IRocketPackRemotingMessageReceiver receiver, IBytesPool bytesPool);
    }

    public interface IRocketPackRemotingMessageReceiver
    {
        ValueTask OnReceiveConnectMessageAsync(uint sessionId, uint functionId);

        ValueTask OnReceiveDataMessageAsync(uint sessionId, ReadOnlySequence<byte> sequence);

        ValueTask OnReceiveCancelMessageAsync(uint sessionId);
    }

    public interface IRocketPackRemotingMessenger
    {
        Task EventLoopAsync(CancellationToken cancellationToken = default);

        ValueTask SendConnectMessageAsync(uint sessionId, uint functionId, CancellationToken cancellationToken = default);

        ValueTask SendDataMessageAsync(uint sessionId, Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default);

        ValueTask SendCancelMessageAsync(uint sessionId, CancellationToken cancellationToken = default);
    }

    public sealed class RocketPackRemotingMessenger : AsyncDisposableBase, IRocketPackRemotingMessenger
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IConnection _connection;
        private readonly IRocketPackRemotingMessageReceiver _receiver;
        private readonly IBytesPool _bytesPool;

        private readonly Task _eventLoopTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        internal sealed class RocketPackRemotingMessengerFactory : IRocketPackRemotingMessengerFactory
        {
            public IRocketPackRemotingMessenger Create(IConnection connection, IRocketPackRemotingMessageReceiver receiver, IBytesPool bytesPool)
            {
                var result = new RocketPackRemotingMessenger(connection, receiver, bytesPool);
                return result;
            }
        }

        public static IRocketPackRemotingMessengerFactory Factory { get; } = new RocketPackRemotingMessengerFactory();

        public RocketPackRemotingMessenger(IConnection connection, IRocketPackRemotingMessageReceiver receiver, IBytesPool bytesPool)
        {
            _connection = connection;
            _receiver = receiver;
            _bytesPool = bytesPool;

            _eventLoopTask = this.EventLoopAsync(_cancellationTokenSource.Token);
        }

        protected override async ValueTask OnDisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            await _eventLoopTask;
            _cancellationTokenSource.Dispose();

            _connection.Dispose();
        }

        private enum StreamPacketType : byte
        {
            Unknown = 0,
            ConnectMessage = 1,
            DataMessage = 2,
            CancelMessage = 3,
        }

        public async ValueTask SendConnectMessageAsync(uint sessionId, uint functionId, CancellationToken cancellationToken)
        {
            await _connection.EnqueueAsync(
                (bufferWriter) =>
                {
                    var packetType = (byte)StreamPacketType.ConnectMessage;
                    Varint.SetUInt8(packetType, bufferWriter);
                    Varint.SetUInt32(sessionId, bufferWriter);
                    Varint.SetUInt32(functionId, bufferWriter);
                }, cancellationToken);
        }

        public async ValueTask SendDataMessageAsync(uint sessionId, Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
        {
            await _connection.EnqueueAsync(
                (bufferWriter) =>
                {
                    var packetType = (byte)StreamPacketType.DataMessage;
                    Varint.SetUInt8(packetType, bufferWriter);
                    Varint.SetUInt32(sessionId, bufferWriter);
                    action.Invoke(bufferWriter);
                }, cancellationToken);
        }

        public async ValueTask SendCancelMessageAsync(uint sessionId, CancellationToken cancellationToken = default)
        {
            await _connection.EnqueueAsync(
                (bufferWriter) =>
                {
                    var packetType = (byte)StreamPacketType.CancelMessage;
                    Varint.SetUInt8(packetType, bufferWriter);
                    Varint.SetUInt32(sessionId, bufferWriter);
                }, cancellationToken);
        }

        public async Task EventLoopAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await _connection.DequeueAsync(
                    async (sequence) =>
                    {
                        if (!Varint.TryGetUInt8(ref sequence, out var packetType)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                        if (!Varint.TryGetUInt32(ref sequence, out var sessionId)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                        switch ((StreamPacketType)packetType)
                        {
                            case StreamPacketType.ConnectMessage:
                                if (!Varint.TryGetUInt32(ref sequence, out var functionId)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                                await _receiver.OnReceiveConnectMessageAsync(sessionId, functionId);
                                break;
                            case StreamPacketType.DataMessage:
                                await _receiver.OnReceiveDataMessageAsync(sessionId, sequence);
                                break;
                            case StreamPacketType.CancelMessage:
                                await _receiver.OnReceiveCancelMessageAsync(sessionId);
                                break;
                        }
                    }, cancellationToken);
            }
        }
    }
}
