using System;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Omnius.Core.Avalonia
{
    public interface IApplicationDispatcher
    {
        public Task InvokeAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal);

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> function, DispatcherPriority priority = DispatcherPriority.Normal);

        public Task InvokeAsync(Func<Task> function, DispatcherPriority priority = DispatcherPriority.Normal);

        public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority = DispatcherPriority.Normal);
    }

    public class ApplicationDispatcher : IApplicationDispatcher
    {
        public Task InvokeAsync(Action action, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            return Dispatcher.UIThread.InvokeAsync(action, priority);
        }

        public Task<TResult> InvokeAsync<TResult>(Func<TResult> function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            return Dispatcher.UIThread.InvokeAsync(function, priority);
        }

        public Task InvokeAsync(Func<Task> function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            return Dispatcher.UIThread.InvokeAsync(function, priority);
        }

        public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            return Dispatcher.UIThread.InvokeAsync(function, priority);
        }
    }
}
