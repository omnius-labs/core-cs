using System;

namespace Omnius.Core.Pipelines;

public interface IEventSubscriber<TResult>
{
    IDisposable Subscribe(Func<TResult> func);
}