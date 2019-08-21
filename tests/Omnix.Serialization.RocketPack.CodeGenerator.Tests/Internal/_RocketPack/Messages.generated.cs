
#nullable enable

namespace Omnix.Serialization.RocketPack.CodeGenerator.Internal
{
    internal enum Enum1 : sbyte
    {
        Yes = 0,
        No = 1,
    }

    internal enum Enum2 : short
    {
        Yes = 0,
        No = 1,
    }

    internal enum Enum3 : int
    {
        Yes = 0,
        No = 1,
    }

    internal enum Enum4 : long
    {
        Yes = 0,
        No = 1,
    }

    internal enum Enum5 : byte
    {
        Yes = 0,
        No = 1,
    }

    internal enum Enum6 : ushort
    {
        Yes = 0,
        No = 1,
    }

    internal enum Enum7 : uint
    {
        Yes = 0,
        No = 1,
    }

    internal enum Enum8 : ulong
    {
        Yes = 0,
        No = 1,
    }

    internal readonly struct SmallMessageStructElement : global::Omnix.Serialization.RocketPack.IRocketPackMessage<SmallMessageStructElement>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallMessageStructElement> Formatter { get; }
        public static SmallMessageStructElement Empty { get; }

        static SmallMessageStructElement()
        {
            SmallMessageStructElement.Formatter = new ___CustomFormatter();
            SmallMessageStructElement.Empty = new SmallMessageStructElement(false);
        }

        private readonly int ___hashCode;

        public SmallMessageStructElement(bool x0)
        {
            this.X0 = x0;

            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                ___hashCode = ___h.ToHashCode();
            }
        }

        public bool X0 { get; }

        public static SmallMessageStructElement Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SmallMessageStructElement left, SmallMessageStructElement right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(SmallMessageStructElement left, SmallMessageStructElement right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SmallMessageStructElement)) return false;
            return this.Equals((SmallMessageStructElement)other);
        }
        public bool Equals(SmallMessageStructElement target)
        {
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallMessageStructElement>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in SmallMessageStructElement value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.X0);
            }

            public SmallMessageStructElement Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                bool p_x0 = false;

                {
                    p_x0 = r.GetBoolean();
                }
                return new SmallMessageStructElement(p_x0);
            }
        }
    }

    internal readonly struct MessageStructElement : global::Omnix.Serialization.RocketPack.IRocketPackMessage<MessageStructElement>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<MessageStructElement> Formatter { get; }
        public static MessageStructElement Empty { get; }

        static MessageStructElement()
        {
            MessageStructElement.Formatter = new ___CustomFormatter();
            MessageStructElement.Empty = new MessageStructElement(false);
        }

        private readonly int ___hashCode;

        public MessageStructElement(bool x0)
        {
            this.X0 = x0;

            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                ___hashCode = ___h.ToHashCode();
            }
        }

        public bool X0 { get; }

        public static MessageStructElement Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(MessageStructElement left, MessageStructElement right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(MessageStructElement left, MessageStructElement right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is MessageStructElement)) return false;
            return this.Equals((MessageStructElement)other);
        }
        public bool Equals(MessageStructElement target)
        {
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<MessageStructElement>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in MessageStructElement value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.X0 != false)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.X0 != false)
                {
                    w.Write((uint)0);
                    w.Write(value.X0);
                }
            }

            public MessageStructElement Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                bool p_x0 = false;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_x0 = r.GetBoolean();
                                break;
                            }
                    }
                }

                return new MessageStructElement(p_x0);
            }
        }
    }

}
