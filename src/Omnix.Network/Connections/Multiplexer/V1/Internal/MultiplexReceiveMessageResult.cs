namespace Omnix.Network.Connections.Multiplexer.V1.Internal
{
    internal sealed class MultiplexReceiveMessageResult
    {
        public MultiplexReceiveMessageResult(SessionConnectMessage? connectMessage = null,
            SessionAcceptMessage? acceptMessage = null,
            SessionUpdateWindowSizeMessage? updateWindowSizeMessage = null,
            SessionDataMessage? dataMessage = null,
            SessionCloseMessage? closeMessage = null,
            SessionErrorMessage? errorMessage = null)
        {
            this.ConnectMessage = connectMessage;
            this.AcceptMessage = acceptMessage;
            this.UpdateWindowSizeMessage = updateWindowSizeMessage;
            this.DataMessage = dataMessage;
            this.CloseMessage = closeMessage;
            this.ErrorMessage = errorMessage;
        }

        public SessionConnectMessage? ConnectMessage { get; }
        public SessionAcceptMessage? AcceptMessage { get; }
        public SessionUpdateWindowSizeMessage? UpdateWindowSizeMessage { get; }
        public SessionDataMessage? DataMessage { get; }
        public SessionCloseMessage? CloseMessage { get; }
        public SessionErrorMessage? ErrorMessage { get; }
    }
}
