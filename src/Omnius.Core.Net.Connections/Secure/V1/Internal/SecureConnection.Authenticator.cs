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

namespace Omnius.Core.Net.Connections.Secure.V1.Internal
{
    public partial class SecureConnection
    {
        internal readonly struct AuthenticatedResult
        {
            public OmniSignature? Signature { get; init; }

            public CryptoAlgorithmType CryptoAlgorithmType { get; init; }

            public byte[] EncryptKey { get; init; }

            public byte[] DecryptKey { get; init; }

            public byte[] EncryptNonce { get; init; }

            public byte[] DecryptNonce { get; init; }
        }

        internal class Authenticator
        {
            private readonly IConnection _connection;
            private readonly OmniSecureConnectionType _type;
            private readonly OmniDigitalSignature? _digitalSignature;
            private readonly IBytesPool _bytesPool;

            public Authenticator(IConnection connection, OmniSecureConnectionType type, OmniDigitalSignature? digitalSignature, IBytesPool bytesPool)
            {
                _connection = connection;
                _type = type;
                _digitalSignature = digitalSignature;
                _bytesPool = bytesPool;
            }

            public async ValueTask<AuthenticatedResult> AuthenticateAsync(CancellationToken cancellationToken = default)
            {
                var myProfileMessage = new ProfileMessage(
                     GenSessionId(),
                     (_digitalSignature is null) ? AuthenticationType.None : AuthenticationType.Signature,
                     new[] { KeyExchangeAlgorithmType.EcDh_P521_Sha2_256 },
                     new[] { KeyDerivationAlgorithmType.Pbkdf2 },
                     new[] { CryptoAlgorithmType.Aes_Gcm_256 },
                     new[] { HashAlgorithmType.Sha2_256 });
                var otherProfileMessage = await this.ExchangeProfileMessageAsync(myProfileMessage, cancellationToken);

                ParseAlgorithmTypes(myProfileMessage, otherProfileMessage, out var keyExchangeAlgorithmType, out var keyDerivationAlgorithmType, out var cryptoAlgorithmType, out var hashAlgorithmType);

                OmniSignature? signature;
                byte[] secret;

                if (keyExchangeAlgorithmType.HasFlag(KeyExchangeAlgorithmType.EcDh_P521_Sha2_256))
                {
                    var myAgreement = OmniAgreement.Create(OmniAgreementAlgorithmType.EcDh_P521_Sha2_256);
                    var otherAgreementPublicKey = await this.ExchangeAgreementPublicKeyAsync(myAgreement.GetOmniAgreementPublicKey(), cancellationToken);

                    var myHash = this.ComputeHash(myProfileMessage, myAgreement.GetOmniAgreementPublicKey(), hashAlgorithmType);
                    var otherHash = this.ComputeHash(otherProfileMessage, otherAgreementPublicKey, hashAlgorithmType);

                    var myAuthenticationMessage = AuthenticationMessage.Create(DateTime.UtcNow, myHash, _digitalSignature);
                    var otherAuthenticationMessage = await this.ExchangeAuthenticationMessageAsync(myAuthenticationMessage, cancellationToken);
                    if ((DateTime.UtcNow - otherAuthenticationMessage.CreationTime.ToDateTime()) > TimeSpan.FromMinutes(30)) throw new NotSupportedException();
                    if (!BytesOperations.Equals(otherAuthenticationMessage.Hash.Span, otherHash)) throw new NotSupportedException();

                    signature = otherAuthenticationMessage.Certificate?.GetOmniSignature();
                    secret = OmniAgreement.GetSecret(otherAgreementPublicKey, myAgreement.GetOmniAgreementPrivateKey());
                }
                else
                {
                    throw new NotSupportedException();
                }

                byte[] encryptKey;
                byte[] decryptKey;
                byte[] encryptNonce;
                byte[] decryptNonce;

                if (keyDerivationAlgorithmType.HasFlag(KeyDerivationAlgorithmType.Pbkdf2))
                {
                    byte[] xorSessionId = new byte[Math.Max(myProfileMessage.SessionId.Length, otherProfileMessage.SessionId.Length)];
                    BytesOperations.Xor(myProfileMessage.SessionId.Span, otherProfileMessage.SessionId.Span, xorSessionId);

                    int cryptoKeyLength = 0;
                    int nonceLength = 0;

                    if (cryptoAlgorithmType.HasFlag(CryptoAlgorithmType.Aes_Gcm_256))
                    {
                        cryptoKeyLength = 32;
                        nonceLength = 12;
                    }

                    encryptKey = new byte[cryptoKeyLength];
                    decryptKey = new byte[cryptoKeyLength];
                    encryptNonce = new byte[nonceLength];
                    decryptNonce = new byte[nonceLength];

                    var kdfResult = new byte[(cryptoKeyLength + nonceLength) * 2];

                    if (hashAlgorithmType.HasFlag(HashAlgorithmType.Sha2_256))
                    {
                        Pbkdf2_Sha2_256.TryComputeHash(secret.AsSpan(), xorSessionId, 1024, kdfResult);
                    }

                    using (var stream = new MemoryStream(kdfResult))
                    {
                        if (_type == OmniSecureConnectionType.Connected)
                        {
                            stream.Read(encryptKey, 0, encryptKey.Length);
                            stream.Read(decryptKey, 0, decryptKey.Length);
                            stream.Read(encryptNonce, 0, encryptNonce.Length);
                            stream.Read(decryptNonce, 0, decryptNonce.Length);
                        }
                        else if (_type == OmniSecureConnectionType.Accepted)
                        {
                            stream.Read(decryptKey, 0, decryptKey.Length);
                            stream.Read(encryptKey, 0, encryptKey.Length);
                            stream.Read(decryptNonce, 0, decryptNonce.Length);
                            stream.Read(encryptNonce, 0, encryptNonce.Length);
                        }
                    }

                    return new AuthenticatedResult()
                    {
                        Signature = signature,
                        CryptoAlgorithmType = cryptoAlgorithmType,
                        EncryptKey = encryptKey,
                        DecryptKey = decryptKey,
                        EncryptNonce = encryptNonce,
                        DecryptNonce = decryptNonce,
                    };
                }
                else
                {
                    throw new NotSupportedException(nameof(keyDerivationAlgorithmType));
                }
            }

