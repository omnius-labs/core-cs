using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;

namespace Omnix.Base
{
    public sealed partial class Hub : DisposableBase
    {
        private Pipe _pipe;
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Reset();
            }
        }
    }
}
