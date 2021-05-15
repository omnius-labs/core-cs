using System;
using System.Threading;
using System.Threading.Tasks;

namespace Omnius.Core.Net.Upnp
{
    public interface IUpnpClientFactory
    {
        IUpnpClient Create();
    }

    public interface IUpnpClient : IDisposable
    {
        bool IsConnected { get; }

        ValueTask ConnectAsync(CancellationToken cancellationToken = default);

        ValueTask<string> GetExternalIpAddressAsync(CancellationToken cancellationToken = default);

        ValueTask<bool> OpenPortAsync(UpnpProtocolType protocol, int externalPort, int internalPort, string description, CancellationToken cancellationToken = default);

        ValueTask<bool> OpenPortAsync(UpnpProtocolType protocol, string machineIp, int externalPort, int internalPort, string description, CancellationToken cancellationToken = default);

        ValueTask<bool> ClosePortAsync(UpnpProtocolType protocol, int externalPort, CancellationToken cancellationToken = default);

        ValueTask<dynamic> GetPortEntryAsync(int index, CancellationToken cancellationToken = default);
    }
}
