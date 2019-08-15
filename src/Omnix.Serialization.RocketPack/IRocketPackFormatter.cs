namespace Omnix.Serialization.RocketPack
{
    public interface IRocketPackFormatter<T>
    {
        void Serialize(ref RocketPackWriter writer, in T value, in int rank);
        T Deserialize(ref RocketPackReader reader, in int rank);
    }
}
