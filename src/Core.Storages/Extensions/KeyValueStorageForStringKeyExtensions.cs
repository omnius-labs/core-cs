using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Storages;

public static class KeyValueStorageExtensions
{
    public static async ValueTask<TValue?> TryReadAsync<TValue>(this IKeyValueStorage storage, string key, CancellationToken cancellationToken = default)
        where TValue : RocketMessage<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var bytesPipe = new BytesPipe(bytesPool);

        if (!await storage.TryReadAsync(key, bytesPipe.Writer, cancellationToken)) return RocketMessage<TValue>.Empty;

        var value = RocketMessage<TValue>.Import(bytesPipe.Reader.GetSequence(), bytesPool);
        return value;
    }

    public static async ValueTask WriteAsync<TValue>(this IKeyValueStorage storage, string key, TValue value, CancellationToken cancellationToken = default)
        where TValue : RocketMessage<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var bytesPipe = new BytesPipe(bytesPool);

        if (value is not RocketMessage<TValue> rocketPackObject) throw new NotSupportedException();
        rocketPackObject.Export(bytesPipe.Writer, bytesPool);

        await storage.WriteAsync(key, bytesPipe.Reader.GetSequence(), cancellationToken);
    }
}
