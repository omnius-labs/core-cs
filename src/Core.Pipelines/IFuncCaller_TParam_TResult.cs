namespace Core.Pipelines;

public interface IFuncCaller<TParam, TResult>
{
    IEnumerable<TResult> Call(TParam param);
}
