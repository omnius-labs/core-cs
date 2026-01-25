using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Omnius.Core.Base.Serialization;

public record YamlSerializerOptions
{
}

public static class YamlHelper
{
    public static T ReadFile<T>(string path, YamlSerializerOptions? options = null)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return ReadStream<T>(stream);
    }

    public static T ReadStream<T>(Stream stream, YamlSerializerOptions? options = null)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(PascalCaseNamingConvention.Instance)
            .Build();
        using var reader = new StreamReader(stream);
        return deserializer.Deserialize<T>(reader);
    }

    public static void WriteFile(string path, object value, YamlSerializerOptions? options = null)
    {
        using var stream = new FileStream(path, FileMode.Create);
        WriteStream(stream, value);
    }

    public static void WriteStream(Stream stream, object value, YamlSerializerOptions? options = null)
    {
        var serializer = new SerializerBuilder()
            .Build();
        using var writer = new StreamWriter(stream, new UTF8Encoding(false));
        serializer.Serialize(writer, value);
    }
}
