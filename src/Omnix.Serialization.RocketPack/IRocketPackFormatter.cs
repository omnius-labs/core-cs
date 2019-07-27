namespace Omnix.Serialization.RocketPack
{
    public interface IRocketPackFormatter<T>
    {
        void Serialize(RocketPackWriter writer, T value, int rank);
        T Deserialize(RocketPackReader reader, int rank);
    }
}