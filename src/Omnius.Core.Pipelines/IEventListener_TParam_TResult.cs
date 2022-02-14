namespace Omnius.Core.Pipelines;

public interface IEventListener<TParam, TResult>
{
    IDisposable Listen(Func<TParam, TResult> func);
}
