using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Storages;

public static class SingleValueStorageExtensions
{
    public static async ValueTask<TValue?> TryGetValueAsync<TValue>(this ISingleValueStorage storage, CancellationToken cancellationToken = default)
        where TValue : RocketMessage<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var bytesPipe = new BytesPipe(bytesPool);

        if (!await storage.TryReadAsync(bytesPipe.Writer, cancellationToken)) return default;

        var value = RocketMessage<TValue>.Import(bytesPipe.Reader.GetSequence(), bytesPool);
        return value;
    }

    public static async ValueTask SetValueAsync<TValue>(this ISingleValueStorage storage, TValue value, CancellationToken cancellationToken = default)
        where TValue : RocketMessage<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var bytesPipe = new BytesPipe(bytesPool);

        if (value is not RocketMessage<TValue> rocketPackObject) throw new NotSupportedException();
        rocketPackObject.Export(bytesPipe.Writer, bytesPool);

        await storage.WriteAsync(bytesPipe.Reader.GetSequence(), cancellationToken);
    }
}
