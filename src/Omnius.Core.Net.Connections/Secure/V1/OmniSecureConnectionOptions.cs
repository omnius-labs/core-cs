using Omnius.Core.Cryptography;

namespace Omnius.Core.Net.Connections.Secure.V1;

public record OmniSecureConnectionOptions
{
    public OmniSecureConnectionOptions(OmniSecureConnectionType type, OmniDigitalSignature? digitalSignature, int maxReceiveByteCount)
    {
        this.Type = type;
        this.DigitalSignature = digitalSignature;
        this.MaxReceiveByteCount = maxReceiveByteCount;
    }

    public OmniSecureConnectionType Type { get; }

    public OmniDigitalSignature? DigitalSignature { get; }

    public int MaxReceiveByteCount { get; }
}
