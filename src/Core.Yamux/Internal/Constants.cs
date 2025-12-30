namespace Omnius.Yamux.Internal;

internal static class Constants
{
    public const int PROTO_VERSION = 0;
    public const int INITIAL_STREAM_WINDOW = 256 * 1024;
    public class HeaderSize
    {
        public const int VERSION = 1;
        public const int TYPE = 1;
        public const int FLAGS = 2;
        public const int STREAM_ID = 4;
        public const int LENGTH = 4;
        public const int TOTAL = VERSION + TYPE + FLAGS + STREAM_ID + LENGTH;
    }
}
