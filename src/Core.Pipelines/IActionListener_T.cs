namespace Core.Pipelines;

public interface IActionListener<T>
{
    IDisposable Listen(Action<T> action);
}
