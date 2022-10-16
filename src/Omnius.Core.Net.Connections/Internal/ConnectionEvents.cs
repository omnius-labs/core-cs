using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Internal;

internal sealed class ConnectionEvents : DisposableBase, IConnectionEvents
{
    private readonly CancellationTokenRegistration _cancellationTokenRegistration;
    private int _onClosedCalled = 0;
    private readonly ActionPipe _closedActionPipe = new();

    public ConnectionEvents(CancellationToken cancellationToken)
    {
        _cancellationTokenRegistration = cancellationToken.Register(() => this.InternalOnClosed());
    }

    private void InternalOnClosed()
    {
        if (Interlocked.CompareExchange(ref _onClosedCalled, 1, 0) == 0)
        {
            _closedActionPipe.Caller.Call();
        }
    }

    protected override void OnDispose(bool disposing)
    {
        if (disposing)
        {
            _cancellationTokenRegistration.Dispose();
        }
    }

    public IActionListener OnClosed => _closedActionPipe.Listener;
}
