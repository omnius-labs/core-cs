using System.Collections.Generic;
using Omnius.Core.Tasks;

namespace Omnius.Core.Net.Connections.Secure
{
    public record OmniSecureConnectionOptions
    (
        int MaxReceiveByteCount,
        OmniSecureConnectionType Type,
        IReadOnlyList<string>? Passwords,
        IBatchActionDispatcher BatchActionDispatcher,
        IBytesPool BytesPool
    );
}
