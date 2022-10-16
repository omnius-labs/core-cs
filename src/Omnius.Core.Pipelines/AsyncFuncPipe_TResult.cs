using System.Collections.Immutable;

namespace Omnius.Core.Pipelines;

public sealed class AsyncFuncPipe<TResult>
{
    private ImmutableList<Func<ValueTask<TResult>>> _funcs = ImmutableList<Func<ValueTask<TResult>>>.Empty;

    public AsyncFuncPipe()
    {
        this.Caller = new AsyncFuncCaller(this);
        this.Listener = new AsyncFuncListener(this);
    }

    public IAsyncFuncCaller<TResult> Caller { get; }

    public IAsyncFuncListener<TResult> Listener { get; }

    public sealed class AsyncFuncCaller : IAsyncFuncCaller<TResult>
    {
        private readonly AsyncFuncPipe<TResult> _pipe;

        public AsyncFuncCaller(AsyncFuncPipe<TResult> pipe)
        {
            _pipe = pipe;
        }

        public async IAsyncEnumerable<TResult> CallAsync()
        {
            foreach (var func in _pipe._funcs)
            {
                yield return await func();
            }
        }
    }

    public sealed class AsyncFuncListener : IAsyncFuncListener<TResult>
    {
        private readonly AsyncFuncPipe<TResult> _pipe;

        public AsyncFuncListener(AsyncFuncPipe<TResult> pipe)
        {
            _pipe = pipe;
        }

        public IDisposable Listen(Func<ValueTask<TResult>> func)
        {
            return new Cookie(_pipe, func);
        }

        private sealed class Cookie : DisposableBase, IDisposable
        {
            private readonly AsyncFuncPipe<TResult> _pipe;
            private readonly Func<ValueTask<TResult>> _func;

            public Cookie(AsyncFuncPipe<TResult> pipe, Func<ValueTask<TResult>> func)
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
