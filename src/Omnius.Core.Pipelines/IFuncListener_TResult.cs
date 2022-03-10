namespace Omnius.Core.Pipelines;

public interface IFuncListener<TResult>
{
    IDisposable Listen(Func<TResult> func);
}
