namespace Omnius.Core.Pipelines;

public interface IEventPublicher<TParam, TResult>
{
    IEnumerable<TResult> Publish(TParam param);
}