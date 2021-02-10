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
    public sealed partial class RocketPackRemoting : AsyncDisposableBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IConnection _connection;
        private readonly IBytesPool _bytesPool;

        private readonly Random _random = new();

        private readonly Task _eventLoopTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Exception? _eventLoopException = null;

        private readonly Channel<DataStream> _acceptedStreamChannel = Channel.CreateBounded<DataStream>(10);

        private readonly Dictionary<uint, DataStream> _streamMap = new();

        private readonly object _lockObject = new();

        public RocketPackRemoting(IConnection connection, IBytesPool bytesPool)
        {
            _connection = connection;
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
            CloseMessage = 2,
            DataMessage = 3,
        }

        public async ValueTask<RocketPackRemotingFunction> ConnectAsync(uint functionId, CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposingRequested();
            this.ThrowIfEventLoopWasError();

            DataStream? stream = null;

            lock (_lockObject)
            {
                uint streamId;

                for (; ; )
                {
                    streamId = (uint)_random.Next();
                    if (!_streamMap.ContainsKey(streamId)) break;
                }

                stream = new DataStream(streamId, functionId, _connection, () => this.RemoveStream(streamId));
                _streamMap.TryAdd(streamId, stream);
            }

            await _connection.EnqueueAsync(
                (bufferWriter) =>
                {
                    var packetType = (byte)StreamPacketType.ConnectMessage;

                    Varint.SetUInt8(packetType, bufferWriter);
                    Varint.SetUInt32(stream.StreamId, bufferWriter);
                    Varint.SetUInt32(functionId, bufferWriter);
                }, cancellationToken);

            return new RocketPackRemotingFunction(stream, _bytesPool);
        }

        public async ValueTask<RocketPackRemotingFunction> AcceptAsync(CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposingRequested();
            this.ThrowIfEventLoopWasError();

            var stream = await _acceptedStreamChannel.Reader.ReadAsync(cancellationToken);
            return new RocketPackRemotingFunction(stream, _bytesPool);
        }

        private async Task EventLoopAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await _connection.DequeueAsync(
                        async (sequence) =>
                        {
                            if (!Varint.TryGetUInt8(ref sequence, out var packetType)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                            if (!Varint.TryGetUInt32(ref sequence, out var streamId)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                            switch ((StreamPacketType)packetType)
                            {
                                case StreamPacketType.ConnectMessage:
                                    {
                                        if (!Varint.TryGetUInt32(ref sequence, out var callId)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                                        DataStream stream;

                                        lock (_lockObject)
                                        {
                                            if (_streamMap.ContainsKey(streamId)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                                            stream = new DataStream(streamId, callId, _connection, () => this.RemoveStream(streamId));
                                            _streamMap.TryAdd(streamId, stream);
                                        }

                                        await _acceptedStreamChannel.Writer.WriteAsync(stream);
                                    }

                                    break;
                                case StreamPacketType.CloseMessage:
                                    {
                                        DataStream? stream;

                                        lock (_lockObject)
                                        {
                                            if (!_streamMap.TryGetValue(streamId, out stream)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                                        }

                                        stream.OnReceivedCloseMessage();

                                        lock (_lockObject)
                                        {
                                            _streamMap.Remove(streamId);
                                        }
                                    }

                                    break;
                                case StreamPacketType.DataMessage:
                                    {
                                        DataStream? stream;

                                        lock (_lockObject)
                                        {
                                            if (!_streamMap.TryGetValue(streamId, out stream)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                                        }

                                        var buffer = _bytesPool.Array.Rent((int)sequence.Length);
                                        sequence.CopyTo(buffer);
                                        await stream.OnReceivedDataMessageAsync(new ArraySegment<byte>(buffer, 0, (int)sequence.Length));
                                    }

                                    break;
                            }
                        }, cancellationToken);
                }
            }
            catch (OperationCanceledException e)
            {
                _eventLoopException = e;
                _logger.Debug(e);
            }
            catch (Exception e)
            {
                _eventLoopException = e;
                _logger.Error(e);
            }
            finally
            {
                _acceptedStreamChannel.Writer.Complete(_eventLoopException);
            }
        }

        private void ThrowIfEventLoopWasError()
        {
            if (_eventLoopException is not null)
            {
                ExceptionDispatchInfo.Throw(_eventLoopException);
            }
        }

        internal void RemoveStream(uint streamId)
        {
            lock (_lockObject)
            {
                _streamMap.Remove(streamId);
            }
        }
    }
}
