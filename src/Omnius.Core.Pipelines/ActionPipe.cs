namespace Omnius.Core.Pipelines;

public sealed class ActionPipe
{
    private event Action Action = () => { };

    public ActionPipe()
    {
        this.Publicher = new ActionPublicher(this);
        this.Subscriber = new ActionSubscriber(this);
    }

    public IActionPublicher Publicher { get; }

    public IActionSubscriber Subscriber { get; }

    public sealed class ActionPublicher : IActionPublicher
    {
        private readonly ActionPipe _pipe;

        public ActionPublicher(ActionPipe pipe)
        {
            _pipe = pipe;
        }

        public void Publish()
        {
            _pipe.Action.Invoke();
        }
    }

    public sealed class ActionSubscriber : IActionSubscriber
    {
        private readonly ActionPipe _pipe;

        public ActionSubscriber(ActionPipe pipe)
        {
            _pipe = pipe;
        }

        public IDisposable Subscribe(Action action)
        {
            return new Cookie(_pipe, action);
        }

        private sealed class Cookie : DisposableBase, IDisposable
        {
            private readonly ActionPipe _pipe;
            private readonly Action _action;

            public Cookie(ActionPipe pipe, Action action)
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