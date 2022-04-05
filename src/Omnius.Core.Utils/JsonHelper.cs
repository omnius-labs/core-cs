using System.Text.Json;

namespace Omnius.Core.Utils;

public static class JsonHelper
{
    public static T? ReadFile<T>(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return ReadStream<T>(stream);
    }

    public static T? ReadStream<T>(Stream stream)
    {
        var serializeOptions = new JsonSerializerOptions();
        return JsonSerializer.Deserialize<T>(stream, serializeOptions);
    }

    public static void WriteFile<T>(string path, T value, bool writeIndented = false)
    {
        using var stream = new FileStream(path, FileMode.Create);
        WriteStream<T>(stream, value, writeIndented);
    }

    public static void WriteStream<T>(Stream stream, T value, bool writeIndented = false)
    {
        var serializeOptions = new JsonSerializerOptions() { WriteIndented = writeIndented };
        JsonSerializer.Serialize(stream, value, serializeOptions);
    }
    public static async ValueTask<T?> ReadFileAsync<T>(string path)
    {
        using var stream = new FileStream(path, FileMode.Open);
        return await ReadStreamAsync<T>(stream);
    }

    public static async ValueTask<T?> ReadStreamAsync<T>(Stream stream)
    {
        var serializeOptions = new JsonSerializerOptions();
        return await JsonSerializer.DeserializeAsync<T>(stream, serializeOptions);
    }

    public static async ValueTask WriteFileAsync<T>(string path, T value, bool writeIndented = false)
    {
        using var stream = new FileStream(path, FileMode.Create);
        await WriteStreamAsync<T>(stream, value, writeIndented);
    }

    public static async ValueTask WriteStreamAsync<T>(Stream stream, T value, bool writeIndented = false)
    {
        var serializeOptions = new JsonSerializerOptions() { WriteIndented = writeIndented };
        await JsonSerializer.SerializeAsync(stream, value, serializeOptions);
    }
}
