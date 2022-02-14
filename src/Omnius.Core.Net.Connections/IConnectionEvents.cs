using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections;

public interface IConnectionEvents
{
    IActionListener OnClosed { get; }
}
