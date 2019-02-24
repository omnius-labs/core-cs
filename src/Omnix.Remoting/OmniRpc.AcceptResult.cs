namespace Omnix.Remoting
{
    partial class OmniRpc
    {
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
