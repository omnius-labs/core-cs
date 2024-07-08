namespace Omnius.Core.Pipelines;

public interface IActionListener
{
    IDisposable Listen(Action action);
}
