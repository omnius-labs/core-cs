using System.Net.Sockets;

namespace Core.Net.I2p;

public sealed partial class SamBridge
{
    public sealed class SamBridgeAcceptResult
    {
        public SamBridgeAcceptResult(Socket socket, string destination)
        {
            this.Socket = socket;
            this.Destination = destination;
        }

        public Socket Socket { get; }
        public string Destination { get; }
    }
}
