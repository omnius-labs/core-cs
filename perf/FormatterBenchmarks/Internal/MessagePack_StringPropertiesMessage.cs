namespace FormatterBenchmarks.Internal
{
    [MessagePack.MessagePackObject]
    public class MessagePack_StringPropertiesListMessage
    {
        [MessagePack.Key(0)]
        public virtual MessagePack_StringPropertiesMessage[] List { get; set; }
    }

    [MessagePack.MessagePackObject]
    public class MessagePack_StringPropertiesMessage
    {
        [MessagePack.Key(0)]
        public virtual string MyProperty1 { get; set; }
        [MessagePack.Key(1)]
        public virtual string MyProperty2 { get; set; }
        [MessagePack.Key(2)]
        public virtual string MyProperty3 { get; set; }
    }
}
