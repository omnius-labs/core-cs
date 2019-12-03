using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Extensions;
using Omnius.Core.Helpers;
using Omnius.Core.Network.Connections.Secure.Internal;

namespace Omnius.Core.Network.Connections.Secure
{
    public sealed class OmniSecureConnection : DisposableBase, IConnection
    {
        private readonly IConnection _connection;
        private readonly OmniSecureConnectionOptions _options;
        private readonly IBufferPool<byte> _bufferPool;

        private OmniSecureConnectionVersion _version = OmniSecureConnectionVersion.Version1;
        private V1.Internal.SecureConnection? _secureConnection_v1;

        public OmniSecureConnection(IConnection connection, OmniSecureConnectionOptions options)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (!EnumHelper.IsValid(options.Type))
            {
                throw new ArgumentException(nameof(options.Type));
            }

            _connection = connection;
            _options = options;
            _bufferPool = options.BufferPool ?? BufferPool<byte>.Shared;
        }

        public IConnection BaseConnection => _connection;

        public bool IsConnected => _connection.IsConnected;

        public long TotalBytesSent => _connection.TotalBytesSent;

        public long TotalBytesReceived => _connection.TotalBytesReceived;

        public OmniSecureConnectionType Type => _options.Type;

        public IEnumerable<string> MatchedPasswords
        {
            get
            {
                if (_secureConnection_v1 != null)
                {
                    return _secureConnection_v1.MatchedPasswords;
                }

                return Enumerable.Empty<string>();
            }
        }

        private static T GetOverlapMaxEnum<T>(IEnumerable<T> s1, IEnumerable<T> s2)
            where T : Enum
        {
            var list = s1.ToList();
            list.Sort((x, y) => y.CompareTo(x));

            var hashSet = new HashSet<T>(s2);

            foreach (var item in list)
            {
                if (hashSet.Contains(item))
                {
                    return item;
                }
            }

            throw new OmniSecureConnectionException($"Overlap enum of {nameof(T)} could not be found.");
        }

        public void DoEvents()
        {
            _connection.DoEvents();
        }

        public async ValueTask Handshake(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.Hello(cancellationToken);

                if (_version == OmniSecureConnectionVersion.Version1)
                {
                    _secureConnection_v1 = new V1.Internal.SecureConnection(_connection, _options);
                    await _secureConnection_v1.Handshake(cancellationToken);
                }
                else
                {
                    throw new NotSupportedException("Not supported OmniSecureConnectionVersion.");
                }
            }
            catch (OmniSecureConnectionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new OmniSecureConnectionException($"Handshake of {nameof(OmniSecureConnection)} failed.", e);
            }
        }

        private async ValueTask Hello(CancellationToken cancellationToken)
        {
            HelloMessage sendHelloMessage;
            HelloMessage? receiveHelloMessage = null;
            {
                sendHelloMessage = new HelloMessage(new[] { _version });

                var enqueueTask = _connection.SendAsync((bufferWriter) => sendHelloMessage.Export(bufferWriter, _bufferPool), cancellationToken);
                var dequeueTask = _connection.ReceiveAsync((sequence) => receiveHelloMessage = HelloMessage.Import(sequence, _bufferPool), cancellationToken);

                await ValueTaskHelper.WhenAll(enqueueTask, dequeueTask);

                if (receiveHelloMessage is null)
                {
                    throw new NullReferenceException();
                }
            }

            _version = GetOverlapMaxEnum(sendHelloMessage.Versions, receiveHelloMessage.Versions);
        }

        public async ValueTask SendAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
        {
            if (_secureConnection_v1 != null)
            {
                await _secureConnection_v1.SendAsync(action, cancellationToken);
            }
            else
            {
                throw new NotSupportedException("Not supported OmniSecureConnectionVersion.");
            }
        }

        public async ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default)
        {
            if (_secureConnection_v1 != null)
            {
                await _secureConnection_v1.ReceiveAsync(action, cancellationToken);
            }
            else
            {
                throw new NotSupportedException("Not supported OmniSecureConnectionVersion.");
            }
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _secureConnection_v1?.Dispose();
            }
        }

        protected override async ValueTask OnDisposeAsync()
        {
            if (_secureConnection_v1 != null)
            {
                await _secureConnection_v1.DisposeAsync();
            }
        }
    }
}
