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

namespace Omnix.Network.Connection.Secure
{
    public sealed class OmniSecureConnection : DisposableBase, IConnection
    {
        private readonly IConnection _connection;
        private readonly SecureConnectionType _type;
        private readonly IReadOnlyList<string> _passwords;
        private readonly BufferPool _bufferPool;

        private SecureConnectionVersion _version = SecureConnectionVersion.Version1;
        private long _totalSentSize;
        private long _totalReceivedSize;
        private string[] _matchedPasswords;

        private InfoV1 _infoV1;

        private RandomNumberGenerator _random = RandomNumberGenerator.Create();

        private volatile bool _disposed;

        public OmniSecureConnection(IConnection connection, SecureConnectionType type, BufferPool bufferPool)
            : this(connection, type, Enumerable.Empty<string>(), bufferPool)
        {

        }

        public OmniSecureConnection(IConnection connection, SecureConnectionType type, IEnumerable<string> passwords, BufferPool bufferPool)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (!EnumHelper.IsValid(type)) throw new ArgumentException(nameof(type));
            if (passwords == null) throw new ArgumentNullException(nameof(passwords));
            if (bufferPool == null) throw new ArgumentNullException(nameof(bufferPool));

            _connection = connection;
            _type = type;
            _passwords = new ReadOnlyCollection<string>(passwords.ToList());
            _bufferPool = bufferPool;
        }

        public IConnection BaseConnection => _connection;

        public bool IsConnected => _connection.IsConnected;
        public long ReceivedByteCount => _connection.ReceivedByteCount;
        public long SentByteCount => _connection.SentByteCount;

        public SecureConnectionType Type => _type;
        public IReadOnlyList<string> MatchedPasswords => _matchedPasswords;

