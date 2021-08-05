using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Connections;

namespace Omnius.Core.RocketPack.Remoting
{
    public interface IRocketRemotingAccepter<TError>
        where TError : IRocketMessage<TError>
    {
        ValueTask<IRocketRemotingListener<TError>> AcceptAsync(CancellationToken cancellationToken = default);
    }
}
