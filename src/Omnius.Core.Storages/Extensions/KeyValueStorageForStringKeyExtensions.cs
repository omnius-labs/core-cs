using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Storages
{
    public static class KeyValueStorageForStringKeyExtensions
    {
        public static async IAsyncEnumerable<TValue> GetValuesAsync<TValue>(this IKeyValueStorage<string> storage, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            where TValue : IRocketMessage<TValue>
        {
            var bytesPool = BytesPool.Shared;
            using var bytesPipe = new BytesPipe(bytesPool);

            await foreach (var key in storage.GetKeysAsync(cancellationToken))
            {
                if (!await storage.TryReadAsync(key, bytesPipe.Writer, cancellationToken)) continue;

                var value = IRocketMessage<TValue>.Import(bytesPipe.Reader.GetSequence(), bytesPool);
                yield return value;

                bytesPipe.Reset();
            }
        }

        public static async ValueTask<TValue?> TryGetValueAsync<TValue>(this IKeyValueStorage<string> storage, string key, CancellationToken cancellationToken = default)
            where TValue : IRocketMessage<TValue>
        {
            var bytesPool = BytesPool.Shared;
            using var bytesPipe = new BytesPipe(bytesPool);

            if (!await storage.TryReadAsync(key, bytesPipe.Writer, cancellationToken)) return default;

            var value = IRocketMessage<TValue>.Import(bytesPipe.Reader.GetSequence(), bytesPool);
            return value;
        }

        public static async ValueTask<bool> TrySetValueAsync<TValue>(this IKeyValueStorage<string> storage, string key, TValue value, CancellationToken cancellationToken = default)
            where TValue : IRocketMessage<TValue>
        {
            var bytesPool = BytesPool.Shared;
            using var bytesPipe = new BytesPipe(bytesPool);

            if (value is not IRocketMessage<TValue> rocketPackObject) throw new NotSupportedException();
            rocketPackObject.Export(bytesPipe.Writer, bytesPool);

            return await storage.TryWriteAsync(key, bytesPipe.Reader.GetSequence(), cancellationToken);
        }
    }
}
