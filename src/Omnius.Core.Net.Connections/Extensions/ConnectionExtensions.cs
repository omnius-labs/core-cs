using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Net.Connections.Extensions
{
    public static class ConnectionExtensions
    {
        public static bool TryEnqueue<T>(this IConnection connection, T value)
            where T : IRocketPackObject<T>
        {
            return connection.TryEnqueue(bufferWriter => value.Export(bufferWriter, BytesPool.Shared));
        }

        public static async ValueTask EnqueueAsync<T>(this IConnection connection, T value, CancellationToken cancellationToken = default)
            where T : IRocketPackObject<T>
        {
            await connection.EnqueueAsync(bufferWriter => value.Export(bufferWriter, BytesPool.Shared), cancellationToken);
        }

        public static bool TryDequeue<T>(this IConnection connection, [NotNullWhen(true)] out T value)
            where T : IRocketPackObject<T>
        {
            T valueResult = default!;
            var returnResult = connection.TryDequeue(sequence => valueResult = IRocketPackObject<T>.Import(sequence, BytesPool.Shared));

            value = valueResult!;
            return returnResult;
        }

        public static async ValueTask<T> DequeueAsync<T>(this IConnection connection, CancellationToken cancellationToken = default)
            where T : IRocketPackObject<T>
        {
            T valueResult = default!;
            await connection.DequeueAsync(sequence => valueResult = IRocketPackObject<T>.Import(sequence, BytesPool.Shared), cancellationToken);

            return valueResult!;
        }
    }
}
