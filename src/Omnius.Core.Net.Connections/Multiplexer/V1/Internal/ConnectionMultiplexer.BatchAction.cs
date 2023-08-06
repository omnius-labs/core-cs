using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal;

internal sealed partial class ConnectionMultiplexer
{
    private sealed class BatchAction : IBatchAction
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly Action<Exception> _exceptionCallback;

        public BatchAction(ConnectionMultiplexer connectionMultiplexer, Action<Exception> exceptionCallback)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _exceptionCallback = exceptionCallback;
        }

        public TimeSpan Interval { get; } = TimeSpan.FromMilliseconds(10);

        public void Execute()
        {
            try
            {
                _connectionMultiplexer.InternalSend();
            }
            catch (Exception e)
            {
                _logger.Debug(e, "Send Exception");
                _exceptionCallback.Invoke(e);
            }

            try
            {
                _connectionMultiplexer.Receive();
            }
            catch (Exception e)
            {
                _logger.Debug(e, "Receive Exception");
                _exceptionCallback.Invoke(e);
            }
        }
    }
}
