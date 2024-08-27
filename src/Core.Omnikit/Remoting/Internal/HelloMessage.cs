using System.Buffers;
using System.Collections.Concurrent;
using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting.Internal;

internal enum OmniRemotingVersion
{
    Unknown = 0,
    V1 = 1
}

internal class HelloMessage : RocketMessage<HelloMessage>
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

    public override bool Equals(HelloMessage? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Version == other.Version && this.FunctionId == other.FunctionId;
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<HelloMessage>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in HelloMessage value, scoped in int depth)
        {
            w.Write(value.Version.ToString());
            w.Write(value.FunctionId);
        }
        public HelloMessage Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            var version = Enum.Parse<OmniRemotingVersion>(r.GetString(1024));
            var functionId = r.GetUInt32();

            return new HelloMessage()
            {
                Version = version,
                FunctionId = functionId,
            };
        }
    }
}
