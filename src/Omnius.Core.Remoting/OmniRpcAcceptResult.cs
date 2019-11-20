namespace Omnius.Core.Remoting
{
    public readonly struct OmniRpcAcceptResult
    {
        public OmniRpcAcceptResult(ulong type, OmniRpcStream stream)
        {
            this.Type = type;
            this.Stream = stream;
        }

        public ulong Type { get; }
        public OmniRpcStream Stream { get; }
    }
}

