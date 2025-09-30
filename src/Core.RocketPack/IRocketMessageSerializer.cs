namespace Omnius.Core.RocketPack;

// TODO
public interface IRocketMessageSerializer<T>
{
    void Serialize(ref RocketMessageWriter writer, scoped in T value, scoped in int depth);
    T Deserialize(ref RocketMessageReader reader, scoped in int depth);
}
