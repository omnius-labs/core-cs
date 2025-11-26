
namespace Omnius.Core.RocketPack;

public sealed class EmptyRocketMessage : IRocketPackStruct<EmptyRocketMessage>
{
    public static void Pack(ref RocketPackBytesEncoder encoder, in EmptyRocketMessage value)
    {
    }

    public static EmptyRocketMessage Unpack(ref RocketPackBytesDecoder decoder)
    {
        return new EmptyRocketMessage();
    }
}