        public async ValueTask Handshake(CancellationToken token = default)
        {
            try
            {
                await this.Hello(_connection, token);

                if (_version == SecureConnectionVersion.Version1)
                {
                    await this.HandshakeV1(_connection, token);
                }
                else
                {
                    throw new NotSupportedException("Not supported SecureConnectionVersion.");
                }
            }
            catch (SecureConnectionException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new SecureConnectionException($"Handshake of {nameof(OmniSecureConnection)} failed.", e);
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

            throw new SecureConnectionException($"Overlap enum of {nameof(T)} could not be found.");
        }

        private async ValueTask Hello(IConnection connection, CancellationToken token)
        {
            if (_type == SecureConnectionType.Connect)
            {
                // Helloメッセージを送信
                var sendHelloMessage = new SecureConnectionHelloMessage(new[] { _version });
                await connection.EnqueueAsync((bufferWriter) => sendHelloMessage.Export(bufferWriter, _bufferPool), token);

                // Helloメッセージを受信
                SecureConnectionHelloMessage receiveHelloMessage = null;
                await connection.DequeueAsync((sequence) => receiveHelloMessage = SecureConnectionHelloMessage.Import(sequence, _bufferPool), token);

                _version = GetOverlapMaxEnum(sendHelloMessage.Versions, receiveHelloMessage.Versions);
            }
            else if (_type == SecureConnectionType.Accept)
            {
                // Helloメッセージを受信
                SecureConnectionHelloMessage receiveHelloMessage = null;
                await connection.DequeueAsync((sequence) => receiveHelloMessage = SecureConnectionHelloMessage.Import(sequence, _bufferPool), token);

                // Helloメッセージを送信
                var sendHelloMessage = new SecureConnectionHelloMessage(new[] { _version });
                await connection.EnqueueAsync((bufferWriter) => sendHelloMessage.Export(bufferWriter, _bufferPool), token);

                _version = GetOverlapMaxEnum(sendHelloMessage.Versions, receiveHelloMessage.Versions);
            }
        }

        private async ValueTask HandshakeV1(IConnection connection, CancellationToken token)
        {
            V1.SecureConnectionProfileMessage myProfileMessage = null;
            V1.SecureConnectionProfileMessage otherProfileMessage = null;
            {
                {
                    var sessionId = new byte[32];
                    _random.GetBytes(sessionId);

                    myProfileMessage = new V1.SecureConnectionProfileMessage(
                        sessionId,
                        (_passwords.Count == 0) ? V1.AuthenticationType.None : V1.AuthenticationType.Password,
                        new[] { V1.KeyExchangeAlgorithm.EcDh_P521_Sha2_256 },
                        new[] { V1.KeyDerivationAlgorithm.Pbkdf2 },
                        new[] { V1.CryptoAlgorithm.Aes_256 },
                        new[] { V1.HashAlgorithm.Sha2_256 });
                }

                // 送信
                await connection.EnqueueAsync((bufferWriter) => myProfileMessage.Export(bufferWriter, _bufferPool), token);

                // 受信
                await connection.DequeueAsync((sequence) => otherProfileMessage = V1.SecureConnectionProfileMessage.Import(sequence, _bufferPool), token);

                if (myProfileMessage.AuthenticationType != otherProfileMessage.AuthenticationType)
                {
                    throw new SecureConnectionException("AuthenticationType does not match.");
                }
            }

            var keyExchangeAlgorithm = GetOverlapMaxEnum(myProfileMessage.KeyExchangeAlgorithms, otherProfileMessage.KeyExchangeAlgorithms);
            var keyDerivationAlgorithm = GetOverlapMaxEnum(myProfileMessage.KeyDerivationAlgorithms, otherProfileMessage.KeyDerivationAlgorithms);
            var cryptoAlgorithm = GetOverlapMaxEnum(myProfileMessage.CryptoAlgorithms, otherProfileMessage.CryptoAlgorithms);
            var hashAlgorithm = GetOverlapMaxEnum(myProfileMessage.HashAlgorithms, otherProfileMessage.HashAlgorithms);

            if (!EnumHelper.IsValid(keyExchangeAlgorithm))
            {
                throw new SecureConnectionException("key exchange algorithm does not match.");
            }
            if (!EnumHelper.IsValid(keyDerivationAlgorithm))
            {
                throw new SecureConnectionException("key derivation algorithm does not match.");
            }
            if (!EnumHelper.IsValid(cryptoAlgorithm))
            {
                throw new SecureConnectionException("Crypto algorithm does not match.");
            }
            if (!EnumHelper.IsValid(hashAlgorithm))
            {
                throw new SecureConnectionException("Hash algorithm does not match.");
            }

            ReadOnlyMemory<byte> secret = null;

            if (keyExchangeAlgorithm.HasFlag(V1.KeyExchangeAlgorithm.EcDh_P521_Sha2_256))
            {
                OmniAgreementPrivateKey myAgreementPrivateKey = null;
                OmniAgreementPublicKey otherAgreementPublicKey = null;
                {
                    var myAgreement = OmniAgreement.Create(OmniAgreementAlgorithmType.EcDh_P521_Sha2_256);

                    {
                        // 送信
                        await connection.EnqueueAsync((bufferWriter) => myAgreement.GetOmniAgreementPublicKey().Export(bufferWriter, _bufferPool), token);

                        myAgreementPrivateKey = myAgreement.GetOmniAgreementPrivateKey();
                    }

                    {
                        // 受信
                        await connection.DequeueAsync((sequence) => otherAgreementPublicKey = OmniAgreementPublicKey.Import(sequence, _bufferPool), token);

                        if ((DateTime.UtcNow - otherAgreementPublicKey.CreationTime.ToDateTime()).TotalMinutes > 30) throw new SecureConnectionException("Agreement public key has Expired.");
                    }

                    if (_passwords.Count > 0)
                    {
                        {
                            var myHashAndPasswordList = this.GetHashesV1(myProfileMessage, myAgreement.GetOmniAgreementPublicKey(), hashAlgorithm).ToList();

                            RandomProvider.GetThreadRandom().Shuffle(myHashAndPasswordList);
                            var myAuthenticationMessage = new V1.SecureConnectionAuthenticationMessage(myHashAndPasswordList.Select(n => n.Item1).ToList());

                            // 送信
                            await connection.EnqueueAsync((bufferWriter) => myAuthenticationMessage.Export(bufferWriter, _bufferPool), token);
                        }

                        var matchedPasswords = new List<string>();
                        {
                            V1.SecureConnectionAuthenticationMessage otherAuthenticationMessage = null;

                            // 受信
                            await connection.DequeueAsync((sequence) => otherAuthenticationMessage = V1.SecureConnectionAuthenticationMessage.Import(sequence, _bufferPool), token);

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

                        if (matchedPasswords.Count == 0) throw new SecureConnectionException("Password does not match.");

                        _matchedPasswords = matchedPasswords.ToArray();
                    }
                }

                if (hashAlgorithm.HasFlag(V1.HashAlgorithm.Sha2_256))
                {
                    secret = OmniAgreement.GetSecret(otherAgreementPublicKey, myAgreementPrivateKey);
                }
            }

            byte[] myCryptoKey = null;
            byte[] otherCryptoKey = null;
            byte[] myHmacKey = null;
            byte[] otherHmacKey = null;

            if (keyDerivationAlgorithm.HasFlag(V1.KeyDerivationAlgorithm.Pbkdf2))
            {
                byte[] xorSessionId = new byte[Math.Max(myProfileMessage.SessionId.Length, otherProfileMessage.SessionId.Length)];
                BytesOperations.Xor(myProfileMessage.SessionId.Span, otherProfileMessage.SessionId.Span, xorSessionId);

                int cryptoKeyLength = 0;
                int hmacKeyLength = 0;

                if (cryptoAlgorithm.HasFlag(V1.CryptoAlgorithm.Aes_256))
                {
                    cryptoKeyLength = 32;
                }

                if (hashAlgorithm.HasFlag(V1.HashAlgorithm.Sha2_256))
                {
                    hmacKeyLength = 32;
                }

                myCryptoKey = new byte[cryptoKeyLength];
                otherCryptoKey = new byte[cryptoKeyLength];
                myHmacKey = new byte[hmacKeyLength];
                otherHmacKey = new byte[hmacKeyLength];

                var kdfResult = new byte[(cryptoKeyLength + hmacKeyLength) * 2];

                if (hashAlgorithm.HasFlag(V1.HashAlgorithm.Sha2_256))
                {
                    Pbkdf2_Sha2_256.TryComputeHash(secret.Span, xorSessionId, 1024, kdfResult);
                }

                using (var stream = new MemoryStream(kdfResult))
                {
                    if (_type == SecureConnectionType.Connect)
                    {
                        stream.Read(myCryptoKey, 0, myCryptoKey.Length);
                        stream.Read(otherCryptoKey, 0, otherCryptoKey.Length);
                        stream.Read(myHmacKey, 0, myHmacKey.Length);
                        stream.Read(otherHmacKey, 0, otherHmacKey.Length);
                    }
                    else if (_type == SecureConnectionType.Accept)
                    {
                        stream.Read(otherCryptoKey, 0, otherCryptoKey.Length);
                        stream.Read(myCryptoKey, 0, myCryptoKey.Length);
                        stream.Read(otherHmacKey, 0, otherHmacKey.Length);
                        stream.Read(myHmacKey, 0, myHmacKey.Length);
                    }
                }
            }

            _infoV1 = new InfoV1();
            _infoV1.CryptoAlgorithm = cryptoAlgorithm;
            _infoV1.HashAlgorithm = hashAlgorithm;
            _infoV1.MyCryptoKey = myCryptoKey;
            _infoV1.OtherCryptoKey = otherCryptoKey;
            _infoV1.MyHmacKey = myHmacKey;
            _infoV1.OtherHmacKey = otherHmacKey;
        }

        private IEnumerable<(ReadOnlyMemory<byte>, string)> GetHashesV1(V1.SecureConnectionProfileMessage profileMessage, OmniAgreementPublicKey agreementPublicKey, V1.HashAlgorithm hashAlgorithm)
        {
            var results = new Dictionary<ReadOnlyMemory<byte>, string>();

            byte[] verificationMessageHash = null;
            {
                var verificationMessage = new V1.SecureConnectionVerificationMessage(profileMessage, agreementPublicKey);

                if (hashAlgorithm == V1.HashAlgorithm.Sha2_256)
                {
                    var hub = new Hub();

                    verificationMessage.Export(hub.Writer, _bufferPool);
                    verificationMessageHash = Sha2_256.ComputeHash(hub.Reader.GetSequence());
                }
            }

            foreach (var password in _passwords)
            {
                if (hashAlgorithm.HasFlag(V1.HashAlgorithm.Sha2_256))
                {
                    results.Add(Hmac_Sha2_256.ComputeHash(verificationMessageHash, Sha2_256.ComputeHash(password)), password);
                }
            }

            return results.Select(item => (item.Key, item.Value));
        }

        private void InternalEnqueue(IBufferWriter<byte> bufferWriter, Action<IBufferWriter<byte>> action)
        {
            using (var hub = new Hub())
            {
                action.Invoke(hub.Writer);
                hub.Writer.Complete();

                var sequence = hub.Reader.GetSequence();

                try
                {
                    if (_version.HasFlag(SecureConnectionVersion.Version1))
                    {
                        if (_infoV1.CryptoAlgorithm.HasFlag(V1.CryptoAlgorithm.Aes_256)
                            && _infoV1.HashAlgorithm.HasFlag(V1.HashAlgorithm.Sha2_256))
                        {
                            const int headerSize = 8;
                            const int hashLength = 32;
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
                catch (SecureConnectionException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new SecureConnectionException(e.Message, e);
                }
            }

            throw new SecureConnectionException("Conversion failed.");
        }

        public void Enqueue(Action<IBufferWriter<byte>> action)
        {
            _connection.Enqueue((bufferWriter) => this.InternalEnqueue(bufferWriter, action));
        }

        public async ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) => this.InternalEnqueue(bufferWriter, action), token);
        }

        public bool TryEnqueue(Action<IBufferWriter<byte>> action)
        {
            return _connection.TryEnqueue((bufferWriter) => this.InternalEnqueue(bufferWriter, action));
        }

        private void InternalDequeue(ReadOnlySequence<byte> sequence, Action<ReadOnlySequence<byte>> action)
        {
            using (var hub = new Hub())
            {
                try
                {
                    if (_version.HasFlag(SecureConnectionVersion.Version1))
                    {
                        if (_infoV1.CryptoAlgorithm.HasFlag(V1.CryptoAlgorithm.Aes_256)
                            && _infoV1.HashAlgorithm.HasFlag(V1.HashAlgorithm.Sha2_256))
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

                                if (totalReceivedSize != _totalReceivedSize) throw new SecureConnectionException();
                            }

                            // HMACが正しいか検証する
                            {
                                Span<byte> receivedHash = stackalloc byte[hashLength];
                                sequence.Slice(sequence.Length - hashLength).CopyTo(receivedHash);

                                var computedhash = Hmac_Sha2_256.ComputeHash(sequence.Slice(headerSize, sequence.Length - (headerSize + hashLength)), _infoV1.OtherHmacKey);
                                if (!BytesOperations.SequenceEqual(receivedHash, computedhash)) throw new SecureConnectionException();
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
                catch (SecureConnectionException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw new SecureConnectionException(e.Message, e);
                }
            }

            throw new SecureConnectionException("Conversion failed.");
        }

        public void Dequeue(Action<ReadOnlySequence<byte>> action)
        {
            _connection.Dequeue((sequence) => this.InternalDequeue(sequence, action));
        }

        public async ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
            await _connection.DequeueAsync((sequence) => this.InternalDequeue(sequence, action), token);
        }

        public bool TryDequeue(Action<ReadOnlySequence<byte>> action)
        {
            return _connection.TryDequeue((sequence) => this.InternalDequeue(sequence, action));
        }

        private class InfoV1
        {
            public V1.CryptoAlgorithm CryptoAlgorithm { get; set; }
            public V1.HashAlgorithm HashAlgorithm { get; set; }

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
                _random?.Dispose();
                _random = null;
            }
        }

        public class SecureConnectionException : Exception
        {
            public SecureConnectionException() : base() { }
            public SecureConnectionException(string message) : base(message) { }
            public SecureConnectionException(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}
