namespace Omnius.Core.Base.Pipelines;

public interface IActionListener<T>
{
    IDisposable Listen(Action<T> action);
}
