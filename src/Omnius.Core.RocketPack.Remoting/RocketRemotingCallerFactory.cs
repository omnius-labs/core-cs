using Omnius.Core.Net.Connections;

namespace Omnius.Core.RocketPack.Remoting;

public sealed partial class RocketRemotingCallerFactory<TError> : IRocketRemotingCallerFactory<TError>
    where TError : IRocketMessage<TError>
{
    private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly IConnectionMultiplexer _multiplexer;
    private readonly IBytesPool _bytesPool;

    public RocketRemotingCallerFactory(IConnectionMultiplexer multiplexer, IBytesPool bytesPool)
    {
        _multiplexer = multiplexer;
        _bytesPool = bytesPool;
    }

    public async ValueTask<IRocketRemotingCaller<TError>> CreateAsync(uint functionId, CancellationToken cancellationToken = default)
    {
        var connection = await _multiplexer.ConnectAsync(cancellationToken);

        await connection.Sender.SendAsync(
            bufferWriter =>
            {
                Varint.SetUInt32(functionId, bufferWriter);
            }, cancellationToken);

        var caller = new RocketRemotingCaller<TError>(connection, functionId, _bytesPool);
        return caller;
    }
}
