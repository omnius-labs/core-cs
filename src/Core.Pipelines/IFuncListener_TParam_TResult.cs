namespace Omnius.Core.Pipelines;

public interface IFuncListener<TParam, TResult>
{
    IDisposable Listen(Func<TParam, TResult> func);
}
