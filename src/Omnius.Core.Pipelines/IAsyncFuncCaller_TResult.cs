namespace Omnius.Core.Pipelines;

public interface IAsyncFuncCaller<TResult>
{
    IAsyncEnumerable<TResult> CallAsync();
}
