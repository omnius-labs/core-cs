using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

namespace Omnius.Core.Network
{
    /// <summary>
    /// Omniusのアドレス
    /// </summary>
    public partial class OmniAddress
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static implicit operator string(OmniAddress omniAddress)
        {
            return omniAddress.Value;
        }

        public static implicit operator OmniAddress(string text)
        {
            return new OmniAddress(text);
        }

        public override string ToString()
        {
            return this.Value;
        }

        public FunctionElement Parse()
        {
            return Parser.Parse(this.Value);
        }
    }
}
