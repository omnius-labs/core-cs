
#nullable enable

namespace Omnius.Core.Remoting
{
    public sealed partial class OmniRpcErrorMessage : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<OmniRpcErrorMessage>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<OmniRpcErrorMessage> Formatter { get; }
        public static OmniRpcErrorMessage Empty { get; }

        static OmniRpcErrorMessage()
        {
            OmniRpcErrorMessage.Formatter = new ___CustomFormatter();
            OmniRpcErrorMessage.Empty = new OmniRpcErrorMessage(string.Empty, string.Empty, string.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxTypeLength = 8192;
        public static readonly int MaxMessageLength = 8192;
        public static readonly int MaxStackTraceLength = 8192;

        public OmniRpcErrorMessage(string type, string message, string stackTrace)
        {
            if (type is null) throw new global::System.ArgumentNullException("type");
            if (type.Length > 8192) throw new global::System.ArgumentOutOfRangeException("type");
            if (message is null) throw new global::System.ArgumentNullException("message");
            if (message.Length > 8192) throw new global::System.ArgumentOutOfRangeException("message");
            if (stackTrace is null) throw new global::System.ArgumentNullException("stackTrace");
            if (stackTrace.Length > 8192) throw new global::System.ArgumentOutOfRangeException("stackTrace");

            this.Type = type;
            this.Message = message;
            this.StackTrace = stackTrace;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (type != default) ___h.Add(type.GetHashCode());
                if (message != default) ___h.Add(message.GetHashCode());
                if (stackTrace != default) ___h.Add(stackTrace.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public string Type { get; }
        public string Message { get; }
        public string StackTrace { get; }

        public static OmniRpcErrorMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(OmniRpcErrorMessage? left, OmniRpcErrorMessage? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(OmniRpcErrorMessage? left, OmniRpcErrorMessage? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is OmniRpcErrorMessage)) return false;
            return this.Equals((OmniRpcErrorMessage)other);
        }
        public bool Equals(OmniRpcErrorMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.Type != target.Type) return false;
            if (this.Message != target.Message) return false;
            if (this.StackTrace != target.StackTrace) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<OmniRpcErrorMessage>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in OmniRpcErrorMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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

            public OmniRpcErrorMessage Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
