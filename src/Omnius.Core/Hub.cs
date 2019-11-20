using System.IO.Pipelines;

namespace Omnius.Core
{
    public sealed partial class Hub : DisposableBase
    {
        private readonly Pipe _pipe;
        private readonly HubReader _hubReader;
        private readonly HubWriter _hubWriter;

        public Hub()
            : this(new Pipe())
        {

        }

        public Hub(Pipe pipe)
        {
            _pipe = pipe;
            _hubReader = new HubReader(_pipe);
            _hubWriter = new HubWriter(_pipe);
        }

        public HubReader Reader => _hubReader;
        public HubWriter Writer => _hubWriter;

        public void Reset()
        {
            if (!_hubReader.IsCompleted)
            {
                _hubReader.Complete();
            }

            if (!_hubWriter.IsCompleted)
            {
                _hubWriter.Complete();
            }

            _pipe.Reset();

            _hubReader.Reset();
            _hubWriter.Reset();
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                this.Reset();
            }
        }
    }
}
