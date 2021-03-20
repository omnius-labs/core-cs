using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Storages.Extensions
{
    public static class RocketPackObjectStorageExtensions
    {
        public static async IAsyncEnumerable<TValue> GetValuesAsync<TKey, TValue>(this IBytesStorage<TKey> bytesStorage, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            where TKey : notnull, IEquatable<TKey>
            where TValue : IRocketPackObject<TValue>
        {
            var bytesPool = BytesPool.Shared;
            using var hub = new BytesHub(bytesPool);

            await foreach (var key in bytesStorage.GetKeysAsync(cancellationToken))
            {
                if (!await bytesStorage.TryReadAsync(key, hub.Writer, cancellationToken)) continue;

                var value = IRocketPackObject<TValue>.Import(hub.Reader.GetSequence(), bytesPool);
                yield return value;

                hub.Reset();
            }
        }

        public static async ValueTask<TValue?> TryGetValueAsync<TKey, TValue>(this IBytesStorage<TKey> bytesStorage, TKey key, CancellationToken cancellationToken = default)
            where TKey : notnull, IEquatable<TKey>
            where TValue : IRocketPackObject<TValue>
        {
            var bytesPool = BytesPool.Shared;
            using var hub = new BytesHub(bytesPool);

            if (!await bytesStorage.TryReadAsync(key, hub.Writer, cancellationToken)) return default;

            var value = IRocketPackObject<TValue>.Import(hub.Reader.GetSequence(), bytesPool);
            return value;
        }

        public static async ValueTask SetValueAsync<TKey, TValue>(this IBytesStorage<TKey> bytesStorage, TKey key, TValue value, CancellationToken cancellationToken = default)
            where TKey : notnull, IEquatable<TKey>
            where TValue : IRocketPackObject<TValue>
        {
            var bytesPool = BytesPool.Shared;
            using var hub = new BytesHub(bytesPool);

            if (value is not IRocketPackObject<TValue> rocketPackObject) throw new NotSupportedException();
            rocketPackObject.Export(hub.Writer, bytesPool);

            await bytesStorage.WriteAsync(key, hub.Reader.GetSequence(), cancellationToken);
        }
    }
}
