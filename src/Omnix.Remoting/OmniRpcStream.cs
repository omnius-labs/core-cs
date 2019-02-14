using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Omnix.Base;
using Omnix.Network.Connection;
using Omnix.Serialization.RocketPack;

namespace Omnix.Remoting
{
    public sealed class OmniRpcStream
    {
        private IConnection _connection;

        internal OmniRpcStream(IConnection connection)
        {
            _connection = connection;
        }

        public async ValueTask SendMessageAsync(Action<IBufferWriter<byte>> callback, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)OmniRpcStreamPacketType.Data;
                bufferWriter.Advance(1);

                callback.Invoke(bufferWriter);
            }, token);
        }

        public async ValueTask SendCancelAsync(CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)OmniRpcStreamPacketType.Cancel;
                bufferWriter.Advance(1);
            }, token);
        }

        public async ValueTask SendCompleteAsync(CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)OmniRpcStreamPacketType.Complete;
                bufferWriter.Advance(1);
            }, token);
        }

        public async ValueTask SendErrorAsync(ErrorMessage errorMessage, CancellationToken token = default)
        {
            await _connection.EnqueueAsync((bufferWriter) =>
            {
                bufferWriter.GetSpan(1)[0] = (byte)OmniRpcStreamPacketType.Error;
                bufferWriter.Advance(1);
                errorMessage.Export(bufferWriter, BufferPool.Shared);
            }, token);
        }

        public async ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> message = default, Action cancel = default, Action complete = default, Action<ErrorMessage> error = default, CancellationToken token = default)
        {
            await _connection.DequeueAsync((sequence) =>
            {
                Span<byte> type = stackalloc byte[1];
                sequence.CopyTo(type);

                switch ((OmniRpcStreamPacketType)type[0])
                {
                    case OmniRpcStreamPacketType.Data:
                        message.Invoke(sequence.Slice(1));
                        break;
                    case OmniRpcStreamPacketType.Cancel:
                        cancel.Invoke();
                        break;
                    case OmniRpcStreamPacketType.Complete:
                        complete.Invoke();
                        break;
                    case OmniRpcStreamPacketType.Error:
                        error.Invoke(ErrorMessage.Import(sequence.Slice(1), BufferPool.Shared));
                        break;
                }
            }, token);
        }
    }
}
