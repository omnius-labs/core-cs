using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Storages;

public static class SingleValueStorageExtensions
{
    public static async ValueTask<TValue?> TryGetValueAsync<TValue>(this ISingleValueStorage storage, CancellationToken cancellationToken = default)
        where TValue : IRocketMessage<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var bytesPipe = new BytesPipe(bytesPool);

        if (!await storage.TryReadAsync(bytesPipe.Writer, cancellationToken)) return default;

        var value = IRocketMessage<TValue>.Import(bytesPipe.Reader.GetSequence(), bytesPool);
        return value;
    }

    public static async ValueTask<bool> TrySetValueAsync<TValue>(this ISingleValueStorage storage, TValue value, CancellationToken cancellationToken = default)
        where TValue : IRocketMessage<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var bytesPipe = new BytesPipe(bytesPool);

        if (value is not IRocketMessage<TValue> rocketPackObject) throw new NotSupportedException();
        rocketPackObject.Export(bytesPipe.Writer, bytesPool);

        return await storage.TryWriteAsync(bytesPipe.Reader.GetSequence(), cancellationToken);
    }
}