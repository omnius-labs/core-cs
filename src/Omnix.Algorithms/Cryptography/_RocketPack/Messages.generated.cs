
#nullable enable

namespace Omnix.Algorithms.Cryptography
{
    public enum OmniHashAlgorithmType : byte
    {
        Sha2_256 = 0,
    }

    public enum OmniAgreementAlgorithmType : byte
    {
        EcDh_P521_Sha2_256 = 0,
    }

    public enum OmniDigitalSignatureAlgorithmType : byte
    {
        EcDsa_P521_Sha2_256 = 0,
    }

    public enum OmniHashcashAlgorithmType : byte
    {
        Simple_Sha2_256 = 0,
    }

}
