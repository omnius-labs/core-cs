using System.Text.Json;

namespace Omnius.Core.Base.Serialization;

public static class JsonHelper
{
    public static T? ReadFile<T>(string path, JsonSerializerOptions? options = null)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return ReadStream<T>(stream, options);
    }

    public static T? ReadStream<T>(Stream stream, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(stream, options);
    }

    public static void WriteFile<T>(string path, T value, JsonSerializerOptions? options = null)
    {
        using var stream = new FileStream(path, FileMode.Create);
        WriteStream<T>(stream, value, options);
    }

    public static void WriteStream<T>(Stream stream, T value, JsonSerializerOptions? options = null)
    {
        JsonSerializer.Serialize(stream, value, options);
    }

    public static async ValueTask<T?> ReadFileAsync<T>(string path, JsonSerializerOptions? options = null)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return await ReadStreamAsync<T>(stream, options);
    }

    public static async ValueTask<T?> ReadStreamAsync<T>(Stream stream, JsonSerializerOptions? options = null)
    {
        return await JsonSerializer.DeserializeAsync<T>(stream, options);
    }

    public static async ValueTask WriteFileAsync<T>(string path, T value, JsonSerializerOptions? options = null)
    {
        using var stream = new FileStream(path, FileMode.Create);
        await WriteStreamAsync<T>(stream, value, options);
    }

    public static async ValueTask WriteStreamAsync<T>(Stream stream, T value, JsonSerializerOptions? options = null)
    {
        await JsonSerializer.SerializeAsync(stream, value, options);
    }
}
