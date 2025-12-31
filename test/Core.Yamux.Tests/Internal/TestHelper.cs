using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Omnius.Yamux.Internal;

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

    public async ValueTask<(YamuxMultiplexer, YamuxMultiplexer)> CreateYamuxMultiplexerPair(ILogger logger)
    {
        return await this.CreateYamuxMultiplexerPair(TimeProvider.System, logger);
    }

    public async ValueTask<(YamuxMultiplexer, YamuxMultiplexer)> CreateYamuxMultiplexerPair(TimeProvider timeProvider, ILogger logger)
    {
        return await this.CreateYamuxMultiplexerPair(new YamuxOptions(), timeProvider, logger);
    }

    public async ValueTask<(YamuxMultiplexer, YamuxMultiplexer)> CreateYamuxMultiplexerPair(YamuxOptions yamuxOptions, ILogger logger)
    {
        return await this.CreateYamuxMultiplexerPair(yamuxOptions, yamuxOptions, TimeProvider.System, logger);
    }

    public async ValueTask<(YamuxMultiplexer, YamuxMultiplexer)> CreateYamuxMultiplexerPair(YamuxOptions yamuxOptions, TimeProvider timeProvider, ILogger logger)
    {
        return await this.CreateYamuxMultiplexerPair(yamuxOptions, yamuxOptions, timeProvider, logger);
    }

    public async ValueTask<(YamuxMultiplexer, YamuxMultiplexer)> CreateYamuxMultiplexerPair(YamuxOptions serverYamuxOptions, YamuxOptions clientYamuxOptions, TimeProvider timeProvider, ILogger logger)
    {
        var (client, server) = DuplexStream.CreatePair();

        var serverYamuxMultiplexer = new YamuxMultiplexer(YamuxSessionType.Server, client, serverYamuxOptions, timeProvider, logger);
        var clientYamuxMultiplexer = new YamuxMultiplexer(YamuxSessionType.Client, server, clientYamuxOptions, timeProvider, logger);

        return (clientYamuxMultiplexer, serverYamuxMultiplexer);
    }
}
