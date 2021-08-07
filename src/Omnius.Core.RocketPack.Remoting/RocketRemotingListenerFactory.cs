using System;
using System.Buffers.Binary;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Connections;

namespace Omnius.Core.RocketPack.Remoting
{
    public sealed partial class RocketRemotingListenerFactory<TError> : IRocketRemotingListenerFactory<TError>
        where TError : IRocketMessage<TError>
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly IConnectionAccepter _accepter;
        private readonly IErrorMessageFactory<TError> _errorMessageFactory;
        private readonly IBytesPool _bytesPool;

        public RocketRemotingListenerFactory(IConnectionAccepter accepter, IErrorMessageFactory<TError> errorMessageFactory, IBytesPool bytesPool)
        {
            _accepter = accepter;
            _errorMessageFactory = errorMessageFactory;
            _bytesPool = bytesPool;
        }

        public async ValueTask<IRocketRemotingListener<TError>> CreateAsync(CancellationToken cancellationToken = default)
        {
            uint functionId = 0;

            var connection = await _accepter.AcceptAsync(cancellationToken);

            await connection.Receiver.ReceiveAsync(
                sequence =>
                {
                    Varint.TryGetUInt32(ref sequence, out functionId);
                }, cancellationToken);

            var listener = new RocketRemotingListener<TError>(connection, functionId, _errorMessageFactory, _bytesPool);
            return listener;
        }
    }
}
