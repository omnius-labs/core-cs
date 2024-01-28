using Avalonia.Threading;

namespace Omnius.Core.Avalonia;

public interface IApplicationDispatcher
{
    public Task InvokeAsync(Action action);
    public Task InvokeAsync(Action action, DispatcherPriority priority);
    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function);
    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function, DispatcherPriority priority);
    public Task InvokeAsync(Func<Task> function);
    public Task InvokeAsync(Func<Task> function, DispatcherPriority priority);
    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function);
    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority);
}

public class ApplicationDispatcher : IApplicationDispatcher
{
    public Task InvokeAsync(Action action) => this.InvokeAsync(action, DispatcherPriority.Normal);

    public async Task InvokeAsync(Action action, DispatcherPriority priority)
    {
        await Dispatcher.UIThread.InvokeAsync(action, priority).GetTask();
    }

    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function) => this.InvokeAsync(function, DispatcherPriority.Normal);

    public async Task<TResult> InvokeAsync<TResult>(Func<TResult> function, DispatcherPriority priority)
    {
        return await Dispatcher.UIThread.InvokeAsync(function, priority).GetTask();
    }

    public Task InvokeAsync(Func<Task> function) => this.InvokeAsync(function, DispatcherPriority.Normal);

    public Task InvokeAsync(Func<Task> function, DispatcherPriority priority)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority);
    }

    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function) => this.InvokeAsync(function, DispatcherPriority.Normal);

    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority);
    }
}
