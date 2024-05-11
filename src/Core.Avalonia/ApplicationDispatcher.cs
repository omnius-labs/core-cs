using Avalonia.Threading;

namespace Core.Avalonia;

public interface IApplicationDispatcher
{
    public Task InvokeAsync(Action action);
    public Task InvokeAsync(Action action, DispatcherPriority priority);
    public Task InvokeAsync(Action action, DispatcherPriority priority, CancellationToken cancellationToken = default);
    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function);
    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function, DispatcherPriority priority);
    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function, DispatcherPriority priority, CancellationToken cancellationToken = default);
    public Task InvokeAsync(Func<Task> function);
    public Task InvokeAsync(Func<Task> function, DispatcherPriority priority);
    public Task InvokeAsync(Func<Task> function, DispatcherPriority priority, CancellationToken cancellationToken = default);
    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function);
    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority);
    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority, CancellationToken cancellationToken = default);
}

public class ApplicationDispatcher : IApplicationDispatcher
{
    public Task InvokeAsync(Action action)
    {
        return Dispatcher.UIThread.InvokeAsync(action, DispatcherPriority.Normal).GetTask();
    }

    public Task InvokeAsync(Action action, DispatcherPriority priority)
    {
        return Dispatcher.UIThread.InvokeAsync(action, priority).GetTask();
    }

    public Task InvokeAsync(Action action, DispatcherPriority priority, CancellationToken cancellationToken = default)
    {
        return Dispatcher.UIThread.InvokeAsync(action, priority, cancellationToken).GetTask();
    }

    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function)
    {
        return Dispatcher.UIThread.InvokeAsync(function, DispatcherPriority.Normal).GetTask();
    }

    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function, DispatcherPriority priority)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority).GetTask();
    }

    public Task<TResult> InvokeAsync<TResult>(Func<TResult> function, DispatcherPriority priority, CancellationToken cancellationToken = default)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority, cancellationToken).GetTask();
    }

    public Task InvokeAsync(Func<Task> function)
    {
        return Dispatcher.UIThread.InvokeAsync(function, DispatcherPriority.Normal);
    }

    public Task InvokeAsync(Func<Task> function, DispatcherPriority priority)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority);
    }

    public Task InvokeAsync(Func<Task> function, DispatcherPriority priority, CancellationToken cancellationToken = default)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority, cancellationToken).GetTask();
    }

    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function)
    {
        return Dispatcher.UIThread.InvokeAsync(function, DispatcherPriority.Normal);
    }

    public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority)
    {
        return Dispatcher.UIThread.InvokeAsync(function, priority);
    }

    public async Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> function, DispatcherPriority priority, CancellationToken cancellationToken = default)
    {
        var task = await Dispatcher.UIThread.InvokeAsync(function, priority, cancellationToken);
        return await task;
    }
}
