using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Cryptography;
using Omnius.Core.Cryptography.Functions;
using Omnius.Core.Helpers;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Pipelines;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal
{
    public sealed partial class SecureConnection : AsyncDisposableBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IConnection _connection;
        private readonly int _maxReceiveByteCount;
        private readonly OmniSecureConnectionType _type;
        private readonly IReadOnlyList<string> _passwords;
        private readonly IBatchActionDispatcher _batchActionDispatcher;
        private readonly IBytesPool _bytesPool;

        private string[]? _matchedPasswords;

        private ConnectionSender? _sender;
        private ConnectionReceiver? _receiver;
        private ConnectionSubscribers? _subscribers;
        private BatchAction? _batchAction;

        private readonly Random _random = new Random();
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private const int FrameSize = 16 * 1024;

        public SecureConnection(IConnection connection, OmniSecureConnectionOptions options)
        {
            _connection = connection;
            _maxReceiveByteCount = options.MaxReceiveByteCount;
            _type = options.Type;
            _passwords = options.Passwords ?? Array.Empty<string>();
            _batchActionDispatcher = options.BatchActionDispatcher;
            _bytesPool = options.BytesPool;
        }

        protected override async ValueTask OnDisposeAsync()
        {
            if (_batchAction is not null) _batchActionDispatcher.Unregister(_batchAction);

            _cancellationTokenSource.Cancel();
            _subscribers?.Dispose();
            _sender?.Dispose();
            _receiver?.Dispose();
            await _connection.DisposeAsync();
            _cancellationTokenSource.Dispose();
        }

        public bool IsConnected => _connection.IsConnected;

        public IConnectionSender Sender => _sender ?? throw new InvalidOperationException();

        public IConnectionReceiver Receiver => _receiver ?? throw new InvalidOperationException();

        public IConnectionSubscribers Subscribers => _subscribers ?? throw new InvalidOperationException();

        public IEnumerable<string> MatchedPasswords => _matchedPasswords ?? Enumerable.Empty<string>();

        internal static void Increment(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0xff)
                {
                    bytes[i] = 0x00;
                }
                else
                {
                    bytes[i]++;
                    break;
                }
            }
        }

        public async ValueTask Handshake(CancellationToken cancellationToken = default)
        {
            ProfileMessage myProfileMessage;
            ProfileMessage? otherProfileMessage = null;
            {
                {
                    var sessionId = new byte[32];
                    using (var randomNumberGenerator = RandomNumberGenerator.Create())
                    {
                        randomNumberGenerator.GetBytes(sessionId);
                    }

                    myProfileMessage = new ProfileMessage(
                        sessionId,
                        (_passwords.Count == 0) ? AuthenticationType.None : AuthenticationType.Password,
                        new[] { KeyExchangeAlgorithm.EcDh_P521_Sha2_256 },
                        new[] { KeyDerivationAlgorithm.Pbkdf2 },
                        new[] { CryptoAlgorithm.Aes_Gcm_256 },
                        new[] { HashAlgorithm.Sha2_256 });
                }

                var enqueueTask = _connection.Sender.SendAsync(myProfileMessage, cancellationToken).AsTask();
                var dequeueTask = _connection.Receiver.ReceiveAsync<ProfileMessage>(cancellationToken).AsTask();

                await Task.WhenAll(enqueueTask, dequeueTask);
                otherProfileMessage = dequeueTask.Result;

                if (otherProfileMessage is null) throw new NullReferenceException();
                if (myProfileMessage.AuthenticationType != otherProfileMessage.AuthenticationType) throw new OmniSecureConnectionException("AuthenticationType does not match.");
            }

            var keyExchangeAlgorithm = EnumHelper.GetOverlappedMaxValue(myProfileMessage.KeyExchangeAlgorithms, otherProfileMessage.KeyExchangeAlgorithms);
            var keyDerivationAlgorithm = EnumHelper.GetOverlappedMaxValue(myProfileMessage.KeyDerivationAlgorithms, otherProfileMessage.KeyDerivationAlgorithms);
            var cryptoAlgorithm = EnumHelper.GetOverlappedMaxValue(myProfileMessage.CryptoAlgorithms, otherProfileMessage.CryptoAlgorithms);
            var hashAlgorithm = EnumHelper.GetOverlappedMaxValue(myProfileMessage.HashAlgorithms, otherProfileMessage.HashAlgorithms);

            if (keyExchangeAlgorithm is null) throw new OmniSecureConnectionException("key exchange algorithm does not match.");
            if (keyDerivationAlgorithm is null) throw new OmniSecureConnectionException("key derivation algorithm does not match.");
            if (cryptoAlgorithm is null) throw new OmniSecureConnectionException("Crypto algorithm does not match.");
            if (hashAlgorithm is null) throw new OmniSecureConnectionException("Hash algorithm does not match.");

            ReadOnlyMemory<byte> secret = null;

            if (keyExchangeAlgorithm.Value.HasFlag(KeyExchangeAlgorithm.EcDh_P521_Sha2_256))
            {
                var myAgreement = OmniAgreement.Create(OmniAgreementAlgorithmType.EcDh_P521_Sha2_256);

                OmniAgreementPrivateKey myAgreementPrivateKey;
                OmniAgreementPublicKey? otherAgreementPublicKey = null;
                {
                    {
                        myAgreementPrivateKey = myAgreement.GetOmniAgreementPrivateKey();

                        var enqueueTask = _connection.Sender.SendAsync(myAgreement.GetOmniAgreementPublicKey(), cancellationToken).AsTask();
                        var dequeueTask = _connection.Receiver.ReceiveAsync<OmniAgreementPublicKey>(cancellationToken).AsTask();

                        await Task.WhenAll(enqueueTask, dequeueTask);
                        otherAgreementPublicKey = dequeueTask.Result;

                        if (otherAgreementPublicKey is null) throw new NullReferenceException();
                        if ((DateTime.UtcNow - otherAgreementPublicKey.CreationTime.ToDateTime()).TotalMinutes > 30) throw new OmniSecureConnectionException("Agreement public key has Expired.");
                    }

                    if (_passwords.Count > 0)
                    {
                        AuthenticationMessage myAuthenticationMessage;
                        AuthenticationMessage? otherAuthenticationMessage = null;
                        {
                            {
                                var myHashAndPasswordList = this.GetHashes(myProfileMessage, myAgreement.GetOmniAgreementPublicKey(), hashAlgorithm.Value).ToList();

                                _random.Shuffle(myHashAndPasswordList);
                                myAuthenticationMessage = new AuthenticationMessage(myHashAndPasswordList.Select(n => n.Item1).ToArray());
                            }

                            var enqueueTask = _connection.Sender.SendAsync(myAuthenticationMessage, cancellationToken).AsTask();
                            var dequeueTask = _connection.Receiver.ReceiveAsync<AuthenticationMessage>(cancellationToken).AsTask();

                            await Task.WhenAll(enqueueTask, dequeueTask);
                            otherAuthenticationMessage = dequeueTask.Result;

                            if (otherAuthenticationMessage is null) throw new NullReferenceException();

                            var matchedPasswords = new List<string>();
                            {
                                var equalityComparer = new CustomEqualityComparer<ReadOnlyMemory<byte>>((x, y) => BytesOperations.Equals(x.Span, y.Span), (x) => Fnv1_32.ComputeHash(x.Span));
                                var receiveHashes = new HashSet<ReadOnlyMemory<byte>>(otherAuthenticationMessage.Hashes, equalityComparer);

                                foreach (var (hash, password) in this.GetHashes(otherProfileMessage, otherAgreementPublicKey, hashAlgorithm.Value))
                                {
                                    if (receiveHashes.Contains(hash))
                                    {
                                        matchedPasswords.Add(password);
                                    }
                                }
                            }

                            if (matchedPasswords.Count == 0) throw new OmniSecureConnectionException("Password does not match.");

                            _matchedPasswords = matchedPasswords.ToArray();
                        }
                    }
                }

                if (hashAlgorithm.Value.HasFlag(HashAlgorithm.Sha2_256))
                {
                    secret = OmniAgreement.GetSecret(otherAgreementPublicKey, myAgreementPrivateKey);
                }
            }

            byte[] myCryptoKey;
            byte[] otherCryptoKey;
            byte[] myNonce;
            byte[] otherNonce;

            if (keyDerivationAlgorithm.Value.HasFlag(KeyDerivationAlgorithm.Pbkdf2))
            {
                byte[] xorSessionId = new byte[Math.Max(myProfileMessage.SessionId.Length, otherProfileMessage.SessionId.Length)];
                BytesOperations.Xor(myProfileMessage.SessionId.Span, otherProfileMessage.SessionId.Span, xorSessionId);

                int cryptoKeyLength = 0;
                int nonceLength = 0;

                if (cryptoAlgorithm.Value.HasFlag(CryptoAlgorithm.Aes_Gcm_256))
                {
                    cryptoKeyLength = 32;
                    nonceLength = 12;
                }

                myCryptoKey = new byte[cryptoKeyLength];
                otherCryptoKey = new byte[cryptoKeyLength];
                myNonce = new byte[nonceLength];
                otherNonce = new byte[nonceLength];

                var kdfResult = new byte[(cryptoKeyLength + nonceLength) * 2];

                if (hashAlgorithm.Value.HasFlag(HashAlgorithm.Sha2_256))
                {
                    Pbkdf2_Sha2_256.TryComputeHash(secret.Span, xorSessionId, 1024, kdfResult);
                }

                using (var stream = new MemoryStream(kdfResult))
                {
                    if (_type == OmniSecureConnectionType.Connected)
                    {
                        stream.Read(myCryptoKey, 0, myCryptoKey.Length);
                        stream.Read(otherCryptoKey, 0, otherCryptoKey.Length);
                        stream.Read(myNonce, 0, myNonce.Length);
                        stream.Read(otherNonce, 0, otherNonce.Length);
                    }
                    else if (_type == OmniSecureConnectionType.Accepted)
                    {
                        stream.Read(otherCryptoKey, 0, otherCryptoKey.Length);
                        stream.Read(myCryptoKey, 0, myCryptoKey.Length);
                        stream.Read(otherNonce, 0, otherNonce.Length);
                        stream.Read(myNonce, 0, myNonce.Length);
                    }
                }
            }
            else
            {
                throw new NotSupportedException(nameof(keyDerivationAlgorithm));
            }

            var sessionState = new SessionState(cryptoAlgorithm.Value, hashAlgorithm.Value, myCryptoKey, otherCryptoKey, myNonce, otherNonce);
            _sender = new ConnectionSender(_connection.Sender, _bytesPool, sessionState, _cancellationTokenSource);
            _receiver = new ConnectionReceiver(_connection.Receiver, _maxReceiveByteCount, _bytesPool, sessionState, _cancellationTokenSource);
            _subscribers = new ConnectionSubscribers(_cancellationTokenSource.Token);
            _batchAction = new BatchAction(_sender, _receiver);
            _batchActionDispatcher.Register(_batchAction);
        }

        private (ReadOnlyMemory<byte>, string)[] GetHashes(ProfileMessage profileMessage, OmniAgreementPublicKey agreementPublicKey, HashAlgorithm hashAlgorithm)
        {
            var results = new Dictionary<ReadOnlyMemory<byte>, string>();

            byte[] verificationMessageHash;
            {
                var verificationMessage = new VerificationMessage(profileMessage, agreementPublicKey);

                if (hashAlgorithm == HashAlgorithm.Sha2_256)
                {
                    using var bytesPipe = new BytesPipe();

                    verificationMessage.Export(bytesPipe.Writer, _bytesPool);
                    verificationMessageHash = Sha2_256.ComputeHash(bytesPipe.Reader.GetSequence());
                }
                else
                {
                    throw new NotSupportedException(nameof(hashAlgorithm));
                }
            }

            foreach (var password in _passwords)
            {
                if (hashAlgorithm.HasFlag(HashAlgorithm.Sha2_256))
                {
                    results.Add(Hmac_Sha2_256.ComputeHash(verificationMessageHash, Sha2_256.ComputeHash(password)), password);
                }
            }

            return results.Select(item => (item.Key, item.Value)).ToArray();
        }

        internal sealed class SessionState
        {
            public SessionState(CryptoAlgorithm cryptoAlgorithm, HashAlgorithm hashAlgorithm, byte[] myCryptoKey, byte[] otherCryptoKey, byte[] myNonce, byte[] otherNonce)
            {
                this.CryptoAlgorithm = cryptoAlgorithm;
                this.HashAlgorithm = hashAlgorithm;
                this.MyCryptoKey = myCryptoKey;
                this.OtherCryptoKey = otherCryptoKey;
                this.MyNonce = myNonce;
                this.OtherNonce = otherNonce;
            }

            public CryptoAlgorithm CryptoAlgorithm { get; }

            public HashAlgorithm HashAlgorithm { get; }

            public byte[] MyCryptoKey { get; }

            public byte[] OtherCryptoKey { get; }

            public byte[] MyNonce { get; }

            public byte[] OtherNonce { get; }
        }
    }
}
