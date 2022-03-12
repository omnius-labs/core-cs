using System.Reactive.Linq;

namespace Omnius.Core.Avalonia;

public static class IObservableExtensions
{
    // https://stackoverflow.com/questions/67360865/conditional-delaythrottle-operator
    public static IObservable<T> DelayWhen<T>(this IObservable<T> source, TimeSpan delay, Func<T, bool> condition)
    {
        return source
            .Select(x => (Item: x, WithDelay: condition(x)))
            .Publish(published => published
                .SelectMany(t => t.WithDelay
                    ? Observable.Return(t)
                        .Delay(delay)
                        .TakeUntil(published.Where(t2 => !t2.WithDelay))
                    : Observable.Return(t)
                )
            )
            .Select(e => e.Item);
    }
}
