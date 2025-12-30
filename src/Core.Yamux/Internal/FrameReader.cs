using System.Buffers;
using Microsoft.Extensions.Logging;
using Omnius.Yamux;

namespace Omnius.Yamux.Internal;

internal sealed class FrameReader
{
    private readonly Stream _stream;
    private readonly ILogger _logger;
    private readonly ArrayPool<byte> _bytesPool;

    public FrameReader(Stream stream, ArrayPool<byte> bytesPool, ILogger logger)
    {
        _stream = stream;
        _bytesPool = bytesPool;
        _logger = logger;
    }

    public async ValueTask<Header> ReadHeaderAsync(CancellationToken cancellationToken)
    {
        byte[] buffer = new byte[Constants.HeaderSize.TOTAL];

        try
        {
            int remain = Constants.HeaderSize.TOTAL;
            while (remain > 0)
            {
                int read = await _stream.ReadAsync(buffer, Constants.HeaderSize.TOTAL - remain, remain, cancellationToken);
                if (read == 0) throw new YamuxException(YamuxErrorCode.ConnectionShutdown);
                remain -= read;
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (YamuxException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "yamux: failed to read header");
            throw new YamuxException(YamuxErrorCode.ConnectionReceiveError);
        }

        return new Header(buffer);
    }

    public async ValueTask DiscardPayloadAsync(uint length, uint streamId, CancellationToken cancellationToken)
    {
        if (length == 0) return;

        byte[] buffer = _bytesPool.Rent(4096);
        try
        {
            int remain = (int)length;
            while (remain > 0)
            {
                int readLength = await _stream.ReadAsync(buffer, 0, Math.Min(buffer.Length, remain), cancellationToken);
                if (readLength == 0) throw new IOException("Stream closed");
                remain -= readLength;
            }
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "yamux: failed to discard data: {0}", streamId);
        }
        finally
        {
            _bytesPool.Return(buffer);
        }
    }
}
