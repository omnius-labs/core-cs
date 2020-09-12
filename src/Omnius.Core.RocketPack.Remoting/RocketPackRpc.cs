using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Omnius.Core.Network.Connections;
using Omnius.Core.RocketPack.Remoting.Internal;

namespace Omnius.Core.RocketPack.Remoting
{
    public sealed class RocketPackRpc : AsyncDisposableBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IConnection _connection;
        private readonly IBytesPool _bytesPool;

        private readonly Random _random = new();

        private readonly Task _eventLoopTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new();
        private Exception? _eventLoopException = null;

        private readonly Channel<RocketPackRpcStream> _acceptedStreamChannel = Channel.CreateBounded<RocketPackRpcStream>(10);

        private readonly Dictionary<uint, RocketPackRpcStream> _streamMap = new();

        private readonly object _lockObject = new object();

        public RocketPackRpc(IConnection connection, IBytesPool bytesPool)
        {
            _connection = connection;
            _bytesPool = bytesPool;
            _eventLoopTask = this.EventLoop(_cancellationTokenSource.Token);
        }

        protected override async ValueTask OnDisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            await _eventLoopTask;
            _cancellationTokenSource.Dispose();
            _connection.Dispose();
        }

        private enum PacketType : byte
        {
            Unknown = 0,
            Connect = 1,
            Message = 2,
            Close = 3,
        }

        public async ValueTask<RocketPackRpcStream> ConnectAsync(uint callId, CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposingRequested();
            this.ThrowIfEventLoopWasError();

            RocketPackRpcStream? stream = null;

            lock (_lockObject)
            {
                for (; ; )
                {
                    var id = (uint)_random.Next();
                    if (_streamMap.ContainsKey(id)) continue;

                    stream = new RocketPackRpcStream(id, callId, this, _bytesPool);
                    _streamMap.TryAdd(id, stream);
                    break;
                }
            }

            await _connection.EnqueueAsync((bufferWriter) =>
            {
                var packetType = (byte)PacketType.Connect;

                Varint.SetUInt8(packetType, bufferWriter);
                Varint.SetUInt32(stream.Id, bufferWriter);
                Varint.SetUInt32(callId, bufferWriter);
            }, cancellationToken);

            return stream;
        }

        public async ValueTask<RocketPackRpcStream> AcceptAsync(CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposingRequested();
            this.ThrowIfEventLoopWasError();

            return await _acceptedStreamChannel.Reader.ReadAsync(cancellationToken);
        }

        internal void RemoveStream(RocketPackRpcStream stream)
        {
            lock (_lockObject)
            {
                _streamMap.Remove(stream.Id);
            }
        }

        internal async ValueTask SendMessageAsync(uint streamId, Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
        {
            RocketPackRpcStream? stream = null;

            lock (_lockObject)
            {
                if (!_streamMap.TryGetValue(streamId, out stream)) return;
            }

            await _connection.EnqueueAsync((bufferWriter) =>
            {
                var packetType = (byte)PacketType.Message;

                Varint.SetUInt8(packetType, bufferWriter);
                Varint.SetUInt32(stream.Id, bufferWriter);
                action.Invoke(bufferWriter);
            }, cancellationToken);
        }

        internal async ValueTask SendCloseAsync(uint streamId, CancellationToken cancellationToken = default)
        {
            RocketPackRpcStream? stream = null;

            lock (_lockObject)
            {
                if (!_streamMap.TryGetValue(streamId, out stream)) return;
            }

            await _connection.EnqueueAsync((bufferWriter) =>
            {
                var packetType = (byte)PacketType.Close;

                Varint.SetUInt8(packetType, bufferWriter);
                Varint.SetUInt32(stream.Id, bufferWriter);
            }, cancellationToken);

            lock (_lockObject)
            {
                _streamMap.Remove(streamId);
            }
        }

        private void ThrowIfEventLoopWasError()
        {
            if (_eventLoopException is not null)
            {
                ExceptionDispatchInfo.Throw(_eventLoopException);
            }
        }

        private async Task EventLoop(CancellationToken cancellationToken = default)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await _connection.DequeueAsync(async (sequence) =>
                    {
                        if (!Varint.TryGetUInt8(ref sequence, out var packetType)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();
                        if (!Varint.TryGetUInt32(ref sequence, out var streamId)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                        switch ((PacketType)packetType)
                        {
                            case PacketType.Connect:
                                {
                                    if (!Varint.TryGetUInt32(ref sequence, out var callId)) throw ThrowHelper.CreateRocketPackRpcProtocolException_UnexpectedProtocol();

                                    RocketPackRpcStream stream;

                                    lock (_lockObject)
                                    {
                                        if (_streamMap.ContainsKey(streamId)) return;

                                        stream = new RocketPackRpcStream(streamId, callId, this, _bytesPool);
                                        _streamMap.TryAdd(streamId, stream);
                                    }

                                    await _acceptedStreamChannel.Writer.WriteAsync(stream);
                                }
                                break;
                            case PacketType.Message:
                                {
                                    RocketPackRpcStream? stream;

                                    lock (_lockObject)
                                    {
                                        if (!_streamMap.TryGetValue(streamId, out stream)) return;
                                    }

                                    var buffer = _bytesPool.Array.Rent((int)sequence.Length);
                                    sequence.CopyTo(buffer);
                                    await stream.OnReceivedMessageAsync(new ArraySegment<byte>(buffer, 0, (int)sequence.Length));
                                }
                                break;
                            case PacketType.Close:
                                {
                                    RocketPackRpcStream? stream;

                                    lock (_lockObject)
                                    {
                                        if (!_streamMap.TryGetValue(streamId, out stream)) return;
                                    }

                                    stream.OnReceivedClose();

                                    lock (_lockObject)
                                    {
                                        _streamMap.Remove(streamId);
                                    }
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
    }
}
