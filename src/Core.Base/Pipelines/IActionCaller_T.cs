namespace Omnius.Core.Base.Pipelines;

public interface IActionCaller<T>
{
    void Call(T item);
}
