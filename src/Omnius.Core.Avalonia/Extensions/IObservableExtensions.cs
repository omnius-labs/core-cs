using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Omnius.Core.Avalonia;

public static class IObservableExtensions
{
    // https://stackoverflow.com/questions/67360865/conditional-delaythrottle-operator
    public static IObservable<T> DelayWhen<T>(this IObservable<T> source, TimeSpan delay, Func<T, bool> condition, IScheduler? scheduler = null)
    {
        // Arguments validation omitted
        scheduler ??= DefaultScheduler.Instance;
        return source
            .Select(x => (Item: x, WithDelay: condition(x)))
            .Publish(published => published.Window(published.Where(e => !e.WithDelay)))
            .Select(w => Observable.Merge(
                DelayThrottleSpecial(w.Where(e => e.WithDelay), delay, scheduler),
                w.Where(e => !e.WithDelay)
            ))
            .Switch()
            .Select(e => e.Item);
    }

    /// <summary>
    /// Time shifts the observable sequence by the specified time duration, ignoring
    /// elements that are produced while a previous element is scheduled for emission.
    /// </summary>
    private static IObservable<T> DelayThrottleSpecial<T>(IObservable<T> source, TimeSpan dueTime, IScheduler scheduler)
    {
        int mutex = 0; // 0: not acquired, 1: acquired
        return source.SelectMany(x =>
        {
            if (Interlocked.CompareExchange(ref mutex, 1, 0) == 0)
                return Observable.Return(x)
                    .DelaySubscription(dueTime, scheduler)
                    .Finally(() => Volatile.Write(ref mutex, 0));
            return Observable.Empty<T>();
        });
    }
}
