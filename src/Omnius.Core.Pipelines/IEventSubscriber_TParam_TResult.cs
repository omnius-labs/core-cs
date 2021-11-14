namespace Omnius.Core.Pipelines;

public interface IEventSubscriber<TParam, TResult>
{
    IDisposable Subscribe(Func<TParam, TResult> func);
}
