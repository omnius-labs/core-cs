using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Connections;

namespace Omnius.Core.RocketPack.Remoting
{
    public sealed partial class RocketRemotingConnector<TError> : IRocketRemotingConnector<TError>
        where TError : IRocketMessage<TError>
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IConnectionConnector _connector;
        private readonly IBytesPool _bytesPool;

        public RocketRemotingConnector(IConnectionConnector connector, IBytesPool bytesPool)
        {
            _connector = connector;
            _bytesPool = bytesPool;
        }

        public async ValueTask<IRocketRemotingCaller<TError>> ConnectAsync(uint functionId, CancellationToken cancellationToken = default)
        {
            var connection = await _connector.ConnectAsync(cancellationToken);

            await connection.Sender.SendAsync(
                bufferWriter =>
                {
                    Varint.SetUInt32(functionId, bufferWriter);
                }, cancellationToken);

            var caller = new RocketRemoting.Caller<TError>(connection, functionId, _bytesPool);
            return caller;
        }
    }
}
