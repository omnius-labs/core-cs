namespace Omnius.Core.Base.Pipelines;

public interface IFuncListener<TParam, TResult>
{
    IDisposable Listen(Func<TParam, TResult> func);
}
