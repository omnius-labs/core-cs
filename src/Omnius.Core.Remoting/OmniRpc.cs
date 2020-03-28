using System;
using System.Buffers;
using System.Threading.Tasks;
using Omnius.Core;
using Omnius.Core.Network.Connections;
using Omnius.Core.Serialization.RocketPack;

namespace Omnius.Core.Remoting
{
    public sealed partial class OmniRpc
    {
        private readonly IBytesPool _bytesPool;

        public OmniRpc(IBytesPool bytesPool)
        {
            _bytesPool = bytesPool;
        }

        public Func<IConnection> GetConnectedConnection { private get; set; } = () => throw new OmniRpcException("callback is not registed");
        public Func<IConnection> GetAcceptedConnection { private get; set; } = () => throw new OmniRpcException("callback is not registed");

        public async ValueTask<OmniRpcStream> Connect(ulong type)
        {
            var connection = this.GetConnectedConnection.Invoke();

            await connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64(type, bufferWriter);
            });

            return new OmniRpcStream(connection, _bytesPool);
        }

        public async ValueTask<OmniRpcAcceptResult> Accept()
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
            });

            return new OmniRpcAcceptResult(type, new OmniRpcStream(connection, _bytesPool));
        }
    }

    public class OmniRpcException : Exception
    {
        public OmniRpcException(string message) : base(message) { }
    }
}
