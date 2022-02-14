using System.Collections.Immutable;

namespace Omnius.Core.Pipelines;

public sealed class EventPipe<TParam, TResult>
{
    private ImmutableList<Func<TParam, TResult>> _funcs = ImmutableList<Func<TParam, TResult>>.Empty;

    public EventPipe()
    {
        this.Caller = new EventCaller(this);
        this.Listener = new EventListener(this);
    }

    public IEventCaller<TParam, TResult> Caller { get; }

    public IEventListener<TParam, TResult> Listener { get; }

    public sealed class EventCaller : IEventCaller<TParam, TResult>
    {
        private readonly EventPipe<TParam, TResult> _pipe;

        public EventCaller(EventPipe<TParam, TResult> pipe)
        {
            _pipe = pipe;
        }

        public IEnumerable<TResult> Call(TParam param)
        {
            foreach (var func in _pipe._funcs)
            {
                yield return func(param);
            }
        }
    }

    public sealed class EventListener : IEventListener<TParam, TResult>
    {
        private readonly EventPipe<TParam, TResult> _pipe;

        public EventListener(EventPipe<TParam, TResult> pipe)
        {
            _pipe = pipe;
        }

        public IDisposable Listen(Func<TParam, TResult> func)
        {
            return new Cookie(_pipe, func);
        }

        private sealed class Cookie : DisposableBase, IDisposable
        {
            private readonly EventPipe<TParam, TResult> _pipe;
            private readonly Func<TParam, TResult> _func;

            public Cookie(EventPipe<TParam, TResult> pipe, Func<TParam, TResult> func)
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
