using System.Buffers;
using System.IO.Pipelines;

namespace Omnius.Core
{
    public sealed partial class Hub : DisposableBase
    {
        private readonly BufferWriter _bufferWriter;
        private readonly HubReader _hubReader;
        private readonly HubWriter _hubWriter;

        public Hub(IBytesPool bytesPool)
        {
            _bufferWriter = new BufferWriter(bytesPool);
            _hubReader = new HubReader(_bufferWriter);
            _hubWriter = new HubWriter(_bufferWriter);
        }

        public HubReader Reader => _hubReader;
        public HubWriter Writer => _hubWriter;

        public void Reset()
        {
            _bufferWriter.Reset();
            _hubReader.Reset();
            _hubWriter.Reset();
        }

        protected override void OnDispose(bool disposing)
        {
            if (disposing)
            {
                _bufferWriter.Dispose();
            }
        }
    }
}
