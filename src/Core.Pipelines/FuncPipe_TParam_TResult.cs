using System.Collections.Immutable;
using Core.Base;

namespace Core.Pipelines;

public sealed class FuncPipe<TParam, TResult>
{
    private ImmutableList<Func<TParam, TResult>> _funcs = ImmutableList<Func<TParam, TResult>>.Empty;

    public FuncPipe()
    {
        this.Caller = new FuncCaller(this);
        this.Listener = new FuncListener(this);
    }

    public IFuncCaller<TParam, TResult> Caller { get; }

    public IFuncListener<TParam, TResult> Listener { get; }

    public sealed class FuncCaller : IFuncCaller<TParam, TResult>
    {
        private readonly FuncPipe<TParam, TResult> _pipe;

        public FuncCaller(FuncPipe<TParam, TResult> pipe)
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

    public sealed class FuncListener : IFuncListener<TParam, TResult>
    {
        private readonly FuncPipe<TParam, TResult> _pipe;

        public FuncListener(FuncPipe<TParam, TResult> pipe)
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
