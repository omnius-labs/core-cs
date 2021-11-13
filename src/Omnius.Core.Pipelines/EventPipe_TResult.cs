using System.Collections.Immutable;

namespace Omnius.Core.Pipelines;

public sealed class EventPipe<TResult>
{
    private ImmutableList<Func<TResult>> _funcs = ImmutableList<Func<TResult>>.Empty;

    public EventPipe()
    {
        this.Publicher = new EventPublicher(this);
        this.Subscriber = new EventSubscriber(this);
    }

    public IEventPublicher<TResult> Publicher { get; }

    public IEventSubscriber<TResult> Subscriber { get; }

    public sealed class EventPublicher : IEventPublicher<TResult>
    {
        private readonly EventPipe<TResult> _pipe;

        public EventPublicher(EventPipe<TResult> pipe)
        {
            _pipe = pipe;
        }

        public IEnumerable<TResult> Publish()
        {
            foreach (var func in _pipe._funcs)
            {
                yield return func();
            }
        }
    }

    public sealed class EventSubscriber : IEventSubscriber<TResult>
    {
        private readonly EventPipe<TResult> _pipe;

        public EventSubscriber(EventPipe<TResult> pipe)
        {
            _pipe = pipe;
        }

        public IDisposable Subscribe(Func<TResult> func)
        {
            return new Cookie(_pipe, func);
        }

        private sealed class Cookie : DisposableBase, IDisposable
        {
            private readonly EventPipe<TResult> _pipe;
            private readonly Func<TResult> _func;

            public Cookie(EventPipe<TResult> pipe, Func<TResult> func)
            {
                _pipe = pipe;
                _func = func;

                _pipe._funcs = _pipe._funcs.Add(func);
            }

            protected override void OnDispose(bool disposing)
            {
                _pipe._funcs = _pipe._funcs.Remove(_func);
            }
        }
    }
}