namespace FormatterBenchmarks.Internal
{
    [MessagePack.MessagePackObject]
    public class MessagePack_IntElementsList
    {
        [MessagePack.Key(0)]
        public virtual MessagePack_IntElements[]? List { get; set; }
    }

    [MessagePack.MessagePackObject]
    public class MessagePack_IntElements
    {
        [MessagePack.Key(0)]
        public virtual uint X0 { get; set; }
        [MessagePack.Key(1)]
        public virtual uint X1 { get; set; }
        [MessagePack.Key(2)]
        public virtual uint X2 { get; set; }
        [MessagePack.Key(3)]
        public virtual uint X3 { get; set; }
        [MessagePack.Key(4)]
        public virtual uint X4 { get; set; }
        [MessagePack.Key(5)]
        public virtual uint X5 { get; set; }
        [MessagePack.Key(6)]
        public virtual uint X6 { get; set; }
        [MessagePack.Key(7)]
        public virtual uint X7 { get; set; }
        [MessagePack.Key(8)]
        public virtual uint X8 { get; set; }
        [MessagePack.Key(9)]
        public virtual uint X9 { get; set; }
    }
}
