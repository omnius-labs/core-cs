namespace Omnix.Serialization.RocketPack
{
    public interface IRocketPackFormatter<T>
    {
        void Serialize(ref RocketPackWriter writer, T value, int rank);
        T Deserialize(ref RocketPackReader reader, int rank);
    }
}
