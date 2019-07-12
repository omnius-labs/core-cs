
#nullable enable

namespace Omnix.Remoting
{
    public sealed partial class OmniRpcErrorMessage : Omnix.Serialization.RocketPack.RocketPackMessageBase<OmniRpcErrorMessage>
    {
        static OmniRpcErrorMessage()
        {
            OmniRpcErrorMessage.Formatter = new CustomFormatter();
            OmniRpcErrorMessage.Empty = new OmniRpcErrorMessage(string.Empty, string.Empty, string.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxTypeLength = 8192;
        public static readonly int MaxMessageLength = 8192;
        public static readonly int MaxStackTraceLength = 8192;

        public OmniRpcErrorMessage(string type, string message, string stackTrace)
        {
            if (type is null) throw new System.ArgumentNullException("type");
            if (type.Length > 8192) throw new System.ArgumentOutOfRangeException("type");
            if (message is null) throw new System.ArgumentNullException("message");
            if (message.Length > 8192) throw new System.ArgumentOutOfRangeException("message");
            if (stackTrace is null) throw new System.ArgumentNullException("stackTrace");
            if (stackTrace.Length > 8192) throw new System.ArgumentOutOfRangeException("stackTrace");

            this.Type = type;
            this.Message = message;
            this.StackTrace = stackTrace;

            {
                var __h = new System.HashCode();
                if (this.Type != default) __h.Add(this.Type.GetHashCode());
                if (this.Message != default) __h.Add(this.Message.GetHashCode());
                if (this.StackTrace != default) __h.Add(this.StackTrace.GetHashCode());
                __hashCode = __h.ToHashCode();
            }
        }

        public string Type { get; }
        public string Message { get; }
        public string StackTrace { get; }

        public override bool Equals(OmniRpcErrorMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Type != target.Type) return false;
            if (this.Message != target.Message) return false;
            if (this.StackTrace != target.StackTrace) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<OmniRpcErrorMessage>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, OmniRpcErrorMessage value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.Type != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.Message != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.StackTrace != string.Empty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.Type != string.Empty)
                {
                    w.Write((uint)0);
                    w.Write(value.Type);
                }
                if (value.Message != string.Empty)
                {
                    w.Write((uint)1);
                    w.Write(value.Message);
                }
                if (value.StackTrace != string.Empty)
                {
                    w.Write((uint)2);
                    w.Write(value.StackTrace);
                }
            }

            public OmniRpcErrorMessage Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                uint propertyCount = r.GetUInt32();

                string p_type = string.Empty;
                string p_message = string.Empty;
                string p_stackTrace = string.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_type = r.GetString(8192);
                                break;
                            }
                        case 1:
                            {
                                p_message = r.GetString(8192);
                                break;
                            }
                        case 2:
                            {
                                p_stackTrace = r.GetString(8192);
                                break;
                            }
                    }
                }

                return new OmniRpcErrorMessage(p_type, p_message, p_stackTrace);
            }
        }
    }

}
