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

namespace Omnix.Network.Connection.Secure
{
    public sealed class SecureConnection : DisposableBase, INonblockingConnection
    {
        private readonly INonblockingConnection _connection;
        private readonly SecureConnectionType _type;
        private readonly IReadOnlyList<string> _passwords;
        private readonly BufferPool _bufferPool;

        private RandomNumberGenerator _random = RandomNumberGenerator.Create();

        private SecureConnectionVersion _version = SecureConnectionVersion.Version1;

        private InfoV1 _infoV1;

        private string[] _matchedPasswords;

        private volatile bool _disposed;

        public SecureConnection(INonblockingConnection connection, SecureConnectionType type, BufferPool bufferPool)
            : this(connection, type, Enumerable.Empty<string>(), bufferPool)
        {

        }

        public SecureConnection(INonblockingConnection connection, SecureConnectionType type, IEnumerable<string> passwords, BufferPool bufferPool)
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

        public bool IsConnected => _connection.IsConnected;
        public long ReceivedByteCount => _connection.ReceivedByteCount;
        public long SentByteCount => _connection.SentByteCount;

        public SecureConnectionType Type => _type;
        public IReadOnlyList<string> MatchedPasswords => _matchedPasswords;

        public int Send(int limit) => _connection.Send(limit);
        public int Receive(int limit) => _connection.Receive(limit);

        public async ValueTask Handshake(CancellationToken token)
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
                throw new SecureConnectionException($"Handshake of {nameof(SecureConnection)} failed.", e);
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

            throw new SecureConnectionException($"Overlap enums of {nameof(T)} could not be found.");
        }

        private async ValueTask Hello(INonblockingConnection connection, CancellationToken token)
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

        private async ValueTask HandshakeV1(INonblockingConnection connection, CancellationToken token)
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

            if (keyExchangeAlgorithm == 0)
            {
                throw new SecureConnectionException("key exchange algorithm does not match.");
            }
            if (keyDerivationAlgorithm == 0)
            {
                throw new SecureConnectionException("key derivation algorithm does not match.");
            }
            if (cryptoAlgorithm == 0)
            {
                throw new SecureConnectionException("Crypto algorithm does not match.");
            }
            if (hashAlgorithm == 0)
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

                        if ((DateTime.UtcNow - otherAgreementPublicKey.CreationTime.ToDateTime()).TotalMinutes > 30) throw new SecureConnectionException("Agreemet public key has Expired.");
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

                            var equalityComparer = new GenericEqualityComparer<ReadOnlyMemory<byte>>((x, y) => BytesOperations.SequenceEqual(x.Span, y.Span), (x) => Fnv1.ComputeHash32(x.Span));
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

                HMAC hmac = null;

                if (hashAlgorithm.HasFlag(V1.HashAlgorithm.Sha2_256))
                {
                    hmac = new HMACSHA256();
                }

                var pbkdf2 = new Pbkdf2(hmac, secret, xorSessionId, 1024);

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

                using (var stream = new MemoryStream(pbkdf2.GetBytes((cryptoKeyLength + hmacKeyLength) * 2)))
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

