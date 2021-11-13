using Omnius.Core.Cryptography;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Net.Connections.Secure.V1.Internal;

internal partial class AuthenticationMessage
{
    public static AuthenticationMessage Create(DateTime creationTime, ReadOnlyMemory<byte> hash, OmniDigitalSignature? digitalSignature)
    {
        if (digitalSignature is null) return new AuthenticationMessage(Timestamp.FromDateTime(creationTime), hash, null);

        using var bytesPipe = new BytesPipe();
        var target = new AuthenticationMessage(Timestamp.FromDateTime(creationTime), hash, null);
        target.Export(bytesPipe.Writer, BytesPool.Shared);

        var certificate = OmniDigitalSignature.CreateOmniCertificate(digitalSignature, bytesPipe.Reader.GetSequence());
        return new AuthenticationMessage(Timestamp.FromDateTime(creationTime), hash, certificate);
    }

    public bool Verify()
    {
        using var bytesPipe = new BytesPipe();
        var target = new AuthenticationMessage(this.CreationTime, this.Hash, null);
        target.Export(bytesPipe.Writer, BytesPool.Shared);

        return this.Certificate?.Verify(bytesPipe.Reader.GetSequence()) ?? false;
    }
}