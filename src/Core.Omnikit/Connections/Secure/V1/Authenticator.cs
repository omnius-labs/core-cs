using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Omnius.Core.Base;
using Omnius.Core.Omnikit.Connections.Codec;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;

namespace Omnius.Core.Omnikit.Connections.Secure.V1;

class Authenticator
{
    private readonly OmniSecureStreamType _type;
    private readonly FramedReceiver _receiver;
    private readonly FramedSender _sender;
    private readonly OmniSigner? _signer;
    private readonly IRandomBytesProvider _randomBytesProvider;
    private readonly IClock _clock;
    private readonly IBytesPool _bytesPool;

    public Authenticator(OmniSecureStreamType type, FramedReceiver receiver, FramedSender sender, OmniSigner? signer, IRandomBytesProvider randomBytesProvider, IClock clock, IBytesPool bytesPool)
    {
        _type = type;
        _receiver = receiver;
        _sender = sender;
        _signer = signer;
        _randomBytesProvider = randomBytesProvider;
        _clock = clock;
        _bytesPool = bytesPool;
    }

    public async ValueTask<AuthResult> AuthenticationAsync(CancellationToken cancellationToken = default)
    {
        var myProfileMessage = new ProfileMessage()
        {
            SessionId = _randomBytesProvider.GetBytes(32),
            AuthType = _signer is null ? AuthType.None : AuthType.Sign,
            KeyExchangeAlgorithmType = KeyExchangeAlgorithmType.X25519,
            KeyDerivationAlgorithmType = KeyDerivationAlgorithmType.Hkdf,
            CipherAlgorithmType = CipherAlgorithmType.Aes256Gcm,
            HashAlgorithmType = HashAlgorithmType.Sha3_256,
        };

        using var sendingProfileMessageBytes = myProfileMessage.Export(_bytesPool);
        await _sender.SendAsync(sendingProfileMessageBytes.Memory, cancellationToken);

        using var receivedProfileMessageBytes = await _receiver.ReceiveAsync(cancellationToken);
        var otherProfileMessage = ProfileMessage.Import(receivedProfileMessageBytes.Memory, _bytesPool);

        var keyExchangeAlgorithmType = myProfileMessage.KeyExchangeAlgorithmType & otherProfileMessage.KeyExchangeAlgorithmType;
        var keyDerivationAlgorithmType = myProfileMessage.KeyDerivationAlgorithmType & otherProfileMessage.KeyDerivationAlgorithmType;
        var cipherAlgorithmType = myProfileMessage.CipherAlgorithmType & otherProfileMessage.CipherAlgorithmType;
        var hashAlgorithmType = myProfileMessage.HashAlgorithmType & otherProfileMessage.HashAlgorithmType;

        string? otherSign = null;
        byte[] secret;

        if (keyExchangeAlgorithmType == KeyExchangeAlgorithmType.X25519)
        {
            var now = _clock.GetUtcNow();
            var myAgreement = OmniAgreement.Create(now, OmniAgreementAlgorithmType.X25519);

            using var sendingAgreementPublicKeyBytes = myAgreement.GenAgreementPublicKey().Export(_bytesPool);
            await _sender.SendAsync(sendingAgreementPublicKeyBytes.Memory, cancellationToken);

            using var receivedAgreementPublicKeyBytes = await _receiver.ReceiveAsync(cancellationToken);
            var otherAgreementPublicKey = OmniAgreementPublicKey.Import(receivedAgreementPublicKeyBytes.Memory, _bytesPool);

            if (_signer is not null)
            {
                var myHash = GenHash(myProfileMessage, otherAgreementPublicKey, hashAlgorithmType);
                var myCert = _signer.Sign(myHash);

                using var sendingCertBytes = myCert.Export(_bytesPool);
                await _sender.SendAsync(sendingCertBytes.Memory, cancellationToken);
            }

            if (otherProfileMessage.AuthType == AuthType.Sign)
            {
                using var receivedCertBytes = await _receiver.ReceiveAsync(cancellationToken);
                var otherCert = OmniCert.Import(receivedCertBytes.Memory, _bytesPool);

                var otherHash = GenHash(otherProfileMessage, myAgreement.GenAgreementPublicKey(), hashAlgorithmType);
                if (!otherCert.Verify(otherHash)) throw new SecureStreamException("Invalid cert");
                otherSign = otherCert.ToString();
            }

            secret = OmniAgreement.GenSecret(myAgreement.GenAgreementPrivateKey(), otherAgreementPublicKey);
        }
        else
        {
            throw new SecureStreamException("Unsupported key exchange algorithm type");
        }

        if (keyDerivationAlgorithmType == KeyDerivationAlgorithmType.Hkdf)
        {
            var salt = new byte[Math.Min(myProfileMessage.SessionId.Length, otherProfileMessage.SessionId.Length)];
            BytesOperations.Xor(myProfileMessage.SessionId, otherProfileMessage.SessionId, salt);

            var (keyLen, nonceLen) = cipherAlgorithmType switch
            {
                CipherAlgorithmType.Aes256Gcm => (32, 12),
                _ => throw new SecureStreamException("Unsupported cipher algorithm type"),
            };

            if (hashAlgorithmType == HashAlgorithmType.Sha3_256)
            {
                var hmacKeyParams = new HkdfParameters(secret, salt, null);
                var hmacKeyGenerator = new HkdfBytesGenerator(new Sha3Digest(256));

                var okm = new byte[(keyLen + nonceLen) * 2];
                hmacKeyGenerator.Init(hmacKeyParams);
                hmacKeyGenerator.GenerateBytes(okm, 0, okm.Length);

                var (encOffset, decOffset) = _type switch
                {
                    OmniSecureStreamType.Connected => (0, keyLen + nonceLen),
                    OmniSecureStreamType.Accepted => (keyLen + nonceLen, 0),
                    _ => throw new SecureStreamException("Unsupported secure stream type"),
                };

                var encKey = okm[encOffset..(encOffset + keyLen)].ToArray();
                var encNonce = okm[(encOffset + keyLen)..(encOffset + keyLen + nonceLen)].ToArray();
                var decKey = okm[decOffset..(decOffset + keyLen)].ToArray();
                var decNonce = okm[(decOffset + keyLen)..(decOffset + keyLen + nonceLen)].ToArray();

                var authResult = new AuthResult()
                {
                    Sign = otherSign,
                    CipherAlgorithmType = cipherAlgorithmType,
                    EncKey = encKey,
                    EncNonce = encNonce,
                    DecKey = decKey,
                    DecNonce = decNonce,
                };

                return authResult;
            }
            else
            {
                throw new SecureStreamException("Unsupported hash algorithm type");
            }
        }
        else
        {
            throw new SecureStreamException("Unsupported key derivation algorithm type");
        }
    }

