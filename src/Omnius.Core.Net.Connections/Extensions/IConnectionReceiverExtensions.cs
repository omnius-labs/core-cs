using System.Diagnostics.CodeAnalysis;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Net.Connections;

public static class IConnectionReceiverExtensions
{
    public static bool TryReceive<T>(this IConnectionReceiver connectionReceiver, [NotNullWhen(true)] out T value)
        where T : IRocketMessage<T>
    {
        T valueResult = default!;
        var returnResult = connectionReceiver.TryReceive(sequence => valueResult = IRocketMessage<T>.Import(sequence, BytesPool.Shared));

        value = valueResult!;
        return returnResult;
    }

    public static async ValueTask<T> ReceiveAsync<T>(this IConnectionReceiver connectionReceiver, CancellationToken cancellationToken = default)
        where T : IRocketMessage<T>
    {
        T valueResult = default!;
        await connectionReceiver.ReceiveAsync(sequence => valueResult = IRocketMessage<T>.Import(sequence, BytesPool.Shared), cancellationToken);

        return valueResult!;
    }
}