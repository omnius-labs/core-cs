using System.Buffers;
using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Storages;

public static class SingleValueStorageExtensions
{
    public static async ValueTask<TValue?> TryGetValueAsync<TValue>(this ISingleValueStorage storage, CancellationToken cancellationToken = default)
        where TValue : IRocketPackStruct<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var bytesPipe = new BytesPipe(bytesPool);

        if (!await storage.TryReadAsync(bytesPipe.Writer, cancellationToken)) return default;

        var value = RocketPackStruct.Import<TValue>(bytesPipe.Reader.GetSequence(), bytesPool);
        return value;
    }

    public static async ValueTask SetValueAsync<TValue>(this ISingleValueStorage storage, TValue value, CancellationToken cancellationToken = default)
        where TValue : IRocketPackStruct<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var memoryOwner = RocketPackStruct.Export(value, bytesPool);
        await storage.WriteAsync(new ReadOnlySequence<byte>(memoryOwner.Memory), cancellationToken);
    }
}
