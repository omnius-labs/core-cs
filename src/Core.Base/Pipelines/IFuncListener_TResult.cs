namespace Omnius.Core.Base.Pipelines;

public interface IFuncListener<TResult>
{
    IDisposable Listen(Func<TResult> func);
}
