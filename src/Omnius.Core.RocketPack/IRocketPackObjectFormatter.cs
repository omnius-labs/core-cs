namespace Omnius.Core.RocketPack
{
    public interface IRocketPackObjectFormatter<T>
    {
        void Serialize(ref RocketPackObjectWriter writer, in T value, in int rank);

        T Deserialize(ref RocketPackObjectReader reader, in int rank);
    }
}
