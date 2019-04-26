namespace FormatterBenchmarks.Internal
{
    [MessagePack.MessagePackObject]
    public class MessagePack_StringElementsList
    {
        [MessagePack.Key(0)]
        public virtual MessagePack_StringElements[]? List { get; set; }
    }

    [MessagePack.MessagePackObject]
    public class MessagePack_StringElements
    {
        [MessagePack.Key(0)]
        public virtual string? X0 { get; set; }

        [MessagePack.Key(1)]
        public virtual string? X1 { get; set; }

        [MessagePack.Key(2)]
        public virtual string? X2 { get; set; }

        [MessagePack.Key(3)]
        public virtual string? X3 { get; set; }

        [MessagePack.Key(4)]
        public virtual string? X4 { get; set; }

        [MessagePack.Key(5)]
        public virtual string? X5 { get; set; }

        [MessagePack.Key(6)]
        public virtual string? X6 { get; set; }

        [MessagePack.Key(7)]
        public virtual string? X7 { get; set; }

        [MessagePack.Key(8)]
        public virtual string? X8 { get; set; }

        [MessagePack.Key(9)]
        public virtual string? X9 { get; set; }
    }
}
