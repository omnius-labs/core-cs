using System.Collections.Immutable;

namespace Omnius.Core.Pipelines;

public sealed class FuncPipe<TResult>
{
    private ImmutableList<Func<TResult>> _funcs = ImmutableList<Func<TResult>>.Empty;

    public FuncPipe()
    {
        this.Caller = new EventCaller(this);
        this.Listener = new EventListener(this);
    }

    public IFuncCaller<TResult> Caller { get; }

    public IFuncListener<TResult> Listener { get; }

    public sealed class EventCaller : IFuncCaller<TResult>
    {
        private readonly FuncPipe<TResult> _pipe;

        public EventCaller(FuncPipe<TResult> pipe)
        {
            _pipe = pipe;
        }

        public IEnumerable<TResult> Call()
        {
            foreach (var func in _pipe._funcs)
            {
                yield return func();
            }
        }
    }

    public sealed class EventListener : IFuncListener<TResult>
    {
        private readonly FuncPipe<TResult> _pipe;

        public EventListener(FuncPipe<TResult> pipe)
        {
            _pipe = pipe;
        }

        public IDisposable Listen(Func<TResult> func)
        {
            return new Cookie(_pipe, func);
        }

        private sealed class Cookie : DisposableBase, IDisposable
        {
            private readonly FuncPipe<TResult> _pipe;
            private readonly Func<TResult> _func;

            public Cookie(FuncPipe<TResult> pipe, Func<TResult> func)
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
