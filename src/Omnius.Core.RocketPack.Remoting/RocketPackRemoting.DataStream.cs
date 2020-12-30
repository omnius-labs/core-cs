using System;
using System.Buffers;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Omnius.Core.Network.Connections;
using Omnius.Core.RocketPack.Remoting.Internal;

namespace Omnius.Core.RocketPack.Remoting
{
    public sealed partial class RocketPackRemoting : AsyncDisposableBase
    {
        internal sealed class DataStream : DisposableBase
        {
            private readonly IConnection _connection;
            private readonly Action _disposeCallback;

            private readonly Channel<ArraySegment<byte>> _receivedMessageChannel = Channel.CreateBounded<ArraySegment<byte>>(10);
            private readonly CancellationTokenSource _cancellationTokenSource = new();

            internal DataStream(uint streamId, uint functionId, IConnection connection, Action disposeCallback)
            {
                this.StreamId = streamId;
                this.FunctionId = functionId;
                _connection = connection;
                _disposeCallback = disposeCallback;
            }

            protected override void OnDispose(bool disposing)
            {
                if (disposing)
                {
                    _disposeCallback.Invoke();

                    _receivedMessageChannel.Writer.Complete();

                    _cancellationTokenSource.Cancel();
                    _cancellationTokenSource.Dispose();
                }
            }

            internal uint StreamId { get; }

            public uint FunctionId { get; }

            public async ValueTask SendMessageAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
            {
                await _connection.EnqueueAsync(
                    (bufferWriter) =>
                    {
                        var packetType = (byte)StreamPacketType.DataMessage;
                        Varint.SetUInt8(packetType, bufferWriter);
                        Varint.SetUInt32(this.StreamId, bufferWriter);
                        action.Invoke(bufferWriter);
                    }, cancellationToken);
            }

            public async ValueTask SendCloseAsync(CancellationToken cancellationToken = default)
            {
                await _connection.EnqueueAsync(
                    (bufferWriter) =>
                    {
                        var packetType = (byte)StreamPacketType.CloseMessage;
                        Varint.SetUInt8(packetType, bufferWriter);
                        Varint.SetUInt32(this.StreamId, bufferWriter);
                    }, cancellationToken);
            }

            internal async ValueTask OnReceivedDataMessageAsync(ArraySegment<byte> receivedMessage, CancellationToken cancellationToken = default)
            {
                this.ThrowIfDisposingRequested();

                await _receivedMessageChannel.Writer.WriteAsync(receivedMessage, cancellationToken);
            }

            internal void OnReceivedCloseMessage()
            {
                this.ThrowIfDisposingRequested();

                _cancellationTokenSource.Cancel();
            }

            public async ValueTask<ArraySegment<byte>> ReceiveAsync(CancellationToken cancellationToken = default)
            {
                var receivedMessage = await _receivedMessageChannel.Reader.ReadAsync(cancellationToken);
                return receivedMessage;
            }
        }
    }
}
