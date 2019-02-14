using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Omnix.Network.Connection;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;

namespace Omnix.Remoting
{
    public sealed class OmniRpcServer
    {
        public Func<IConnection> CreateConnection { private get; set; }

        public async ValueTask<AcceptResult> Accept()
        {
            var connection = this.CreateConnection.Invoke();

            ulong type = 0;

            await connection.DequeueAsync((sequence) =>
            {
                if (!Varint.TryGetUInt64(sequence, out type, out _))
                {
                    throw new FormatException();
                }
            });

            return new AcceptResult(type, new OmniRpcStream(connection));
        }

        public readonly struct AcceptResult
        {
            public AcceptResult(ulong type, OmniRpcStream stream)
            {
                this.Type = type;
                this.Stream = stream;
            }

            public ulong Type { get; }
            public OmniRpcStream Stream { get; }
        }
    }
}
