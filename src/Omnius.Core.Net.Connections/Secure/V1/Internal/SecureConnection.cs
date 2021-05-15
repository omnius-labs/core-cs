using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Cryptography;
using Omnius.Core.Cryptography.Functions;
using Omnius.Core.Extensions;
using Omnius.Core.Helpers;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal
{
    public sealed class SecureConnection : DisposableBase
    {
        private readonly IConnection _connection;
        private readonly OmniSecureConnectionType _type;
        private readonly IReadOnlyList<string> _passwords;
        private readonly IBytesPool _bytesPool;

        private string[]? _matchedPasswords;

        private State? _state;

        private readonly Random _random = new Random();

        private const int FrameSize = 16 * 1024;

        public SecureConnection(IConnection connection, OmniSecureConnectionOptions options)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));

            if (options == null) throw new ArgumentNullException(nameof(options));
            if (!EnumHelper.IsValid(options.Type)) throw new ArgumentException(nameof(options.Type));

            _type = options.Type;
            _passwords = options.Passwords ?? Array.Empty<string>();
            _bytesPool = options.BufferPool ?? BytesPool.Shared;
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
            }
        }

        public IEnumerable<string> MatchedPasswords => _matchedPasswords ?? Enumerable.Empty<string>();

        private static T GetOverlapMaxEnum<T>(IEnumerable<T> s1, IEnumerable<T> s2)
            where T : Enum
        {
            var list = s1.ToList();
            list.Sort((x, y) => y.CompareTo(x));

            var hashSet = new HashSet<T>(s2);

            foreach (var item in list)
            {
                if (hashSet.Contains(item)) return item;
            }

            throw new OmniSecureConnectionException($"Overlap enum of {nameof(T)} could not be found.");
        }

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

                var enqueueTask = _connection.EnqueueAsync((bufferWriter) => myProfileMessage.Export(bufferWriter, _bytesPool), cancellationToken);
                var dequeueTask = _connection.DequeueAsync((sequence) => otherProfileMessage = ProfileMessage.Import(sequence, _bytesPool), cancellationToken);

                await ValueTaskHelper.WhenAll(enqueueTask, dequeueTask);

                if (otherProfileMessage is null) throw new NullReferenceException();
                if (myProfileMessage.AuthenticationType != otherProfileMessage.AuthenticationType) throw new OmniSecureConnectionException("AuthenticationType does not match.");
            }

            var keyExchangeAlgorithm = GetOverlapMaxEnum(myProfileMessage.KeyExchangeAlgorithms, otherProfileMessage.KeyExchangeAlgorithms);
            var keyDerivationAlgorithm = GetOverlapMaxEnum(myProfileMessage.KeyDerivationAlgorithms, otherProfileMessage.KeyDerivationAlgorithms);
            var cryptoAlgorithm = GetOverlapMaxEnum(myProfileMessage.CryptoAlgorithms, otherProfileMessage.CryptoAlgorithms);
            var hashAlgorithm = GetOverlapMaxEnum(myProfileMessage.HashAlgorithms, otherProfileMessage.HashAlgorithms);

            if (!EnumHelper.IsValid(keyExchangeAlgorithm)) throw new OmniSecureConnectionException("key exchange algorithm does not match.");
            if (!EnumHelper.IsValid(keyDerivationAlgorithm)) throw new OmniSecureConnectionException("key derivation algorithm does not match.");
            if (!EnumHelper.IsValid(cryptoAlgorithm)) throw new OmniSecureConnectionException("Crypto algorithm does not match.");
            if (!EnumHelper.IsValid(hashAlgorithm)) throw new OmniSecureConnectionException("Hash algorithm does not match.");

            ReadOnlyMemory<byte> secret = null;

            if (keyExchangeAlgorithm.HasFlag(KeyExchangeAlgorithm.EcDh_P521_Sha2_256))
            {
                var myAgreement = OmniAgreement.Create(OmniAgreementAlgorithmType.EcDh_P521_Sha2_256);

                OmniAgreementPrivateKey myAgreementPrivateKey;
                OmniAgreementPublicKey? otherAgreementPublicKey = null;
                {
                    {
                        myAgreementPrivateKey = myAgreement.GetOmniAgreementPrivateKey();

                        var enqueueTask = _connection.EnqueueAsync((bufferWriter) => myAgreement.GetOmniAgreementPublicKey().Export(bufferWriter, _bytesPool), cancellationToken);
                        var dequeueTask = _connection.DequeueAsync((sequence) => otherAgreementPublicKey = OmniAgreementPublicKey.Import(sequence, _bytesPool), cancellationToken);

                        await ValueTaskHelper.WhenAll(enqueueTask, dequeueTask);

                        if (otherAgreementPublicKey is null) throw new NullReferenceException();
                        if ((DateTime.UtcNow - otherAgreementPublicKey.CreationTime.ToDateTime()).TotalMinutes > 30) throw new OmniSecureConnectionException("Agreement public key has Expired.");
                    }

                    if (_passwords.Count > 0)
                    {
                        AuthenticationMessage myAuthenticationMessage;
                        AuthenticationMessage? otherAuthenticationMessage = null;
                        {
                            {
                                var myHashAndPasswordList = this.GetHashes(myProfileMessage, myAgreement.GetOmniAgreementPublicKey(), hashAlgorithm).ToList();

                                _random.Shuffle(myHashAndPasswordList);
                                myAuthenticationMessage = new AuthenticationMessage(myHashAndPasswordList.Select(n => n.Item1).ToArray());
                            }

                            var enqueueTask = _connection.EnqueueAsync((bufferWriter) => myAuthenticationMessage.Export(bufferWriter, _bytesPool), cancellationToken);
                            var dequeueTask = _connection.DequeueAsync((sequence) => otherAuthenticationMessage = AuthenticationMessage.Import(sequence, _bytesPool), cancellationToken);

                            await ValueTaskHelper.WhenAll(enqueueTask, dequeueTask);

                            if (otherAuthenticationMessage is null) throw new NullReferenceException();

                            var matchedPasswords = new List<string>();
                            {
                                var equalityComparer = new CustomEqualityComparer<ReadOnlyMemory<byte>>((x, y) => BytesOperations.Equals(x.Span, y.Span), (x) => Fnv1_32.ComputeHash(x.Span));
                                var receiveHashes = new HashSet<ReadOnlyMemory<byte>>(otherAuthenticationMessage.Hashes, equalityComparer);

                                foreach (var (hash, password) in this.GetHashes(otherProfileMessage, otherAgreementPublicKey, hashAlgorithm))
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

                if (hashAlgorithm.HasFlag(HashAlgorithm.Sha2_256))
                {
                    secret = OmniAgreement.GetSecret(otherAgreementPublicKey, myAgreementPrivateKey);
                }
            }

            byte[] myCryptoKey;
            byte[] otherCryptoKey;
            byte[] myNonce;
            byte[] otherNonce;

            if (keyDerivationAlgorithm.HasFlag(KeyDerivationAlgorithm.Pbkdf2))
            {
                byte[] xorSessionId = new byte[Math.Max(myProfileMessage.SessionId.Length, otherProfileMessage.SessionId.Length)];
                BytesOperations.Xor(myProfileMessage.SessionId.Span, otherProfileMessage.SessionId.Span, xorSessionId);

                int cryptoKeyLength = 0;
                int nonceLength = 0;

                if (cryptoAlgorithm.HasFlag(CryptoAlgorithm.Aes_Gcm_256))
                {
                    cryptoKeyLength = 32;
                    nonceLength = 12;
                }

                myCryptoKey = new byte[cryptoKeyLength];
                otherCryptoKey = new byte[cryptoKeyLength];
                myNonce = new byte[nonceLength];
                otherNonce = new byte[nonceLength];

                var kdfResult = new byte[(cryptoKeyLength + nonceLength) * 2];

                if (hashAlgorithm.HasFlag(HashAlgorithm.Sha2_256))
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

            _state = new State(cryptoAlgorithm, hashAlgorithm, myCryptoKey, otherCryptoKey, myNonce, otherNonce);
        }

        private (ReadOnlyMemory<byte>, string)[] GetHashes(ProfileMessage profileMessage, OmniAgreementPublicKey agreementPublicKey, HashAlgorithm hashAlgorithm)
        {
            var results = new Dictionary<ReadOnlyMemory<byte>, string>();

            byte[] verificationMessageHash;
            {
                var verificationMessage = new VerificationMessage(profileMessage, agreementPublicKey);

                if (hashAlgorithm == HashAlgorithm.Sha2_256)
                {
                    using var hub = new BytesHub();

                    verificationMessage.Export(hub.Writer, _bytesPool);
                    verificationMessageHash = Sha2_256.ComputeHash(hub.Reader.GetSequence());
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

        private void Encode(IBufferWriter<byte> bufferWriter, Action<IBufferWriter<byte>> action)
        {
            if (_state == null) throw new OmniSecureConnectionException("Not handshaked");

            using var hub = new BytesHub();
            action.Invoke(hub.Writer);

            var sequence = hub.Reader.GetSequence();

            try
            {
                if (_state.CryptoAlgorithm.HasFlag(CryptoAlgorithm.Aes_Gcm_256))
                {
                    using (var aes = new AesGcm(_state.MyCryptoKey))
                    {
                        Span<byte> tag = stackalloc byte[16];
                        var inBuffer = _bytesPool.Array.Rent(FrameSize - tag.Length);
                        var outBuffer = _bytesPool.Array.Rent(FrameSize - tag.Length);

                        try
                        {
                            while (sequence.Length > 0)
                            {
                                int contentLength = (int)Math.Min(sequence.Length, FrameSize - tag.Length);

                                var plaintext = inBuffer.AsSpan(0, contentLength);
                                var ciphertext = outBuffer.AsSpan(0, contentLength);

                                sequence.Slice(0, contentLength).CopyTo(plaintext);
                                sequence = sequence.Slice(contentLength);

                                aes.Encrypt(_state.MyNonce, plaintext, ciphertext, tag);
                                Increment(_state.MyNonce);

                                bufferWriter.Write(ciphertext);
                                bufferWriter.Write(tag);
                            }
                        }
                        finally
                        {
                            _bytesPool.Array.Return(inBuffer);
                            _bytesPool.Array.Return(outBuffer);
                        }
                    }

                    return;
                }
            }
            catch (OmniSecureConnectionException)
            {
                throw;
            }

            throw new OmniSecureConnectionException("Conversion failed.");
        }

        public bool TryEnqueue(Action<IBufferWriter<byte>> action)
        {
            return _connection.TryEnqueue((bufferWriter) => this.Encode(bufferWriter, action));
        }

        public async ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken cancellationToken = default)
        {
            await _connection.EnqueueAsync((bufferWriter) => this.Encode(bufferWriter, action), cancellationToken);
        }

        private void Decode(ReadOnlySequence<byte> sequence, Action<ReadOnlySequence<byte>> action)
        {
            if (_state == null) throw new OmniSecureConnectionException("Not handshaked");

            using var hub = new BytesHub();

            try
            {
                if (_state.CryptoAlgorithm.HasFlag(CryptoAlgorithm.Aes_Gcm_256))
                {
                    using (var aes = new AesGcm(_state.OtherCryptoKey))
                    {
                        Span<byte> tag = stackalloc byte[16];
                        var inBuffer = _bytesPool.Array.Rent(FrameSize - tag.Length);
                        var outBuffer = _bytesPool.Array.Rent(FrameSize - tag.Length);

                        try
                        {
                            while (sequence.Length > 0)
                            {
                                if (sequence.Length <= tag.Length) throw new FormatException();

                                int contentLength = (int)Math.Min(sequence.Length, FrameSize) - tag.Length;

                                var ciphertext = inBuffer.AsSpan(0, contentLength);
                                var plaintext = outBuffer.AsSpan(0, contentLength);

                                sequence.Slice(0, contentLength).CopyTo(ciphertext);
                                sequence = sequence.Slice(contentLength);

                                sequence.Slice(0, tag.Length).CopyTo(tag);
                                sequence = sequence.Slice(tag.Length);

                                aes.Decrypt(_state.OtherNonce, ciphertext, tag, plaintext);
                                Increment(_state.OtherNonce);

                                hub.Writer.Write(plaintext);
                            }
                        }
                        finally
                        {
                            _bytesPool.Array.Return(inBuffer);
                            _bytesPool.Array.Return(outBuffer);
                        }
                    }

                    action.Invoke(hub.Reader.GetSequence());
                    return;
                }
            }
            catch (OmniSecureConnectionException)
            {
                throw;
            }

            throw new OmniSecureConnectionException("Conversion failed.");
        }

        public bool TryDequeue(Action<ReadOnlySequence<byte>> action)
        {
            return _connection.TryDequeue((sequence) => this.Decode(sequence, action));
        }

        public async ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken cancellationToken = default)
        {
            await _connection.DequeueAsync((sequence) => this.Decode(sequence, action), cancellationToken);
        }

        private sealed class State
        {
            public State(CryptoAlgorithm cryptoAlgorithm, HashAlgorithm hashAlgorithm, byte[] myCryptoKey, byte[] otherCryptoKey, byte[] myNonce, byte[] otherNonce)
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
