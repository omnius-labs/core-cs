namespace Omnius.Core.Pipelines;

public interface IActionCaller<T>
{
    void Call(T item);
}
