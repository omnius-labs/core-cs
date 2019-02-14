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
    public sealed class OmniRpcClient
    {
        public Func<IConnection> CreateConnection { private get; set; }

        public async ValueTask<OmniRpcStream> Connect(ulong type)
        {
            var connection = this.CreateConnection.Invoke();

            await connection.EnqueueAsync((bufferWriter) =>
            {
                Varint.SetUInt64(type, bufferWriter);
            });

            return new OmniRpcStream(connection);
        }
    }
}
