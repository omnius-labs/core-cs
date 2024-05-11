using System.Net;
using System.Net.Sockets;

namespace Core.Base;

public static class SocketExtensions
{
    public static async ValueTask ConnectAsync(this Socket socket, IPEndPoint remoteEndPoint, TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken).ConfigureAwait(false);

        using var linkedCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCancellationTokenSource.CancelAfter(timeout);

        var asyncResult = socket.BeginConnect(remoteEndPoint, null, null);

        try
        {
            if (!asyncResult.IsCompleted && !asyncResult.CompletedSynchronously)
            {
                await asyncResult.AsyncWaitHandle.WaitAsync(linkedCancellationTokenSource.Token);
            }
        }
        finally
        {
            socket.EndConnect(asyncResult);
        }
    }
}
