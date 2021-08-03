using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Omnius.Core.Net.Connections.Internal;
using Omnius.Core.Pipelines;

namespace Omnius.Core.Net.Connections.Multiplexer.V1.Internal
{
    internal partial class ConnectionMultiplexer
    {
        internal sealed class StreamConnection : AsyncDisposableBase, IConnection
        {
            private readonly int _maxSendDataQueueSize;
            private readonly int _maxReceiveDataQueueSize;
            private readonly IBytesPool _bytesPool;

            private BoundedMessagePipe<ArraySegment<byte>> _sendDataMessagePipe;
            private BoundedMessagePipe<ArraySegment<byte>> _receiveDataMessagePipe;
            private BoundedMessagePipe _sendDataAcceptedMessagePipe;
            private ActionPipe _receiveDataAcceptedActionPipe;
            private readonly StreamConnectionSender _streamConnectionSender;
            private readonly StreamConnectionReceiver _streamConnectionReceiver;
            private readonly ConnectionSubscribers _subscribers;

            private BoundedMessagePipe _sendFinishMessagePipe;
            private ActionPipe _receiveFinishActionPipe;

            private List<IDisposable> _disposables = new();
            private readonly CancellationTokenSource _cancellationTokenSource = new();

            public StreamConnection(int maxSendDataQueueSize, int maxReceiveDataQueueSize, IBytesPool bytesPool)
            {
                _maxSendDataQueueSize = maxSendDataQueueSize;
                _maxReceiveDataQueueSize = maxReceiveDataQueueSize;
                _bytesPool = bytesPool;

                _sendDataMessagePipe = new BoundedMessagePipe<ArraySegment<byte>>(_maxSendDataQueueSize);
                _disposables.Add(_sendDataMessagePipe);
                _receiveDataMessagePipe = new BoundedMessagePipe<ArraySegment<byte>>(_maxReceiveDataQueueSize);
                _disposables.Add(_receiveDataMessagePipe);
                _sendDataAcceptedMessagePipe = new BoundedMessagePipe(_maxReceiveDataQueueSize);
                _receiveDataAcceptedActionPipe = new ActionPipe();

                _streamConnectionSender = new StreamConnectionSender(maxSendDataQueueSize, _sendDataMessagePipe.Writer, _receiveDataAcceptedActionPipe.Subscriber, _bytesPool, _cancellationTokenSource.Token, _disposables);
                _streamConnectionReceiver = new StreamConnectionReceiver(_receiveDataMessagePipe.Reader, _sendDataAcceptedMessagePipe.Writer, _bytesPool, _cancellationTokenSource.Token);
                _subscribers = new ConnectionSubscribers(_cancellationTokenSource.Token);

                _sendFinishMessagePipe = new BoundedMessagePipe(1);
                _receiveFinishActionPipe = new ActionPipe();
                _disposables.Add(_receiveFinishActionPipe.Subscriber.Subscribe(() => _cancellationTokenSource.Cancel()));

                _disposables.Add(_cancellationTokenSource);
            }

            public bool IsConnected => !this.IsDisposed;

            public IConnectionSender Sender => _streamConnectionSender;

            public IConnectionReceiver Receiver => _streamConnectionReceiver;

            public IConnectionSubscribers Subscribers => _subscribers;

            internal IMessagePipeReader<ArraySegment<byte>> SendDataReader => _sendDataMessagePipe.Reader;

            internal IMessagePipeWriter<ArraySegment<byte>> ReceiveDataWriter => _receiveDataMessagePipe.Writer;

            internal IMessagePipeReader SendDataAcceptedReader => _sendDataAcceptedMessagePipe.Reader;

            internal IActionPublicher ReceiveDataAcceptedPublicher => _receiveDataAcceptedActionPipe.Publicher;

            internal IMessagePipeReader SendFinishReader => _sendFinishMessagePipe.Reader;

            internal IActionPublicher ReceiveFinishPublicher => _receiveFinishActionPipe.Publicher;

            protected override async ValueTask OnDisposeAsync()
            {
                if (!_cancellationTokenSource.IsCancellationRequested)
                {
                    _cancellationTokenSource.Cancel();
                    _sendFinishMessagePipe.Writer.TryWrite();
                }

                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
