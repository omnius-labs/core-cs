namespace Omnius.Core.Base.Pipelines;

public interface IAsyncFuncListener<TResult>
{
    IDisposable Listen(Func<ValueTask<TResult>> func);
}
