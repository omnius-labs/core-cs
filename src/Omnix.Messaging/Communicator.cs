using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace Omnix.Messaging
{
    public sealed class Communicator
    {
        private readonly CommunicatorOptions _options;

        public Communicator(CommunicatorOptions options)
        {
            if (options.SendQueueSize < 8) throw new ArgumentOutOfRangeException(nameof(options.SendQueueSize));
            if (options.ReceiveQueueSize < 8) throw new ArgumentOutOfRangeException(nameof(options.ReceiveQueueSize));
            if (options.PacketSize < 256) throw new ArgumentOutOfRangeException(nameof(options.PacketSize));

            _options = options;
        }

        public void Send(MessagingPriorityType priorityType, Action<IBufferWriter<byte>> action)
        {

        }

        public ValueTask SendAsync(MessagingPriorityType priorityType, Action<IBufferWriter<byte>> action, CancellationToken token = default)
        {

        }

        public void Receive(Action<ReadOnlySequence<byte>> action)
        {

        }

        public ValueTask ReceiveAsync(Action<ReadOnlySequence<byte>> action, CancellationToken token = default)
        {

        }

        private sealed class SendItem
        {

        }


    }
}
