using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Base.Extensions;
using Omnix.Base.Helpers;
using Omnix.Cryptography;
using Omnix.Io;
using Omnix.Serialization;
using System.Buffers.Binary;
using Omnix.Network.Connection.Secure.Internal;

namespace Omnix.Network.Connection.Secure
{
    public sealed class OmniSecureConnection : DisposableBase, IConnection
    {
        private readonly IConnection _connection;
        private readonly OmniSecureConnectionType _type;
        private readonly IReadOnlyList<string> _passwords;
        private readonly BufferPool _bufferPool;

        private OmniSecureConnectionVersion _version = OmniSecureConnectionVersion.Version1;
        private long _totalSentSize;
        private long _totalReceivedSize;
        private string[]? _matchedPasswords;

        private InfoV1? _infoV1;

        private RandomNumberGenerator _random = RandomNumberGenerator.Create();

        private volatile bool _disposed;

        public OmniSecureConnection(IConnection connection, OmniSecureConnectionOptions options)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (!EnumHelper.IsValid(options.Type)) throw new ArgumentException(nameof(options.Type));

            _connection = connection;
            _type = options.Type;
            _passwords = new ReadOnlyCollection<string>(options.Passwords ?? Array.Empty<string>());
            _bufferPool = options.BufferPool ?? BufferPool.Shared;
        }

        public IConnection BaseConnection => _connection;

        public bool IsConnected => _connection.IsConnected;
        public long ReceivedByteCount => _connection.ReceivedByteCount;
        public long SentByteCount => _connection.SentByteCount;

        public OmniSecureConnectionType Type => _type;
        public IEnumerable<string> MatchedPasswords => _matchedPasswords ?? Enumerable.Empty<string>();

        public void DoEvents()
        {
            _connection.DoEvents();
        }

