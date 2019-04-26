namespace FormatterBenchmarks.Internal
{
    [MessagePack.MessagePackObject]
    public class MessagePack_BytesElementsList
    {
        [MessagePack.Key(0)]
        public virtual MessagePack_BytesElements[]? List { get; set; }
    }

    [MessagePack.MessagePackObject]
    public class MessagePack_BytesElements
    {
        [MessagePack.Key(0)]
        public virtual byte[]? X0 { get; set; }
        [MessagePack.Key(1)]
        public virtual byte[]? X1 { get; set; }
        [MessagePack.Key(2)]
        public virtual byte[]? X2 { get; set; }
        [MessagePack.Key(3)]
        public virtual byte[]? X3 { get; set; }
        [MessagePack.Key(4)]
        public virtual byte[]? X4 { get; set; }
        [MessagePack.Key(5)]
        public virtual byte[]? X5 { get; set; }
        [MessagePack.Key(6)]
        public virtual byte[]? X6 { get; set; }
        [MessagePack.Key(7)]
        public virtual byte[]? X7 { get; set; }
        [MessagePack.Key(8)]
        public virtual byte[]? X8 { get; set; }
        [MessagePack.Key(9)]
        public virtual byte[]? X9 { get; set; }
    }
}
