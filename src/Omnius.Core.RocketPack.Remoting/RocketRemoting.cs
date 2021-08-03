using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Omnius.Core.Net.Connections;

namespace Omnius.Core.RocketPack.Remoting
{
    public sealed partial class RocketRemoting
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IBytesPool _bytesPool;

        public RocketRemoting(IBytesPool bytesPool)
        {
            _bytesPool = bytesPool;
        }

        public async ValueTask<IRocketRemotingCaller> Connect(IConnection connection, int functionId, CancellationToken cancellationToken = default)
        {
            await connection.Sender.SendAsync(
                bufferWriter =>
                {
                    BinaryPrimitives.WriteInt32BigEndian(bufferWriter.GetSpan(4), functionId);
                    bufferWriter.Advance(4);
                }, cancellationToken);

            var caller = new Caller(connection, functionId, _bytesPool);
            return caller;
        }

        public async ValueTask<IRocketRemotingListener> Accept(IConnection connection, CancellationToken cancellationToken = default)
        {
            int functionId = 0;

            await connection.Receiver.ReceiveAsync(
                sequence =>
                {
                    Span<byte> buffer = stackalloc byte[4];
                    functionId = BinaryPrimitives.ReadInt32BigEndian(buffer);
                }, cancellationToken);

            var listener = new Listener(connection, functionId, _bytesPool);
            return listener;
        }
    }
}
