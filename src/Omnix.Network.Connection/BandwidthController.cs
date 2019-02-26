using System;
using System.Collections.Generic;
using System.Text;
using Omnix.Base;

namespace Omnix.Network.Connection
{
    public sealed class BandwidthController : DisposableBase
    {
        private Queue<(DateTime time, int size)> _sendInfoQueue;

        public int SendBytesPerSecond { get; set; }
        public int ReceiveBytesPerSecond { get; set; }

        public int ComputeSendBytes(int size)
        {
            var lowerLimit = DateTime.UtcNow.AddSeconds(-1);

            while (_sendInfoQueue.Peek().time < lowerLimit)
            {
                _sendInfoQueue.Dequeue();
            }

            throw new NotImplementedException();
        }

        public int ComputeReceiveBytes(int size)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
