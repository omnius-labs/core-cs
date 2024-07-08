namespace Omnius.Core.Pipelines;

public interface IFuncCaller<TResult>
{
    IEnumerable<TResult> Call();
}
