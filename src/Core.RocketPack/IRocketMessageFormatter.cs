namespace Omnius.Core.RocketPack;

public interface IRocketMessageFormatter<T>
{
    void Serialize(ref RocketMessageWriter writer, scoped in T value, scoped in int rank);
    T Deserialize(ref RocketMessageReader reader, scoped in int rank);
}
