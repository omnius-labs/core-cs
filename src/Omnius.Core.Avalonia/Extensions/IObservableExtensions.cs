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
                .Timestamp()
                .Scan((delayOverTime: DateTimeOffset.MinValue, output: Observable.Empty<T>()), (state, t) =>
                {
                    if (!t.Value.WithDelay)
                    {
                        //value isn't delayed, current delay status irrelevant, emit immediately, and cancel previous delay.
                        return (DateTimeOffset.MinValue, Observable.Return(t.Value.Item));
                    }
                    else if (state.delayOverTime > t.Timestamp)
                    {
                        //value should be delayed, but current delay already in progress. Ignore value.
                        return (state.delayOverTime, Observable.Empty<T>());
                    }
                    else
                    {
                        //value should be delayed, no delay in progress. Set delay state, and return delayed observable.
                        return (t.Timestamp + delay, Observable.Return(t.Value.Item).Delay(delay).TakeUntil(published.Where(t2 => !t2.WithDelay)));
                    }
                })
            )
            .SelectMany(t => t.output);
    }
}
