namespace Omnius.Core.Pipelines;

public interface IEventListener<TResult>
{
    IDisposable Listen(Func<TResult> func);
}
