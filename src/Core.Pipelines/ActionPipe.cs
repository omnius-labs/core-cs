using Core.Base;

namespace Core.Pipelines;

public sealed class ActionPipe
{
    private event Action Action = () => { };

    public ActionPipe()
    {
        this.Caller = new ActionCaller(this);
        this.Listener = new ActionListener(this);
    }

    public IActionCaller Caller { get; }

    public IActionListener Listener { get; }

    public sealed class ActionCaller : IActionCaller
    {
        private readonly ActionPipe _pipe;

        public ActionCaller(ActionPipe pipe)
        {
            _pipe = pipe;
        }

        public void Call()
        {
            _pipe.Action.Invoke();
        }
    }

    public sealed class ActionListener : IActionListener
    {
        private readonly ActionPipe _pipe;

        public ActionListener(ActionPipe pipe)
        {
            _pipe = pipe;
        }

        public IDisposable Listen(Action action)
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
