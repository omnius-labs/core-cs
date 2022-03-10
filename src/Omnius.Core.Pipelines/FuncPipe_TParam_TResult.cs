using System.Collections.Immutable;

namespace Omnius.Core.Pipelines;

public sealed class FuncPipe<TParam, TResult>
{
    private ImmutableList<Func<TParam, TResult>> _funcs = ImmutableList<Func<TParam, TResult>>.Empty;

    public FuncPipe()
    {
        this.Caller = new EventCaller(this);
        this.Listener = new EventListener(this);
    }

    public IFuncCaller<TParam, TResult> Caller { get; }

    public IFuncListener<TParam, TResult> Listener { get; }

    public sealed class EventCaller : IFuncCaller<TParam, TResult>
    {
        private readonly FuncPipe<TParam, TResult> _pipe;

        public EventCaller(FuncPipe<TParam, TResult> pipe)
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

    public sealed class EventListener : IFuncListener<TParam, TResult>
    {
        private readonly FuncPipe<TParam, TResult> _pipe;

        public EventListener(FuncPipe<TParam, TResult> pipe)
        {
            _pipe = pipe;
        }

        public IDisposable Listen(Func<TParam, TResult> func)
        {
            return new Cookie(_pipe, func);
        }

        private sealed class Cookie : DisposableBase, IDisposable
        {
            private readonly FuncPipe<TParam, TResult> _pipe;
            private readonly Func<TParam, TResult> _func;

            public Cookie(FuncPipe<TParam, TResult> pipe, Func<TParam, TResult> func)
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
