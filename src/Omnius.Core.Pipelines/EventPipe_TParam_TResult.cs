using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Omnius.Core.Pipelines
{
    public sealed class EventPipe<TParam, TResult>
    {
        private ImmutableList<Func<TParam, TResult>> _funcs = ImmutableList<Func<TParam, TResult>>.Empty;

        public EventPipe()
        {
            this.Publicher = new EventPublicher(this);
            this.Subscriber = new EventSubscriber(this);
        }

        public IEventPublicher<TParam, TResult> Publicher { get; }

        public IEventSubscriber<TParam, TResult> Subscriber { get; }

        public sealed class EventPublicher : IEventPublicher<TParam, TResult>
        {
            private readonly EventPipe<TParam, TResult> _pipe;

            public EventPublicher(EventPipe<TParam, TResult> pipe)
            {
                _pipe = pipe;
            }

            public IEnumerable<TResult> Publish(TParam param)
            {
                foreach (var func in _pipe._funcs)
                {
                    yield return func(param);
                }
            }
        }

        public sealed class EventSubscriber : IEventSubscriber<TParam, TResult>
        {
            private readonly EventPipe<TParam, TResult> _pipe;

            public EventSubscriber(EventPipe<TParam, TResult> pipe)
            {
                _pipe = pipe;
            }

            public IDisposable Subscribe(Func<TParam, TResult> func)
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
}
