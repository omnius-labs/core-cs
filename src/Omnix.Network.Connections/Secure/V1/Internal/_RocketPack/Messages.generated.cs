using Omnix.Algorithms.Cryptography;

#nullable enable

namespace Omnix.Network.Connections.Secure.V1.Internal
{
    internal enum KeyExchangeAlgorithm : byte
    {
        EcDh_P521_Sha2_256 = 0,
    }

    internal enum KeyDerivationAlgorithm : byte
    {
        Pbkdf2 = 0,
    }

    internal enum HashAlgorithm : byte
    {
        Sha2_256 = 0,
    }

    internal enum CryptoAlgorithm : byte
    {
        Aes_256 = 0,
    }

    internal enum AuthenticationType : byte
    {
        None = 0,
        Password = 1,
    }

}
