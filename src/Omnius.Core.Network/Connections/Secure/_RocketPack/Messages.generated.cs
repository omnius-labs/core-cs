using Omnius.Core.Cryptography;

#nullable enable

namespace Omnius.Core.Network.Connections.Secure
{
    public enum OmniSecureConnectionType : byte
    {
        Unknown = 0,
        Connected = 1,
        Accepted = 2,
    }

    public enum OmniSecureConnectionVersion : byte
    {
        Unknown = 0,
        Version1 = 1,
    }

}
