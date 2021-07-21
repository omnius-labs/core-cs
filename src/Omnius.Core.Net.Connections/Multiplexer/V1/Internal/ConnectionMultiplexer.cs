using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Helpers;
using Omnius.Core.Net.Connections.Extensions;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal sealed partial class ConnectionMultiplexer : AsyncDisposableBase
    {
        private readonly IConnection _connection;
        private readonly ConnectionMultiplexerV1Options _options;
        private readonly IBytesPool _bytesPool;

        private SessionState? _sessionState;

        private const int FrameSize = 16 * 1024;

        public ConnectionMultiplexer(IConnection connection, ConnectionMultiplexerV1Options options)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

            if (options == null) throw new ArgumentNullException(nameof(options));
            if (!EnumHelper.IsValid(options.Type)) throw new ArgumentException(nameof(options.Type));
            _options = options;
        }

        protected override ValueTask OnDisposeAsync()
        {
            throw new NotImplementedException();
        }

        public async ValueTask Handshake(CancellationToken cancellationToken = default)
        {
            var myProfileMessage = new ProfileMessage(_options.MaxMessageSize, _options.MaxQueueSize);
            var enqueueTask = _connection.EnqueueAsync(myProfileMessage, cancellationToken).AsTask();
            var dequeueTask = _connection.DequeueAsync<ProfileMessage>(cancellationToken).AsTask();

            await Task.WhenAll(enqueueTask, dequeueTask);
            var otherProfileMessage = dequeueTask.Result;

            _sessionState = new SessionState(otherProfileMessage.MaxMessageSize, otherProfileMessage.MaxQueueSize);
        }

        private sealed class SessionState
        {
            public SessionState(uint maxMessageSize, uint maxQueueSize)
            {
                this.MaxMessageSize = maxMessageSize;
                this.MaxQueueSize = maxQueueSize;
            }

            public uint MaxMessageSize { get; }

            public uint MaxQueueSize { get; }
        }
    }
}
