using System.Threading;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Internal
{
    internal sealed class ConnectionSubscribers : DisposableBase, IConnectionSubscribers
    {
        private readonly CancellationTokenRegistration _cancellationTokenRegistration;
        private int _onClosedCalled = 0;
        private readonly ActionPipe _closedPipe = new ActionPipe();

        public ConnectionSubscribers(CancellationToken cancellationToken)
        {
            _cancellationTokenRegistration = cancellationToken.Register(() => this.InternalOnClosed());
        }

        private void InternalOnClosed()
        {
            if (Interlocked.CompareExchange(ref _onClosedCalled, 1, 0) == 0)
            {
                _closedPipe.Publicher.Publish();
            }
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _cancellationTokenRegistration.Dispose();
            }
        }

        public IActionSubscriber OnClosed => _closedPipe.Subscriber;
    }
}
