namespace FormatterBenchmarks.Internal
{
    [MessagePack.MessagePackObject]
    public class MessagePack_BytesMessage
    {
        [MessagePack.Key(0)]
        public virtual byte[] Bytes { get; set; }
    }
}