            private static byte[] GenSessionId()
            {
                var sessionId = new byte[32];
                using var randomNumberGenerator = RandomNumberGenerator.Create();
                randomNumberGenerator.GetBytes(sessionId);

                return sessionId;
            }

            private static void ParseAlgorithmTypes(ProfileMessage myProfileMessage, ProfileMessage otherProfileMessage, out KeyExchangeAlgorithmType keyExchangeAlgorithmType, out KeyDerivationAlgorithmType keyDerivationAlgorithmType, out CryptoAlgorithmType cryptoAlgorithmType, out HashAlgorithmType hashAlgorithmType)
            {
                keyExchangeAlgorithmType = EnumHelper.GetOverlappedMaxValue(myProfileMessage.KeyExchangeAlgorithmTypes, otherProfileMessage.KeyExchangeAlgorithmTypes) ?? throw new OmniSecureConnectionException("key exchange algorithm does not match.");
                keyDerivationAlgorithmType = EnumHelper.GetOverlappedMaxValue(myProfileMessage.KeyDerivationAlgorithmTypes, otherProfileMessage.KeyDerivationAlgorithmTypes) ?? throw new OmniSecureConnectionException("key derivation algorithm does not match.");
                cryptoAlgorithmType = EnumHelper.GetOverlappedMaxValue(myProfileMessage.CryptoAlgorithmTypes, otherProfileMessage.CryptoAlgorithmTypes) ?? throw new OmniSecureConnectionException("Crypto algorithm does not match.");
                hashAlgorithmType = EnumHelper.GetOverlappedMaxValue(myProfileMessage.HashAlgorithmTypes, otherProfileMessage.HashAlgorithmTypes) ?? throw new OmniSecureConnectionException("Hash algorithm does not match.");
            }

            private async ValueTask<ProfileMessage> ExchangeProfileMessageAsync(ProfileMessage myProfileMessage, CancellationToken cancellationToken = default)
            {
                var enqueueTask = _connection.Sender.SendAsync(myProfileMessage, cancellationToken).AsTask();
                var dequeueTask = _connection.Receiver.ReceiveAsync<ProfileMessage>(cancellationToken).AsTask();

                await Task.WhenAll(enqueueTask, dequeueTask);
                var otherProfileMessage = dequeueTask.Result;

                if (otherProfileMessage is null) throw new NullReferenceException();
                if (myProfileMessage.AuthenticationType != otherProfileMessage.AuthenticationType) throw new OmniSecureConnectionException("AuthenticationType does not match.");

                return otherProfileMessage;
            }

            private async ValueTask<OmniAgreementPublicKey> ExchangeAgreementPublicKeyAsync(OmniAgreementPublicKey myAgreementPublicKey, CancellationToken cancellationToken = default)
            {
                var enqueueTask = _connection.Sender.SendAsync(myAgreementPublicKey, cancellationToken).AsTask();
                var dequeueTask = _connection.Receiver.ReceiveAsync<OmniAgreementPublicKey>(cancellationToken).AsTask();

                await Task.WhenAll(enqueueTask, dequeueTask);
                var otherAgreementPublicKey = dequeueTask.Result;
                if (otherAgreementPublicKey is null) throw new NullReferenceException();
                if ((DateTime.UtcNow - otherAgreementPublicKey.CreationTime.ToDateTime()).TotalMinutes > 30) throw new OmniSecureConnectionException("Agreement public key has Expired.");

                return otherAgreementPublicKey;
            }

            private async ValueTask<AuthenticationMessage> ExchangeAuthenticationMessageAsync(AuthenticationMessage myAuthenticationMessage, CancellationToken cancellationToken = default)
            {
                var enqueueTask = _connection.Sender.SendAsync(myAuthenticationMessage, cancellationToken).AsTask();
                var dequeueTask = _connection.Receiver.ReceiveAsync<AuthenticationMessage>(cancellationToken).AsTask();

                await Task.WhenAll(enqueueTask, dequeueTask);
                var otherAuthenticationMessage = dequeueTask.Result;
                if (otherAuthenticationMessage is null) throw new NullReferenceException();

                return otherAuthenticationMessage;
            }

            private byte[] ComputeHash(ProfileMessage profileMessage, OmniAgreementPublicKey agreementPublicKey, HashAlgorithmType hashAlgorithm)
            {
                var verificationMessage = new VerificationMessage(profileMessage, agreementPublicKey);

                if (hashAlgorithm == HashAlgorithmType.Sha2_256)
                {
                    using var bytesPipe = new BytesPipe();
                    verificationMessage.Export(bytesPipe.Writer, _bytesPool);

                    return Sha2_256.ComputeHash(bytesPipe.Reader.GetSequence());
                }

                throw new NotSupportedException(nameof(hashAlgorithm));
            }
        }
    }
}
