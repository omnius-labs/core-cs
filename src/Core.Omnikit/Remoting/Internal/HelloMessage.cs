using System.Buffers;
using System.Collections.Concurrent;
using System.Runtime.Serialization;
using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting.Internal;

internal enum OmniRemotingVersion
{
    [EnumMember(Value = "unknown")]
    Unknown,

    [EnumMember(Value = "v1")]
    V1 = 1
}

internal class HelloMessage : IRocketPackStruct<HelloMessage>, IEquatable<HelloMessage>
{
    public required OmniRemotingVersion Version { get; init; }
    public required uint FunctionId { get; init; }

    private int? _hashCode;

    public override int GetHashCode()
    {
        if (_hashCode is null)
        {
            var h = new HashCode();
            h.Add(this.Version);
            h.Add(this.FunctionId);
            _hashCode = h.ToHashCode();
        }

        return _hashCode.Value;
    }

    public bool Equals(HelloMessage? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Version == other.Version && this.FunctionId == other.FunctionId;
    }

    public static void Pack(ref RocketPackBytesEncoder encoder, in HelloMessage value)
    {
        encoder.WriteMap(2);
        encoder.WriteU64(0);
        encoder.WriteString(EnumAlias<OmniRemotingVersion>.ToStringAlias(value.Version));
        encoder.WriteU64(1);
        encoder.WriteU32(value.FunctionId);
    }

    public static HelloMessage Unpack(ref RocketPackBytesDecoder decoder)
    {
        var count = decoder.ReadMap();
        OmniRemotingVersion? version = null;
        uint? functionId = null;

        for (ulong i = 0; i < count; i++)
        {
            switch (decoder.ReadU64())
            {
                case 0:
                    version = EnumAlias<OmniRemotingVersion>.ParseAlias(decoder.ReadString());
                    break;
                case 1:
                    functionId = decoder.ReadU32();
                    break;
                default:
                    decoder.SkipField();
                    break;
            }
        }

        return new HelloMessage()
        {
            Version = version ?? throw RocketPackDecoderException.CreateOther("missing field: Version"),
            FunctionId = functionId ?? throw RocketPackDecoderException.CreateOther("missing field: FunctionId"),
        };
    }
}
