using System.Threading;
using System;
using System.Buffers;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Network.Connections;
using Omnius.Core.RocketPack;

namespace Omnius.Core.RocketPack.Remoting
{
    public sealed partial class RocketPackRpc
    {
        private readonly IBytesPool _bytesPool;

        public RocketPackRpc(IBytesPool bytesPool)
        {
            _bytesPool = bytesPool;
        }

        public Func<IConnection> GetConnectedConnection { private get; set; } = () => throw new NullReferenceException("callback is not registed");
        public Func<IConnection> GetAcceptedConnection { private get; set; } = () => throw new NullReferenceException("callback is not registed");

        public async ValueTask<RocketPackRpcStream> ConnectAsync(ulong type, CancellationToken cancellationToken = default)
        {
            var connection = this.GetConnectedConnection.Invoke();

            await connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64(type, bufferWriter);
            }, cancellationToken);

            return new RocketPackRpcStream(connection, _bytesPool);
        }

        public async ValueTask<RocketPackRpcAcceptResult> AcceptAsync(CancellationToken cancellationToken = default)
        {
            var connection = this.GetAcceptedConnection.Invoke();

            ulong type = 0;

            await connection.DequeueAsync((sequence) =>
            {
                var reader = new SequenceReader<byte>(sequence);

                if (!Varint.TryGetUInt64(ref reader, out type))
                {
                    throw new FormatException();
                }
            }, cancellationToken);

            return new RocketPackRpcAcceptResult(type, new RocketPackRpcStream(connection, _bytesPool));
        }
    }
}
