namespace Omnius.Core.RocketPack.Remoting
{
    public readonly struct RocketPackRpcAcceptResult
    {
        public RocketPackRpcAcceptResult(ulong type, RocketPackRpcStream stream)
        {
            this.Type = type;
            this.Stream = stream;
        }

        public ulong Type { get; }
        public RocketPackRpcStream Stream { get; }
    }
}