    private static byte[] GenHash(ProfileMessage profileMessage, OmniAgreementPublicKey agreementPublicKey, HashAlgorithmType hashAlgorithmType)
    {
        if (hashAlgorithmType == HashAlgorithmType.Sha3_256)
        {
            using var sha3 = SHA3_256.Create();
            sha3.TransformBlock(profileMessage.SessionId, 0, profileMessage.SessionId.Length, null, 0);
            sha3.TransformBlock(BitConverter.GetBytes((uint)profileMessage.AuthType), 0, 4, null, 0);
            sha3.TransformBlock(BitConverter.GetBytes((uint)profileMessage.KeyExchangeAlgorithmType), 0, 4, null, 0);
            sha3.TransformBlock(BitConverter.GetBytes((uint)profileMessage.KeyDerivationAlgorithmType), 0, 4, null, 0);
            sha3.TransformBlock(BitConverter.GetBytes((uint)profileMessage.CipherAlgorithmType), 0, 4, null, 0);
            sha3.TransformBlock(BitConverter.GetBytes((uint)profileMessage.HashAlgorithmType), 0, 4, null, 0);
            sha3.TransformBlock(BitConverter.GetBytes(((DateTimeOffset)agreementPublicKey.CreatedTime).ToUnixTimeSeconds()), 0, 8, null, 0);
            sha3.TransformBlock(BitConverter.GetBytes((uint)agreementPublicKey.AlgorithmType), 0, 4, null, 0);
            sha3.TransformBlock(agreementPublicKey.PublicKey, 0, agreementPublicKey.PublicKey.Length, null, 0);
            return sha3.TransformFinalBlock([], 0, 0);
        }
        else
        {
            throw new SecureStreamException("Unsupported hash algorithm type");
        }
    }
}

record AuthResult
{
    public required string? Sign { get; init; }
    public required CipherAlgorithmType CipherAlgorithmType { get; init; }
    public required byte[] EncKey { get; init; }
    public required byte[] EncNonce { get; init; }
    public required byte[] DecKey { get; init; }
    public required byte[] DecNonce { get; init; }
}
