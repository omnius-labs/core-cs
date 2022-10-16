namespace Omnius.Core.RocketPack;

public interface IRocketMessageFormatter<T>
{
    void Serialize(ref RocketMessageWriter writer, in T value, in int rank);
    T Deserialize(ref RocketMessageReader reader, in int rank);
}
