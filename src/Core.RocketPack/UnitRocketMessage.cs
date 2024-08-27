
namespace Omnius.Core.RocketPack;

public sealed class UnitRocketMessage : RocketMessage<UnitRocketMessage>
{
    public override int GetHashCode() => 0;

    public override bool Equals(UnitRocketMessage? other)
    {
        return (other is not null);
    }

    static UnitRocketMessage()
    {
        Formatter = new CustomSerializer();
        Empty = new UnitRocketMessage();
    }

    private sealed class CustomSerializer : IRocketMessageSerializer<UnitRocketMessage>
    {
        public void Serialize(ref RocketMessageWriter w, scoped in UnitRocketMessage value, scoped in int depth)
        {
        }
        public UnitRocketMessage Deserialize(ref RocketMessageReader r, scoped in int depth)
        {
            return Empty;
        }
    }
}
