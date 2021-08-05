using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Connections;

namespace Omnius.Core.RocketPack.Remoting
{
    public interface IRocketRemotingConnector<TError>
        where TError : IRocketMessage<TError>
    {
        ValueTask<IRocketRemotingCaller<TError>> ConnectAsync(uint functionId, CancellationToken cancellationToken = default);
    }
}
