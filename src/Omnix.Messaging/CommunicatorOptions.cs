using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Omnix.Messaging
{
    public sealed class CommunicatorOptions
    {
        public CommunicatorOptions()
        {
            this.SendQueueSize = 256;
            this.ReceiveQueueSize = 256;
            this.PacketSize = 4096;
        }

        public int SendQueueSize { get; set; }
        public int ReceiveQueueSize { get; set; }
        public int PacketSize { get; set; }
    }
}
