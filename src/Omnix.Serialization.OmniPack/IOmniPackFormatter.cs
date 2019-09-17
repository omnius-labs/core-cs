namespace Omnix.Serialization.OmniPack
{
    public interface IOmniPackFormatter<T>
    {
        void Serialize(ref OmniPackWriter writer, in T value, in int rank);
        T Deserialize(ref OmniPackReader reader, in int rank);
    }
}
