namespace Omnius.Core.Pipelines;

public sealed class ActionPipe<T>
{
    private event Action<T> Action = (_) => { };

    public ActionPipe()
    {
        this.Caller = new ActionCaller(this);
        this.Listener = new ActionListener(this);
    }

    public IActionCaller<T> Caller { get; }

    public IActionListener<T> Listener { get; }

    public sealed class ActionCaller : IActionCaller<T>
    {
        private readonly ActionPipe<T> _pipe;

        public ActionCaller(ActionPipe<T> pipe)
        {
            _pipe = pipe;
        }

        public void Call(T item)
        {
            _pipe.Action.Invoke(item);
        }
    }

    public sealed class ActionListener : IActionListener<T>
    {
        private readonly ActionPipe<T> _pipe;

        public ActionListener(ActionPipe<T> pipe)
        {
            _pipe = pipe;
        }

        public IDisposable Listen(Action<T> action)
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
