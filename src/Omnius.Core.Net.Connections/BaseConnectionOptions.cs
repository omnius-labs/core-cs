namespace Omnius.Core.Net.Connections
{
    public class BaseConnectionOptions
    {
        public int MaxReceiveByteCount { get; init; } = 1024 * 64;

        public IBytesPool? BytesPool { get; init; }
    }
}
