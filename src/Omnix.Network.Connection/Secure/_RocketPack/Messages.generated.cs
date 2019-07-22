using Omnix.Algorithms.Cryptography;

#nullable enable

namespace Omnix.Network.Connection.Secure
{
    public enum OmniSecureConnectionType : byte
    {
        Connect = 0,
        Accept = 1,
    }

    public enum OmniSecureConnectionVersion : byte
    {
        Version1 = 1,
    }

}
