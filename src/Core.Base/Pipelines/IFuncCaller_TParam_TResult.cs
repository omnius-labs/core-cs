namespace Omnius.Core.Base.Pipelines;

public interface IFuncCaller<TParam, TResult>
{
    IEnumerable<TResult> Call(TParam param);
}