        public async ValueTask Handshake(CancellationToken token = default)
        {
            try
            {
                await this.Hello(_connection, token);

                if (_version == OmniSecureConnectionVersion.Version1)
                {
                    await this.HandshakeV1(_connection, token);
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

        private async ValueTask Hello(IConnection connection, CancellationToken token)
        {
            HelloMessage sendHelloMessage;
            HelloMessage? receiveHelloMessage = null;
            {
                sendHelloMessage = new HelloMessage(new[] { _version });

                var enqueueTask = connection.EnqueueAsync((bufferWriter) => sendHelloMessage.Export(bufferWriter, _bufferPool), token);
                var dequeueTask = connection.DequeueAsync((sequence) => receiveHelloMessage = HelloMessage.Import(sequence, _bufferPool), token);

                await ValueTaskHelper.WhenAll(enqueueTask, dequeueTask);

                if (receiveHelloMessage is null) throw new NullReferenceException();
            }

            _version = GetOverlapMaxEnum(sendHelloMessage.Versions, receiveHelloMessage.Versions);
        }

        private async ValueTask HandshakeV1(IConnection connection, CancellationToken token)
        {
            V1.Internal.ProfileMessage myProfileMessage;
            V1.Internal.ProfileMessage? otherProfileMessage = null;
            {
                {
                    var sessionId = new byte[32];
                    _random.GetBytes(sessionId);

                    myProfileMessage = new V1.Internal.ProfileMessage(
                        sessionId,
                        (_passwords.Count == 0) ? V1.Internal.AuthenticationType.None : V1.Internal.AuthenticationType.Password,
                        new[] { V1.Internal.KeyExchangeAlgorithm.EcDh_P521_Sha2_256 },
                        new[] { V1.Internal.KeyDerivationAlgorithm.Pbkdf2 },
                        new[] { V1.Internal.CryptoAlgorithm.Aes_256 },
                        new[] { V1.Internal.HashAlgorithm.Sha2_256 });
                }

                var enqueueTask = connection.EnqueueAsync((bufferWriter) => myProfileMessage.Export(bufferWriter, _bufferPool), token);
                var dequeueTask = connection.DequeueAsync((sequence) => otherProfileMessage = V1.Internal.ProfileMessage.Import(sequence, _bufferPool), token);

                await ValueTaskHelper.WhenAll(enqueueTask, dequeueTask);

                if (otherProfileMessage is null) throw new NullReferenceException();

                if (myProfileMessage.AuthenticationType != otherProfileMessage.AuthenticationType)
                {
                    throw new OmniSecureConnectionException("AuthenticationType does not match.");
                }
            }

            var keyExchangeAlgorithm = GetOverlapMaxEnum(myProfileMessage.KeyExchangeAlgorithms, otherProfileMessage.KeyExchangeAlgorithms);
            var keyDerivationAlgorithm = GetOverlapMaxEnum(myProfileMessage.KeyDerivationAlgorithms, otherProfileMessage.KeyDerivationAlgorithms);
            var cryptoAlgorithm = GetOverlapMaxEnum(myProfileMessage.CryptoAlgorithms, otherProfileMessage.CryptoAlgorithms);
            var hashAlgorithm = GetOverlapMaxEnum(myProfileMessage.HashAlgorithms, otherProfileMessage.HashAlgorithms);

            if (!EnumHelper.IsValid(keyExchangeAlgorithm))
            {
                throw new OmniSecureConnectionException("key exchange algorithm does not match.");
            }
            if (!EnumHelper.IsValid(keyDerivationAlgorithm))
            {
                throw new OmniSecureConnectionException("key derivation algorithm does not match.");
            }
            if (!EnumHelper.IsValid(cryptoAlgorithm))
            {
                throw new OmniSecureConnectionException("Crypto algorithm does not match.");
            }
            if (!EnumHelper.IsValid(hashAlgorithm))
            {
                throw new OmniSecureConnectionException("Hash algorithm does not match.");
            }

            ReadOnlyMemory<byte> secret = null;

            if (keyExchangeAlgorithm.HasFlag(V1.Internal.KeyExchangeAlgorithm.EcDh_P521_Sha2_256))
            {
                var myAgreement = OmniAgreement.Create(OmniAgreementAlgorithmType.EcDh_P521_Sha2_256);

                OmniAgreementPrivateKey myAgreementPrivateKey;
                OmniAgreementPublicKey? otherAgreementPublicKey = null;
                {
                    {
                        myAgreementPrivateKey = myAgreement.GetOmniAgreementPrivateKey();

                        var enqueueTask = connection.EnqueueAsync((bufferWriter) => myAgreement.GetOmniAgreementPublicKey().Export(bufferWriter, _bufferPool), token);
                        var dequeueTask = connection.DequeueAsync((sequence) => otherAgreementPublicKey = OmniAgreementPublicKey.Import(sequence, _bufferPool), token);

                        await ValueTaskHelper.WhenAll(enqueueTask, dequeueTask);

                        if (otherAgreementPublicKey is null) throw new NullReferenceException();

                        if ((DateTime.UtcNow - otherAgreementPublicKey.CreationTime.ToDateTime()).TotalMinutes > 30) throw new OmniSecureConnectionException("Agreement public key has Expired.");
                    }

                    if (_passwords.Count > 0)
                    {
                        V1.Internal.AuthenticationMessage myAuthenticationMessage;
                        V1.Internal.AuthenticationMessage? otherAuthenticationMessage = null;
                        {
                            {
                                var myHashAndPasswordList = this.GetHashesV1(myProfileMessage, myAgreement.GetOmniAgreementPublicKey(), hashAlgorithm).ToList();

                                RandomProvider.GetThreadRandom().Shuffle(myHashAndPasswordList);
                                myAuthenticationMessage = new V1.Internal.AuthenticationMessage(myHashAndPasswordList.Select(n => n.Item1).ToArray());
                            }

                            var enqueueTask = connection.EnqueueAsync((bufferWriter) => myAuthenticationMessage.Export(bufferWriter, _bufferPool), token);
                            var dequeueTask = connection.DequeueAsync((sequence) => otherAuthenticationMessage = V1.Internal.AuthenticationMessage.Import(sequence, _bufferPool), token);

                            await ValueTaskHelper.WhenAll(enqueueTask, dequeueTask);

                            if (otherAuthenticationMessage is null) throw new NullReferenceException();

                            var matchedPasswords = new List<string>();
                            {

                                var equalityComparer = new GenericEqualityComparer<ReadOnlyMemory<byte>>((x, y) => BytesOperations.SequenceEqual(x.Span, y.Span), (x) => Fnv1_32.ComputeHash(x.Span));
                                var receiveHashes = new HashSet<ReadOnlyMemory<byte>>(otherAuthenticationMessage.Hashes, equalityComparer);

                                foreach (var (hash, password) in this.GetHashesV1(otherProfileMessage, otherAgreementPublicKey, hashAlgorithm))
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

                if (hashAlgorithm.HasFlag(V1.Internal.HashAlgorithm.Sha2_256))
                {
                    secret = OmniAgreement.GetSecret(otherAgreementPublicKey, myAgreementPrivateKey);
                }
            }

            byte[] myCryptoKey;
            byte[] otherCryptoKey;
            byte[] myHmacKey;
            byte[] otherHmacKey;

            if (keyDerivationAlgorithm.HasFlag(V1.Internal.KeyDerivationAlgorithm.Pbkdf2))
            {
                byte[] xorSessionId = new byte[Math.Max(myProfileMessage.SessionId.Length, otherProfileMessage.SessionId.Length)];
                BytesOperations.Xor(myProfileMessage.SessionId.Span, otherProfileMessage.SessionId.Span, xorSessionId);

                int cryptoKeyLength = 0;
                int hmacKeyLength = 0;

                if (cryptoAlgorithm.HasFlag(V1.Internal.CryptoAlgorithm.Aes_256))
                {
                    cryptoKeyLength = 32;
                }

                if (hashAlgorithm.HasFlag(V1.Internal.HashAlgorithm.Sha2_256))
                {
                    hmacKeyLength = 32;
                }

                myCryptoKey = new byte[cryptoKeyLength];
                otherCryptoKey = new byte[cryptoKeyLength];
                myHmacKey = new byte[hmacKeyLength];
                otherHmacKey = new byte[hmacKeyLength];

                var kdfResult = new byte[(cryptoKeyLength + hmacKeyLength) * 2];

                if (hashAlgorithm.HasFlag(V1.Internal.HashAlgorithm.Sha2_256))
                {
                    Pbkdf2_Sha2_256.TryComputeHash(secret.Span, xorSessionId, 1024, kdfResult);
                }
                else
                {
                    throw new NotSupportedException(nameof(keyDerivationAlgorithm));
                }

                using (var stream = new MemoryStream(kdfResult))
                {
                    if (_type == OmniSecureConnectionType.Connect)
                    {
                        stream.Read(myCryptoKey, 0, myCryptoKey.Length);
                        stream.Read(otherCryptoKey, 0, otherCryptoKey.Length);
                        stream.Read(myHmacKey, 0, myHmacKey.Length);
                        stream.Read(otherHmacKey, 0, otherHmacKey.Length);
                    }
                    else if (_type == OmniSecureConnectionType.Accept)
                    {
                        stream.Read(otherCryptoKey, 0, otherCryptoKey.Length);
                        stream.Read(myCryptoKey, 0, myCryptoKey.Length);
                        stream.Read(otherHmacKey, 0, otherHmacKey.Length);
                        stream.Read(myHmacKey, 0, myHmacKey.Length);
                    }
                }
            }
            else
            {
                throw new NotSupportedException(nameof(keyDerivationAlgorithm));
            }

            _infoV1 = new InfoV1(cryptoAlgorithm, hashAlgorithm, myCryptoKey, otherCryptoKey, myHmacKey, otherHmacKey);
        }

        private IEnumerable<(ReadOnlyMemory<byte>, string)> GetHashesV1(V1.Internal.ProfileMessage profileMessage, OmniAgreementPublicKey agreementPublicKey, V1.Internal.HashAlgorithm hashAlgorithm)
        {
            var results = new Dictionary<ReadOnlyMemory<byte>, string>();

            byte[] verificationMessageHash;
            {
                var verificationMessage = new V1.Internal.VerificationMessage(profileMessage, agreementPublicKey);

                if (hashAlgorithm == V1.Internal.HashAlgorithm.Sha2_256)
                {
                    using var hub = new Hub();

                    verificationMessage.Export(hub.Writer, _bufferPool);
                    verificationMessageHash = Sha2_256.ComputeHash(hub.Reader.GetSequence());
                }
                else
                {
                    throw new NotSupportedException(nameof(hashAlgorithm));
                }
            }

            foreach (var password in _passwords)
            {
                if (hashAlgorithm.HasFlag(V1.Internal.HashAlgorithm.Sha2_256))
                {
                    results.Add(Hmac_Sha2_256.ComputeHash(verificationMessageHash, Sha2_256.ComputeHash(password)), password);
                }
            }

            return results.Select(item => (item.Key, item.Value));
        }

        private void InternalEnqueue(IBufferWriter<byte> bufferWriter, Action<IBufferWriter<byte>> action)
        {
            using var hub = new Hub();

            action.Invoke(hub.Writer);
            hub.Writer.Complete();

            var sequence = hub.Reader.GetSequence();

            try
            {
                if (_version.HasFlag(OmniSecureConnectionVersion.Version1) && _infoV1 != null)
                {
                    if (_infoV1.CryptoAlgorithm.HasFlag(V1.Internal.CryptoAlgorithm.Aes_256)
                        && _infoV1.HashAlgorithm.HasFlag(V1.Internal.HashAlgorithm.Sha2_256))
                    {
                        const int headerSize = 8;
                        const int blockSize = 16;

                        // 送信済みデータ + 送信するデータのサイズを書き込む
                        {
                            var paddingSize = blockSize;

                            if (sequence.Length % blockSize != 0)
                            {
                                paddingSize = blockSize - (int)(sequence.Length % blockSize);
                            }

                            var encryptedContentLength = blockSize + (sequence.Length + paddingSize);

                            BinaryPrimitives.TryWriteUInt64BigEndian(bufferWriter.GetSpan(headerSize), (ulong)(_totalSentSize + encryptedContentLength));
                            bufferWriter.Advance(headerSize);
                        }

                        using (var hmac = new HMACSHA256(_infoV1.MyHmacKey))
                        using (var aes = Aes.Create())
                        {
                            aes.KeySize = 256;
                            aes.Mode = CipherMode.CBC;
                            aes.Padding = PaddingMode.PKCS7;

                            // IVを書き込む
                            var iv = new byte[blockSize];
                            _random.GetBytes(iv);
                            bufferWriter.Write(iv);
                            hmac.TransformBlock(iv, 0, iv.Length, null, 0);
                            Interlocked.Add(ref _totalSentSize, iv.Length);

                            // 暗号化データを書き込む
                            using (var encryptor = aes.CreateEncryptor(_infoV1.MyCryptoKey, iv))
                            {
                                var inBuffer = _bufferPool.GetArrayPool().Rent(blockSize);
                                var outBuffer = _bufferPool.GetArrayPool().Rent(blockSize);

                                try
                                {
                                    while (sequence.Length > blockSize)
                                    {
                                        sequence.Slice(0, blockSize).CopyTo(inBuffer.AsSpan(0, blockSize));

                                        var transed = encryptor.TransformBlock(inBuffer, 0, blockSize, outBuffer, 0);
                                        bufferWriter.Write(outBuffer.AsSpan(0, transed));
                                        hmac.TransformBlock(outBuffer, 0, transed, null, 0);
                                        Interlocked.Add(ref _totalSentSize, transed);

                                        sequence = sequence.Slice(blockSize);
                                    }

                                    {
                                        int remain = (int)sequence.Length;
                                        sequence.CopyTo(inBuffer.AsSpan(0, remain));

                                        var remainBuffer = encryptor.TransformFinalBlock(inBuffer, 0, remain);
                                        bufferWriter.Write(remainBuffer);
                                        hmac.TransformBlock(remainBuffer, 0, remainBuffer.Length, null, 0);
                                        Interlocked.Add(ref _totalSentSize, remainBuffer.Length);
                                    }
                                }
                                finally
                                {
                                    _bufferPool.GetArrayPool().Return(inBuffer);
                                    _bufferPool.GetArrayPool().Return(outBuffer);
                                }
                            }

                            // HMACを書き込む
                            hmac.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
                            bufferWriter.Write(hmac.Hash);
                        }

                        hub.Reader.Complete();

                        return;
                    }
                }
            }
            catch (OmniSecureConnectionException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new OmniSecureConnectionException(e.Message, e);
            }

            throw new OmniSecureConnectionException("Conversion failed.");
        }

        public bool TryEnqueue(Action<IBufferWriter<byte>> action)
        {
            return _connection.TryEnqueue((bufferWriter) => this.InternalEnqueue(bufferWriter, action));
        }

        public async ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) => this.InternalEnqueue(bufferWriter, action), token);
        }

        public void Enqueue(Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            _connection.Enqueue((bufferWriter) => this.InternalEnqueue(bufferWriter, action), token);
        }

        private void InternalDequeue(ReadOnlySequence<byte> sequence, Action<ReadOnlySequence<byte>> action)
        {
            using var hub = new Hub();

            try
            {
                if (_version.HasFlag(OmniSecureConnectionVersion.Version1) && _infoV1 != null)
                {
                    if (_infoV1.CryptoAlgorithm.HasFlag(V1.Internal.CryptoAlgorithm.Aes_256)
                        && _infoV1.HashAlgorithm.HasFlag(V1.Internal.HashAlgorithm.Sha2_256))
                    {
                        const int headerSize = 8;
                        const int hashLength = 32;
                        const int blockSize = 16;

                        Interlocked.Add(ref _totalReceivedSize, sequence.Length - (headerSize + hashLength));

                        // 送信済みデータ + 送信するデータのサイズが正しいか検証する
                        {
                            long totalReceivedSize;
                            {
                                Span<byte> totalReceiveSizeBuffer = stackalloc byte[headerSize];
                                sequence.Slice(0, headerSize).CopyTo(totalReceiveSizeBuffer);
                                totalReceivedSize = (long)BinaryPrimitives.ReadUInt64BigEndian(totalReceiveSizeBuffer);
                            }

                            if (totalReceivedSize != _totalReceivedSize) throw new OmniSecureConnectionException();
                        }

                        // HMACが正しいか検証する
                        {
                            Span<byte> receivedHash = stackalloc byte[hashLength];
                            sequence.Slice(sequence.Length - hashLength).CopyTo(receivedHash);

                            var computedhash = Hmac_Sha2_256.ComputeHash(sequence.Slice(headerSize, sequence.Length - (headerSize + hashLength)), _infoV1.OtherHmacKey);
                            if (!BytesOperations.SequenceEqual(receivedHash, computedhash)) throw new OmniSecureConnectionException();
                        }

                        sequence = sequence.Slice(headerSize, sequence.Length - (headerSize + hashLength));

                        using (var aes = Aes.Create())
                        {
                            aes.KeySize = 256;
                            aes.Mode = CipherMode.CBC;
                            aes.Padding = PaddingMode.PKCS7;

                            // IVを読み込む
                            var iv = new byte[16];
                            sequence.Slice(0, iv.Length).CopyTo(iv);
                            sequence = sequence.Slice(iv.Length);

                            // 暗号化されたデータを復号化する
                            using (var decryptor = aes.CreateDecryptor(_infoV1.OtherCryptoKey, iv))
                            {
                                var inBuffer = _bufferPool.GetArrayPool().Rent(blockSize);
                                var outBuffer = _bufferPool.GetArrayPool().Rent(blockSize);

                                try
                                {
                                    while (sequence.Length > blockSize)
                                    {
                                        sequence.Slice(0, blockSize).CopyTo(inBuffer.AsSpan(0, blockSize));

                                        var transed = decryptor.TransformBlock(inBuffer, 0, blockSize, outBuffer, 0);
                                        hub.Writer.Write(outBuffer.AsSpan(0, transed));

                                        sequence = sequence.Slice(blockSize);
                                    }

                                    {
                                        int remain = (int)sequence.Length;
                                        sequence.CopyTo(inBuffer.AsSpan(0, remain));

                                        var remainBuffer = decryptor.TransformFinalBlock(inBuffer, 0, remain);
                                        hub.Writer.Write(remainBuffer);
                                        hub.Writer.Complete();
                                    }
                                }
                                finally
                                {
                                    _bufferPool.GetArrayPool().Return(inBuffer);
                                    _bufferPool.GetArrayPool().Return(outBuffer);
                                }
                            }
                        }

                        action.Invoke(hub.Reader.GetSequence());

                        hub.Reader.Complete();

                        return;
                    }
                }
            }
            catch (OmniSecureConnectionException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                throw new OmniSecureConnectionException(e.Message, e);
            }

            throw new OmniSecureConnectionException("Conversion failed.");
        }

        public bool TryDequeue(Action<ReadOnlySequence<byte>> action)
        {
            return _connection.TryDequeue((sequence) => this.InternalDequeue(sequence, action));
        }

        public async ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
            await _connection.DequeueAsync((sequence) => this.InternalDequeue(sequence, action), token);
        }
        public void Dequeue(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
            _connection.Dequeue((sequence) => this.InternalDequeue(sequence, action), token);
        }

        private sealed class InfoV1
        {
            public InfoV1(V1.Internal.CryptoAlgorithm cryptoAlgorithm, V1.Internal.HashAlgorithm hashAlgorithm,
                byte[] myCryptoKey, byte[] otherCryptoKey, byte[] myHmacKey, byte[] otherHmacKey)
            {
                this.CryptoAlgorithm = cryptoAlgorithm;
                this.HashAlgorithm = hashAlgorithm;
                this.MyCryptoKey = myCryptoKey;
                this.OtherCryptoKey = otherCryptoKey;
                this.MyHmacKey = myHmacKey;
                this.OtherHmacKey = otherHmacKey;
            }

            public V1.Internal.CryptoAlgorithm CryptoAlgorithm { get; set; }
            public V1.Internal.HashAlgorithm HashAlgorithm { get; set; }

            public byte[] MyCryptoKey { get; set; }
            public byte[] OtherCryptoKey { get; set; }

            public byte[] MyHmacKey { get; set; }
            public byte[] OtherHmacKey { get; set; }
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                _random.Dispose();
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
