namespace Omnius.Core.Pipelines;

public sealed class ActionPipe<T>
{
    private event Action<T> Action = (_) => { };

    public ActionPipe()
    {
        this.Publicher = new ActionPublicher(this);
        this.Subscriber = new ActionSubscriber(this);
    }

    public IActionPublicher<T> Publicher { get; }

    public IActionSubscriber<T> Subscriber { get; }

    public sealed class ActionPublicher : IActionPublicher<T>
    {
        private readonly ActionPipe<T> _pipe;

        public ActionPublicher(ActionPipe<T> pipe)
        {
            _pipe = pipe;
        }

        public void Publish(T item)
        {
            _pipe.Action.Invoke(item);
        }
    }

    public sealed class ActionSubscriber : IActionSubscriber<T>
    {
        private readonly ActionPipe<T> _pipe;

        public ActionSubscriber(ActionPipe<T> pipe)
        {
            _pipe = pipe;
        }

        public IDisposable Subscribe(Action<T> action)
        {
            return new Cookie(_pipe, action);
        }

        private sealed class Cookie : DisposableBase, IDisposable
        {
            private readonly ActionPipe<T> _pipe;
            private readonly Action<T> _action;

            public Cookie(ActionPipe<T> pipe, Action<T> action)
            {
                _pipe = pipe;
                _action = action;

                _pipe.Action += _action;
            }

            protected override void OnDispose(bool disposing)
            {
                _pipe.Action -= _action;
            }
        }
    }
}