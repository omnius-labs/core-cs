using Core.Pipelines;

namespace Core.Net.Connections;

public interface IConnectionEvents
{
    IActionListener OnClosed { get; }
}
