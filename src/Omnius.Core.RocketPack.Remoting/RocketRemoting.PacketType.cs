using System;

namespace Omnius.Core.RocketPack.Remoting
{
    public partial class RocketRemoting
    {
        [Flags]
        internal enum PacketType : byte
        {
            Unknown = 0,
            Completed = 1,
            Continue = 2,
            Error = 3,
        }
    }
}
