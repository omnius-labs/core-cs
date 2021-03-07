using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Omnius.Core.Network.Connections;
using Omnius.Core.RocketPack.Remoting.Internal;

namespace Omnius.Core.RocketPack.Remoting
{
    public interface IRocketPackRemotingFactory
    {
        IRocketPackRemoting Create(IConnection connection, IRocketPackRemotingMessengerFactory messengerFactory, IRocketPackRemotingFunctionFactory functionFactory, IBytesPool bytesPool);
    }

    public interface IRocketPackRemoting : IAsyncDisposable
    {
        ValueTask<IRocketPackRemotingFunction> ConnectAsync(uint functionId, CancellationToken cancellationToken = default);

        ValueTask<IRocketPackRemotingFunction> AcceptAsync(CancellationToken cancellationToken = default);
    }

    public sealed class RocketPackRemoting : AsyncDisposableBase, IRocketPackRemoting
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IConnection _connection;
        private readonly IRocketPackRemotingMessengerFactory _messengerFactory;
        private readonly IRocketPackRemotingFunctionFactory _functionFactory;
        private readonly IBytesPool _bytesPool;

        private readonly Random _random = new();

        private readonly IRocketPackRemotingMessenger _messenger = null!;
        private readonly IRocketPackRemotingMessageReceiver _messageReceiver = null!;

        private Task _eventLoopTask = null!;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly Channel<(uint sessionId, uint functionId)> _acceptedSessionChannel = Channel.CreateBounded<(uint, uint)>(10);
        private readonly ConcurrentDictionary<uint, RocketPackRemotingSession> _sessions = new();

        private uint _currentNextSessionId = 0;

        internal sealed class RocketPackRemotingFactory : IRocketPackRemotingFactory
        {
            public IRocketPackRemoting Create(IConnection connection, IRocketPackRemotingMessengerFactory messengerFactory, IRocketPackRemotingFunctionFactory functionFactory, IBytesPool bytesPool)
            {
                var result = new RocketPackRemoting(connection, messengerFactory, functionFactory, bytesPool);
                return result;
            }
        }

        public static IRocketPackRemotingFactory Factory { get; } = new RocketPackRemotingFactory();

        public RocketPackRemoting(IConnection connection, IRocketPackRemotingMessengerFactory messengerFactory, IRocketPackRemotingFunctionFactory functionFactory, IBytesPool bytesPool)
        {
            _connection = connection;
            _messengerFactory = messengerFactory;
            _functionFactory = functionFactory;
            _bytesPool = bytesPool;

            _messageReceiver = new RocketPackRemotingMessageReceiver(this);
            _messenger = _messengerFactory.Create(_connection, _messageReceiver, _bytesPool);
            _eventLoopTask = this.EventLoopAsync(_cancellationTokenSource.Token);
        }

        protected override async ValueTask OnDisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            await _eventLoopTask;
            _cancellationTokenSource.Dispose();

            _connection.Dispose();
        }

        private uint NextSessionId()
        {
            for (; ; )
            {
                if (!_sessions.ContainsKey(_currentNextSessionId))
                {
                    return _currentNextSessionId++;
                }
            }
        }

        public async ValueTask<IRocketPackRemotingFunction> ConnectAsync(uint functionId, CancellationToken cancellationToken = default)
        {
            var sessionId = this.NextSessionId();
            await _messenger.SendConnectMessageAsync(sessionId, functionId, cancellationToken);

            var session = new RocketPackRemotingSession(sessionId, functionId, this, _bytesPool);
            _sessions.TryAdd(session.Id, session);

            return _functionFactory.Create(session, _bytesPool);
        }

        public async ValueTask<IRocketPackRemotingFunction> AcceptAsync(CancellationToken cancellationToken = default)
        {
            var (sessionId, functionId) = await _acceptedSessionChannel.Reader.ReadAsync(cancellationToken);

            var session = new RocketPackRemotingSession(sessionId, functionId, this, _bytesPool);
            _sessions.TryAdd(sessionId, session);

            return _functionFactory.Create(session, _bytesPool);
        }

        private async Task EventLoopAsync(CancellationToken cancellationToken = default)
        {
            Exception? eventLoopException = null;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    await _messenger.EventLoopAsync(cancellationToken);
                }
            }
            catch (OperationCanceledException e)
            {
                eventLoopException = e;
                _logger.Debug(e);
            }
            catch (Exception e)
            {
                eventLoopException = e;
                _logger.Error(e);
            }
            finally
            {
                if (eventLoopException is not null)
                {
                    _acceptedSessionChannel.Writer.Complete(eventLoopException);
                }
            }
        }

        internal class RocketPackRemotingSession : IRocketPackRemotingSession
        {
            private readonly RocketPackRemoting _remoting;
            private readonly IBytesPool _bytesPool;

            private readonly Channel<ArraySegment<byte>> _receivedDataMessage = Channel.CreateBounded<ArraySegment<byte>>(10);

            public RocketPackRemotingSession(uint id, uint functionId, RocketPackRemoting remoting, IBytesPool bytesPool)
            {
                this.Id = id;
                this.FunctionId = functionId;
                _remoting = remoting;
                _bytesPool = bytesPool;
            }

            public void Dispose()
            {
                _receivedDataMessage.Writer.Complete();

                _remoting._sessions.TryRemove(this.Id, out _);
            }

            public uint Id { get; }

            public uint FunctionId { get; }

            public async ValueTask SendAbortMessageAsync()
            {
                await _remoting._messenger.SendCancelMessageAsync(this.Id);
            }

            internal void OnReceiveAbortMessageEvent()
            {
                this.ReceiveAbortMessageEvent.Invoke();
            }

            public event Action ReceiveAbortMessageEvent = () => { };

            public async ValueTask SendDataMessageAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
            {
                await _remoting._messenger.SendDataMessageAsync(this.Id, action, cancellationToken);
            }

            internal async ValueTask OnReceiveDataMessageAsync(ReadOnlySequence<byte> sequence, CancellationToken cancellationToken = default)
            {
                var buffer = _bytesPool.Array.Rent((int)sequence.Length);
                sequence.CopyTo(buffer);
                await _receivedDataMessage.Writer.WriteAsync(new ArraySegment<byte>(buffer, 0, (int)sequence.Length), cancellationToken);
            }

            public async ValueTask ReceiveDataMessageAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default)
            {
                var buffer = await _receivedDataMessage.Reader.ReadAsync(cancellationToken);
                action.Invoke(new ReadOnlySequence<byte>(buffer.Array!, buffer.Offset, buffer.Count));
            }
        }

        internal class RocketPackRemotingMessageReceiver : IRocketPackRemotingMessageReceiver
        {
            private readonly RocketPackRemoting _remoting;

            public RocketPackRemotingMessageReceiver(RocketPackRemoting remoting)
            {
                _remoting = remoting;
            }

            public async ValueTask OnReceiveConnectMessageAsync(uint sessionId, uint functionId)
            {
                await _remoting._acceptedSessionChannel.Writer.WriteAsync((sessionId, functionId));
            }

            public async ValueTask OnReceiveDataMessageAsync(uint sessionId, ReadOnlySequence<byte> sequence)
            {
                if (!_remoting._sessions.TryGetValue(sessionId, out var session)) return;

                await session.OnReceiveDataMessageAsync(sequence);
            }

            public async ValueTask OnReceiveCancelMessageAsync(uint sessionId)
            {
                if (!_remoting._sessions.TryGetValue(sessionId, out var session)) return;

                session.OnReceiveAbortMessageEvent();
            }
        }
    }
}
