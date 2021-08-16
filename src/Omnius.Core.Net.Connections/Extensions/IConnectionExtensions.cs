using System;
using System.Buffers;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Connections;
using Omnius.Core.RocketPack;

namespace Omnius.Core.Net.Connections
{
    public static class IConnectionExtensions
    {
        public static async ValueTask<TMessage> ExchangeAsync<TMessage>(this IConnection connection, TMessage message, CancellationToken cancellationToken = default)
            where TMessage : IRocketMessage<TMessage>
        {
            var sendTask = connection.Sender.SendAsync(message, cancellationToken).AsTask();
            var receiveTask = connection.Receiver.ReceiveAsync<TMessage>(cancellationToken).AsTask();

            await Task.WhenAll(sendTask, receiveTask).ConfigureAwait(false);
            return receiveTask.Result;
        }

        public static async ValueTask<TReceiveMessage> SendAndReceiveAsync<TSendMessage, TReceiveMessage>(this IConnection connection, TSendMessage message, CancellationToken cancellationToken = default)
            where TReceiveMessage : IRocketMessage<TReceiveMessage>
            where TSendMessage : IRocketMessage<TSendMessage>
        {
            await connection.Sender.SendAsync(message, cancellationToken);
            return await connection.Receiver.ReceiveAsync<TReceiveMessage>(cancellationToken);
        }

        public static async ValueTask ReceiveAndSendAsync<TReceiveMessage, TSendMessage>(this IConnection connection, Func<TReceiveMessage, TSendMessage> callback, CancellationToken cancellationToken = default)
            where TReceiveMessage : IRocketMessage<TReceiveMessage>
            where TSendMessage : IRocketMessage<TSendMessage>
        {
            var receivedMessage = await connection.Receiver.ReceiveAsync<TReceiveMessage>(cancellationToken);
            var sendingMessage = callback.Invoke(receivedMessage);
            await connection.Sender.SendAsync(sendingMessage, cancellationToken);
        }
    }
}
