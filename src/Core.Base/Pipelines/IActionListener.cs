namespace Omnius.Core.Base.Pipelines;

public interface IActionListener
{
    IDisposable Listen(Action action);
}
