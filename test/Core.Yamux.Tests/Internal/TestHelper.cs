using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Omnius.Core.Base;
using Xunit.Abstractions;

namespace Omnius.Core.Yamux.Internal;

public class TestHelper : IDisposable
{
    private readonly ITestOutputHelper _output;

    private TcpListener? _listener;

    public TestHelper(ITestOutputHelper output)
    {
        _output = output;
    }

    public void Dispose()
    {
        if (_listener != null)
        {
            _listener.Stop();
            _listener = null;
        }
    }

    public async ValueTask<(YamuxConnection, YamuxConnection)> CreateYamuxConnectionPair(ILogger logger)
    {
        return await this.CreateYamuxConnectionPair(TimeProvider.System, logger);
    }

    public async ValueTask<(YamuxConnection, YamuxConnection)> CreateYamuxConnectionPair(TimeProvider timeProvider, ILogger logger)
    {
        return await this.CreateYamuxConnectionPair(new YamuxConfig(), timeProvider, logger);
    }

    public async ValueTask<(YamuxConnection, YamuxConnection)> CreateYamuxConnectionPair(YamuxConfig yamuxConfig, ILogger logger)
    {
        return await this.CreateYamuxConnectionPair(yamuxConfig, yamuxConfig, TimeProvider.System, logger);
    }

    public async ValueTask<(YamuxConnection, YamuxConnection)> CreateYamuxConnectionPair(YamuxConfig yamuxConfig, TimeProvider timeProvider, ILogger logger)
    {
        return await this.CreateYamuxConnectionPair(yamuxConfig, yamuxConfig, timeProvider, logger);
    }

    public async ValueTask<(YamuxConnection, YamuxConnection)> CreateYamuxConnectionPair(YamuxConfig serverYamuxConfig, YamuxConfig clientYamuxConfig, TimeProvider timeProvider, ILogger logger)
    {
        var (client, server) = DuplexStream.CreatePair();

        var serverYamuxConnection = new YamuxConnection(
            client,
            serverYamuxConfig,
            YamuxMode.Server,
            BytesPool.Shared);
        var clientYamuxConnection = new YamuxConnection(
            server,
            clientYamuxConfig,
            YamuxMode.Client,
            BytesPool.Shared);

        return (clientYamuxConnection, serverYamuxConnection);
    }
}
