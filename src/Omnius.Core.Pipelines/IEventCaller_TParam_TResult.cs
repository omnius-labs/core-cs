namespace Omnius.Core.Pipelines;

public interface IEventCaller<TParam, TResult>
{
    IEnumerable<TResult> Call(TParam param);
}
