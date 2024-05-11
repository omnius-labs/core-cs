namespace Core.Net.Connections.Multiplexer.V1;

public record OmniConnectionMultiplexerOptions
{
    public required OmniConnectionMultiplexerType Type { get; init; }
    public required TimeSpan PacketReceiveTimeout { get; init; }
    public required uint MaxStreamRequestQueueSize { get; init; }
    public required uint MaxStreamDataSize { get; init; }
    public required uint MaxStreamDataQueueSize { get; init; }
}
