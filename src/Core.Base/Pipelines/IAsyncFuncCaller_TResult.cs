namespace Omnius.Core.Base.Pipelines;

public interface IAsyncFuncCaller<TResult>
{
    IAsyncEnumerable<TResult> CallAsync();
}
