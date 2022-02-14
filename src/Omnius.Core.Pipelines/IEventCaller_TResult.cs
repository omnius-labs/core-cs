namespace Omnius.Core.Pipelines;

public interface IEventCaller<TResult>
{
    IEnumerable<TResult> Call();
}
