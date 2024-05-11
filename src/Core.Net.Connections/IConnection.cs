namespace Core.Net.Connections;

public interface IConnection : IAsyncDisposable
{
    bool IsConnected { get; }

    IConnectionSender Sender { get; }
    IConnectionReceiver Receiver { get; }
    IConnectionEvents Events { get; }
}