            ReadOnlyMemory<byte> verificationMessageHash = null;
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
                    results.Add(Hmac_Sha2_256.ComputeHash(verificationMessageHash.Span, Sha2_256.ComputeHash(password).Span), password);
                }
            }

            return results.Select(item => (item.Key, item.Value));
        }

        public ValueTask EnqueueAsync(Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public bool TryEnqueue(Action<IBufferWriter<byte>> action)
        {
            throw new NotImplementedException();
        }

        public ValueTask DequeueAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public bool TryDequeue(Action<ReadOnlySequence<byte>> action)
        {
            throw new NotImplementedException();
        }
        /*
        private void Encrypt(IBufferWriter<byte> bufferWriter, ReadOnlySequence<byte> sequence)
        {
            try
            {
                if (_version.HasFlag(SecureConnectionVersion.Version1))
                {
                    var resultStream = new RecyclableMemoryStream(_bufferPool);
                    resultStream.SetLength(8);
                    resultStream.Seek(8, SeekOrigin.Begin);

                    if (_infoV1.CryptoAlgorithm.HasFlag(V1.CryptoAlgorithm.Aes256))
                    {
                        byte[] iv = new byte[16];
                        _random.GetBytes(iv);
                        resultStream.Write(iv, 0, iv.Length);

                        using (var aes = Aes.Create())
                        {
                            aes.KeySize = 256;
                            aes.Mode = CipherMode.CBC;
                            aes.Padding = PaddingMode.PKCS7;

                            aes.CreateEncryptor().;

                            using (var cs = new CryptoStream(new WrapperStream(resultStream), aes.CreateEncryptor(_infoV1.MyCryptoKey, iv), CryptoStreamMode.Write))
                            using (var safeBuffer = _bufferPool.CreateSafeBuffer(1024 * 4))
                            {
                                int length;

                                while ((length = targetStream.Read(safeBuffer.Value, 0, safeBuffer.Value.Length)) > 0)
                                {
                                    cs.Write(safeBuffer.Value, 0, length);
                                }
                            }
                        }
                    }

                    _totalSendSize.Add(resultStream.Length - 8);

                    resultStream.Seek(0, SeekOrigin.Begin);

                    byte[] totalSendSizeBuffer = NetworkConverter.GetBytes(_totalSendSize);
                    resultStream.Write(totalSendSizeBuffer, 0, totalSendSizeBuffer.Length);

                    if (_infoV1.HashAlgorithm.HasFlag(V1.HashAlgorithm.Sha2_256))
                    {
                        resultStream.Seek(0, SeekOrigin.Begin);
                        byte[] hmacBuffer = HmacSha256.Compute(resultStream, _infoV1.MyHmacKey);

                        resultStream.Seek(0, SeekOrigin.End);
                        resultStream.Write(hmacBuffer, 0, hmacBuffer.Length);
                    }

                    resultStream.Seek(0, SeekOrigin.Begin);
                    return resultStream;
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

            throw new SecureConnectionException("Conversion failed.");
        }

        private Stream Decrypt(Stream stream)
        {
            if (stream == null) return null;

            try
            {
                using (var targetStream = new RangeStream(stream, stream.Position, stream.Length - stream.Position, false))
                {
                    if (_version.HasFlag(SecureConnectionVersion.Version1))
                    {
                        byte[] totalReceiveSizeBuffer = new byte[8];
                        if (targetStream.Read(totalReceiveSizeBuffer, 0, totalReceiveSizeBuffer.Length) != totalReceiveSizeBuffer.Length) throw new SecureConnectionException();
                        long totalReceiveSize = NetworkConverter.ToInt64(totalReceiveSizeBuffer);

                        if (_infoV1.HashAlgorithm.HasFlag(V1.HashAlgorithm.Sha2_256))
                        {
                            const int hashLength = 32;

                            _totalReceiveSize.Add(targetStream.Length - (8 + hashLength));

                            if (totalReceiveSize != _totalReceiveSize) throw new SecureConnectionException();

                            byte[] otherHmacBuffer = new byte[hashLength];
                            targetStream.Seek(-hashLength, SeekOrigin.End);
                            if (targetStream.Read(otherHmacBuffer, 0, otherHmacBuffer.Length) != otherHmacBuffer.Length) throw new SecureConnectionException();
                            targetStream.SetLength(targetStream.Length - hashLength);
                            targetStream.Seek(0, SeekOrigin.Begin);

                            byte[] myHmacBuffer = HmacSha256.Compute(targetStream, _infoV1.OtherHmacKey);

                            if (!Unsafe.Equals(otherHmacBuffer, myHmacBuffer)) throw new SecureConnectionException();

                            targetStream.Seek(8, SeekOrigin.Begin);
                        }

                        var resultStream = new RecyclableMemoryStream(_bufferPool);

                        if (_infoV1.CryptoAlgorithm.HasFlag(V1.CryptoAlgorithm.Aes256))
                        {
                            byte[] iv = new byte[16];
                            targetStream.Read(iv, 0, iv.Length);

                            using (var aes = Aes.Create())
                            {
                                aes.KeySize = 256;
                                aes.Mode = CipherMode.CBC;
                                aes.Padding = PaddingMode.PKCS7;

                                using (var cs = new CryptoStream(new WrapperStream(resultStream), aes.CreateDecryptor(_infoV1.OtherCryptoKey, iv), CryptoStreamMode.Write))
                                using (var safeBuffer = _bufferPool.CreateSafeBuffer(1024 * 4))
                                {
                                    int readLength;

                                    while ((readLength = targetStream.Read(safeBuffer.Value, 0, safeBuffer.Value.Length)) > 0)
                                    {
                                        cs.Write(safeBuffer.Value, 0, readLength);
                                    }
                                }
                            }
                        }

                        resultStream.Seek(0, SeekOrigin.Begin);
                        return resultStream;
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

            throw new SecureConnectionException("Conversion failed.");
        }
        */
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
                if (_random != null)
                {
                    try
                    {
                        _random.Dispose();
                    }
                    catch (Exception)
                    {

                    }

                    _random = null;
                }
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
