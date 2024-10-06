using System.Text;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Omnikit.Remoting;

public class OmniRemotingDefaultErrorMessage : RocketMessage<OmniRemotingDefaultErrorMessage>
{
    public required string Type { get; init; }
    public required string Message { get; init; }
    public required string StackTrace { get; init; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(this.Type);
        sb.Append(": ");
        sb.Append(this.Message);
        sb.Append("\n");
        sb.Append(this.StackTrace);
        return sb.ToString();
    }

    private int? _hashCode;

    public override int GetHashCode()
    {
        if (_hashCode is null)
        {
            var h = new HashCode();
            h.Add(this.Type);
            h.Add(this.Message);
            h.Add(this.StackTrace);
            _hashCode = h.ToHashCode();
        }

        return _hashCode.Value;
    }

    public override bool Equals(OmniRemotingDefaultErrorMessage? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return this.Type == other.Type && this.Message == other.Message && this.StackTrace == other.StackTrace;
    }

    static OmniRemotingDefaultErrorMessage()
    {
        Formatter = new CustomSerializer();
        Empty = new OmniRemotingDefaultErrorMessage() { Type = string.Empty, Message = string.Empty, StackTrace = string.Empty };
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<OmniRemotingDefaultErrorMessage>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in OmniRemotingDefaultErrorMessage value, scoped in int depth)
        {
            w.Write(value.Type);
            w.Write(value.Message);
            w.Write(value.StackTrace);
        }
        public OmniRemotingDefaultErrorMessage Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            var type = r.GetString(1024);
            var message = r.GetString(1024 * 8);
            var stackTrace = r.GetString(1024 * 32);

            return new OmniRemotingDefaultErrorMessage()
            {
                Type = type,
                Message = message,
                StackTrace = stackTrace,
            };
        }
    }
}
