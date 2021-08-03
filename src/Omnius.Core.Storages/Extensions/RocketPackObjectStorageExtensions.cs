using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Pipelines;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Storages
{
    public static class RocketMessageStorageExtensions
    {
        public static async IAsyncEnumerable<TValue> GetValuesAsync<TKey, TValue>(this IBytesStorage<TKey> bytesStorage, [EnumeratorCancellation] CancellationToken cancellationToken = default)
            where TKey : notnull, IEquatable<TKey>
            where TValue : IRocketMessage<TValue>
        {
            var bytesPool = BytesPool.Shared;
            using var bytesPipe = new BytesPipe(bytesPool);

            await foreach (var key in bytesStorage.GetKeysAsync(cancellationToken))
            {
                if (!await bytesStorage.TryReadAsync(key, bytesPipe.Writer, cancellationToken)) continue;

                var value = IRocketMessage<TValue>.Import(bytesPipe.Reader.GetSequence(), bytesPool);
                yield return value;

                bytesPipe.Reset();
            }
        }

        public static async ValueTask<TValue?> TryGetValueAsync<TKey, TValue>(this IBytesStorage<TKey> bytesStorage, TKey key, CancellationToken cancellationToken = default)
            where TKey : notnull, IEquatable<TKey>
            where TValue : IRocketMessage<TValue>
        {
            var bytesPool = BytesPool.Shared;
            using var bytesPipe = new BytesPipe(bytesPool);

            if (!await bytesStorage.TryReadAsync(key, bytesPipe.Writer, cancellationToken)) return default;

            var value = IRocketMessage<TValue>.Import(bytesPipe.Reader.GetSequence(), bytesPool);
            return value;
        }

        public static async ValueTask SetValueAsync<TKey, TValue>(this IBytesStorage<TKey> bytesStorage, TKey key, TValue value, CancellationToken cancellationToken = default)
            where TKey : notnull, IEquatable<TKey>
            where TValue : IRocketMessage<TValue>
        {
            var bytesPool = BytesPool.Shared;
            using var bytesPipe = new BytesPipe(bytesPool);

            if (value is not IRocketMessage<TValue> rocketPackObject) throw new NotSupportedException();
            rocketPackObject.Export(bytesPipe.Writer, bytesPool);

            await bytesStorage.WriteAsync(key, bytesPipe.Reader.GetSequence(), cancellationToken);
        }
    }
}
