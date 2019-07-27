using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Base.Extensions;
using Omnix.Base.Helpers;
using Omnix.Network.Connections.Secure.Internal;

namespace Omnix.Network.Connections.Secure
{
    public sealed class OmniSecureConnection : DisposableBase, IConnection
    {
        private readonly IConnection _connection;
        private readonly OmniSecureConnectionOptions _options;
        private readonly BufferPool _bufferPool;

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
            _bufferPool = options.BufferPool ?? BufferPool.Shared;
        }

        public IConnection BaseConnection => _connection;

        public bool IsConnected => _connection.IsConnected;
        public long ReceivedByteCount => _connection.ReceivedByteCount;
        public long SentByteCount => _connection.SentByteCount;

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

        public async ValueTask Handshake(CancellationToken token = default)
        {
            try
            {
                await this.Hello(token);

                if (_version == OmniSecureConnectionVersion.Version1)
                {
                    _secureConnection_v1 = new V1.Internal.SecureConnection(_connection, _options);
                    await _secureConnection_v1.Handshake(token);
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

        private async ValueTask Hello(CancellationToken token)
        {
            HelloMessage sendHelloMessage;
            HelloMessage? receiveHelloMessage = null;
            {
                sendHelloMessage = new HelloMessage(new[] { _version });

                var enqueueTask = _connection.EnqueueAsync((bufferWriter) => sendHelloMessage.Export(bufferWriter, _bufferPool), token);
                var dequeueTask = _connection.DequeueAsync((sequence) => receiveHelloMessage = HelloMessage.Import(sequence, _bufferPool), token);

                await ValueTaskHelper.WhenAll(enqueueTask, dequeueTask);

                if (receiveHelloMessage is null)
                {
                    throw new NullReferenceException();
                }
            }

            _version = GetOverlapMaxEnum(sendHelloMessage.Versions, receiveHelloMessage.Versions);
        }

        public bool TryEnqueue(Action<IBufferWriter<byte>> action)
        {
            if (_secureConnection_v1 != null)
            {
                return _secureConnection_v1.TryEnqueue(action);
            }
            else
            {
                throw new NotSupportedException("Not supported OmniSecureConnectionVersion.");
            }
        }

        public async ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            if (_secureConnection_v1 != null)
            {
                await _secureConnection_v1.EnqueueAsync(action, token);
            }
            else
            {
                throw new NotSupportedException("Not supported OmniSecureConnectionVersion.");
            }
        }

        public void Enqueue(Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            if (_secureConnection_v1 != null)
            {
                _secureConnection_v1.Enqueue(action, token);
            }
            else
            {
                throw new NotSupportedException("Not supported OmniSecureConnectionVersion.");
            }
        }

        public bool TryDequeue(Action<ReadOnlySequence<byte>> action)
        {
            if (_secureConnection_v1 != null)
            {
                return _secureConnection_v1.TryDequeue(action);
            }
            else
            {
                throw new NotSupportedException("Not supported OmniSecureConnectionVersion.");
            }
        }

        public async ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
            if (_secureConnection_v1 != null)
            {
                await _secureConnection_v1.DequeueAsync(action, token);
            }
            else
            {
                throw new NotSupportedException("Not supported OmniSecureConnectionVersion.");
            }
        }

        public void Dequeue(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
            if (_secureConnection_v1 != null)
            {
                _secureConnection_v1.Dequeue(action, token);
            }
            else
            {
                throw new NotSupportedException("Not supported OmniSecureConnectionVersion.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _secureConnection_v1?.Dispose();
            }
        }
    }

    public class OmniSecureConnectionException : Exception
    {
        public OmniSecureConnectionException() : base() { }
        public OmniSecureConnectionException(string message) : base(message) { }
        public OmniSecureConnectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
