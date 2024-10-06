
namespace Omnius.Core.RocketPack;

public sealed class EmptyRocketMessage : RocketMessage<EmptyRocketMessage>
{
    public override int GetHashCode() => 0;

    public override bool Equals(EmptyRocketMessage? other)
    {
        return (other is not null);
    }

    static EmptyRocketMessage()
    {
        Formatter = new CustomSerializer();
        Empty = new EmptyRocketMessage();
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<EmptyRocketMessage>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in EmptyRocketMessage value, scoped in int depth)
        {
        }
        public EmptyRocketMessage Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            return Empty;
        }
    }
}
