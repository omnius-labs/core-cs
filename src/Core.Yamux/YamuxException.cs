namespace Omnius.Yamux
{
    public class YamuxException : Exception
    {
        private YamuxErrorCode _errorCode;

        public YamuxException(YamuxErrorCode errorCode) { _errorCode = errorCode; }
        public YamuxException(YamuxErrorCode errorCode, string message) : base(message) { _errorCode = errorCode; }
        public YamuxException(YamuxErrorCode errorCode, string message, Exception innerException) : base(message, innerException) { _errorCode = errorCode; }

        public YamuxErrorCode ErrorCode => _errorCode;
    }
}
