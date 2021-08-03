using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections
{
    public interface IConnectionSubscribers
    {
        IActionSubscriber OnClosed { get; }
    }
}
