namespace Omnius.Core.Base.Pipelines;

public interface IFuncCaller<TResult>
{
    IEnumerable<TResult> Call();
}
