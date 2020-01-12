using System;

namespace Omnius.Core.Remoting.Internal
{
    [Flags]
    internal enum OmniRpcStreamReceiveResultFlags : byte
    {
        None = 0x0,
        Canceled = 0x1,
        Completed = 0x2,
        Error = 0x4,
    }
}
