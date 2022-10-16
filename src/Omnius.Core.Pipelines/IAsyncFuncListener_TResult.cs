namespace Omnius.Core.Pipelines;

public interface IAsyncFuncListener<TResult>
{
    IDisposable Listen(Func<ValueTask<TResult>> func);
}
