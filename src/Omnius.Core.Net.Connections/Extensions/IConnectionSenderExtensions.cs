using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Net.Connections;

public static class IConnectionSenderExtensions
{
    public static bool TrySend<T>(this IConnectionSender connectionSender, T value)
        where T : IRocketMessage<T>
    {
        return connectionSender.TrySend(bufferWriter => value.Export(bufferWriter, BytesPool.Shared));
    }

    public static async ValueTask SendAsync<T>(this IConnectionSender connectionSender, T value, CancellationToken cancellationToken = default)
        where T : IRocketMessage<T>
    {
        await connectionSender.SendAsync(bufferWriter => value.Export(bufferWriter, BytesPool.Shared), cancellationToken);
    }
}