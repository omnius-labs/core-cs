namespace Omnius.Core.Pipelines;

public interface IActionPublicher<T>
{
    void Publish(T item);
}