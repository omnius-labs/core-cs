using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Network.Connection;
using Omnix.Serialization;

namespace Omnix.Remoting
{
    public sealed partial class OmniRpc
    {
        private readonly BufferPool _bufferPool;

        public OmniRpc(BufferPool bufferPool)
        {
            _bufferPool = bufferPool;
        }

        public Func<IConnection> GetConnectedConnection { private get; set; }
        public Func<IConnection> GetAcceptedConnection { private get; set; }

        public async ValueTask<OmniRpcStream> Connect(ulong type)
        {
            var connection = this.GetConnectedConnection.Invoke();

            await connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64(type, bufferWriter);
            });

            return new OmniRpcStream(connection, _bufferPool);
        }

        public async ValueTask<AcceptResult> Accept()
        {
            var connection = this.GetAcceptedConnection.Invoke();

            ulong type = 0;

            await connection.DequeueAsync((sequence) =>
            {
                if (!Varint.TryGetUInt64(sequence, out type, out _))
                {
                    throw new FormatException();
                }
            });

            return new AcceptResult(type, new OmniRpcStream(connection, _bufferPool));
        }
    }
}
