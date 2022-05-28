using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Storages;

public static class KeyValueStorageForStringKeyExtensions
{
    public static async ValueTask<TValue?> TryReadAsync<TValue>(this IKeyValueStorage<string> storage, string key, CancellationToken cancellationToken = default)
        where TValue : IRocketMessage<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var bytesPipe = new BytesPipe(bytesPool);

        if (!await storage.TryReadAsync(key, bytesPipe.Writer, cancellationToken)) return default;

        var value = IRocketMessage<TValue>.Import(bytesPipe.Reader.GetSequence(), bytesPool);
        return value;
    }

    public static async ValueTask WriteAsync<TValue>(this IKeyValueStorage<string> storage, string key, TValue value, CancellationToken cancellationToken = default)
        where TValue : IRocketMessage<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var bytesPipe = new BytesPipe(bytesPool);

        if (value is not IRocketMessage<TValue> rocketPackObject) throw new NotSupportedException();
        rocketPackObject.Export(bytesPipe.Writer, bytesPool);

        await storage.WriteAsync(key, bytesPipe.Reader.GetSequence(), cancellationToken);
    }
}
