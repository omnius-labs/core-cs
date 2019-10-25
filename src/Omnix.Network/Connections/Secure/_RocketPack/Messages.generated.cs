using Omnix.Cryptography;

#nullable enable

namespace Omnix.Network.Connections.Secure
{
    public enum OmniSecureConnectionType : byte
    {
        Connected = 0,
        Accepted = 1,
    }

    public enum OmniSecureConnectionVersion : byte
    {
        Version1 = 1,
    }

}
