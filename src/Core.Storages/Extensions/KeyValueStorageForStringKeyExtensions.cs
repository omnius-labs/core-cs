using System.Buffers;
using Omnius.Core.Base;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Storages;

public static class KeyValueStorageExtensions
{
    public static async ValueTask<TValue?> TryReadAsync<TValue>(this IKeyValueStorage storage, string key, CancellationToken cancellationToken = default)
        where TValue : IRocketPackStruct<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var bytesPipe = new BytesPipe(bytesPool);

        if (!await storage.TryReadAsync(key, bytesPipe.Writer, cancellationToken)) return default;

        var value = RocketPackStruct.Import<TValue>(bytesPipe.Reader.GetSequence(), bytesPool);
        return value;
    }

    public static async ValueTask WriteAsync<TValue>(this IKeyValueStorage storage, string key, TValue value, CancellationToken cancellationToken = default)
        where TValue : IRocketPackStruct<TValue>
    {
        var bytesPool = BytesPool.Shared;
        using var memoryOwner = RocketPackStruct.Export(value, bytesPool);
        await storage.WriteAsync(key, new ReadOnlySequence<byte>(memoryOwner.Memory), cancellationToken);
    }
}
