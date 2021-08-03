using System;

namespace Omnius.Core.Pipelines
{
    public interface IActionSubscriber<T>
    {
        IDisposable Subscribe(Action<T> action);
    }
}
