
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

    internal readonly struct SmallMessageElement : System.IEquatable<SmallMessageElement>
    {
        public static Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallMessageElement> Formatter { get; }
        public static SmallMessageElement Empty { get; }

        static SmallMessageElement()
        {
            SmallMessageElement.Formatter = new CustomFormatter();
            SmallMessageElement.Empty = new SmallMessageElement(false);
        }

        private readonly int __hashCode;

        public SmallMessageElement(bool x0)
        {
            this.X0 = x0;

            {
                var __h = new System.HashCode();
                if (this.X0 != default) __h.Add(this.X0.GetHashCode());
                __hashCode = __h.ToHashCode();
            }
        }

        public bool X0 { get; }

        public static SmallMessageElement Import(System.Buffers.ReadOnlySequence<byte> sequence, Omnix.Base.BufferPool bufferPool)
        {
            return Formatter.Deserialize(new Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool), 0);
        }
        public void Export(System.Buffers.IBufferWriter<byte> bufferWriter, Omnix.Base.BufferPool bufferPool)
        {
            Formatter.Serialize(new Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool), this, 0);
        }
        public static bool operator ==(SmallMessageElement left, SmallMessageElement right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(SmallMessageElement left, SmallMessageElement right)
        {
            return !(left == right);
        }
        public override bool Equals(object other)
        {
            if (!(other is SmallMessageElement)) return false;
            return this.Equals((SmallMessageElement)other);
        }

        public bool Equals(SmallMessageElement target)
        {
            if (this.X0 != target.X0) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallMessageElement>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, SmallMessageElement value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                if (value.X0 != false)
                {
                    w.Write(value.X0);
                }
            }

            public SmallMessageElement Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                bool p_x0 = false;

                {
                    p_x0 = r.GetBoolean();
                }
                return new SmallMessageElement(p_x0);
            }
        }
    }

    internal sealed partial class MessageElement : Omnix.Serialization.RocketPack.RocketPackMessageBase<MessageElement>
    {
        static MessageElement()
        {
            MessageElement.Formatter = new CustomFormatter();
            MessageElement.Empty = new MessageElement(false);
        }

        private readonly int __hashCode;

        public MessageElement(bool x0)
        {
            this.X0 = x0;

            {
                var __h = new System.HashCode();
                if (this.X0 != default) __h.Add(this.X0.GetHashCode());
                __hashCode = __h.ToHashCode();
            }
        }

        public bool X0 { get; }

        public override bool Equals(MessageElement? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.X0 != target.X0) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<MessageElement>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, MessageElement value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

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

            public MessageElement Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

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

                return new MessageElement(p_x0);
            }
        }
    }

    internal readonly struct SmallMessage : System.IEquatable<SmallMessage>
    {
        public static Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallMessage> Formatter { get; }
        public static SmallMessage Empty { get; }

        static SmallMessage()
        {
            SmallMessage.Formatter = new CustomFormatter();
            SmallMessage.Empty = new SmallMessage(false, 0, 0, 0, 0, 0, 0, 0, 0, (Enum1)0, (Enum2)0, (Enum3)0, (Enum4)0, (Enum5)0, (Enum6)0, (Enum7)0, (Enum8)0, 0.0F, 0.0D, string.Empty, Omnix.Serialization.RocketPack.Timestamp.Zero, System.ReadOnlyMemory<byte>.Empty, System.ReadOnlyMemory<byte>.Empty, System.Array.Empty<string>(), new System.Collections.Generic.Dictionary<byte, string>(), SmallMessageElement.Empty, MessageElement.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public SmallMessage(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, Omnix.Serialization.RocketPack.Timestamp x20, System.ReadOnlyMemory<byte> x21, System.ReadOnlyMemory<byte> x22, string[] x23, System.Collections.Generic.Dictionary<byte, string> x24, SmallMessageElement x25, MessageElement x26)
        {
            if (x19 is null) throw new System.ArgumentNullException("x19");
            if (x19.Length > 128) throw new System.ArgumentOutOfRangeException("x19");
            if (x21.Length > 256) throw new System.ArgumentOutOfRangeException("x21");
            if (x22.Length > 256) throw new System.ArgumentOutOfRangeException("x22");
            if (x23 is null) throw new System.ArgumentNullException("x23");
            if (x23.Length > 16) throw new System.ArgumentOutOfRangeException("x23");
            foreach (var n in x23)
            {
                if (n is null) throw new System.ArgumentNullException("n");
                if (n.Length > 128) throw new System.ArgumentOutOfRangeException("n");
            }
            if (x24 is null) throw new System.ArgumentNullException("x24");
            if (x24.Count > 32) throw new System.ArgumentOutOfRangeException("x24");
            foreach (var n in x24)
            {
                if (n.Value is null) throw new System.ArgumentNullException("n.Value");
                if (n.Value.Length > 128) throw new System.ArgumentOutOfRangeException("n.Value");
            }
            if (x26 is null) throw new System.ArgumentNullException("x26");

            this.X0 = x0;
            this.X1 = x1;
            this.X2 = x2;
            this.X3 = x3;
            this.X4 = x4;
            this.X5 = x5;
            this.X6 = x6;
            this.X7 = x7;
            this.X8 = x8;
            this.X9 = x9;
            this.X10 = x10;
            this.X11 = x11;
            this.X12 = x12;
            this.X13 = x13;
            this.X14 = x14;
            this.X15 = x15;
            this.X16 = x16;
            this.X17 = x17;
            this.X18 = x18;
            this.X19 = x19;
            this.X20 = x20;
            this.X21 = x21;
            this.X22 = x22;
            this.X23 = new Omnix.Collections.ReadOnlyListSlim<string>(x23);
            this.X24 = new Omnix.Collections.ReadOnlyDictionarySlim<byte, string>(x24);
            this.X25 = x25;
            this.X26 = x26;

            {
                var __h = new System.HashCode();
                if (this.X0 != default) __h.Add(this.X0.GetHashCode());
                if (this.X1 != default) __h.Add(this.X1.GetHashCode());
                if (this.X2 != default) __h.Add(this.X2.GetHashCode());
                if (this.X3 != default) __h.Add(this.X3.GetHashCode());
                if (this.X4 != default) __h.Add(this.X4.GetHashCode());
                if (this.X5 != default) __h.Add(this.X5.GetHashCode());
                if (this.X6 != default) __h.Add(this.X6.GetHashCode());
                if (this.X7 != default) __h.Add(this.X7.GetHashCode());
                if (this.X8 != default) __h.Add(this.X8.GetHashCode());
                if (this.X9 != default) __h.Add(this.X9.GetHashCode());
                if (this.X10 != default) __h.Add(this.X10.GetHashCode());
                if (this.X11 != default) __h.Add(this.X11.GetHashCode());
                if (this.X12 != default) __h.Add(this.X12.GetHashCode());
                if (this.X13 != default) __h.Add(this.X13.GetHashCode());
                if (this.X14 != default) __h.Add(this.X14.GetHashCode());
                if (this.X15 != default) __h.Add(this.X15.GetHashCode());
                if (this.X16 != default) __h.Add(this.X16.GetHashCode());
                if (this.X17 != default) __h.Add(this.X17.GetHashCode());
                if (this.X18 != default) __h.Add(this.X18.GetHashCode());
                if (this.X19 != default) __h.Add(this.X19.GetHashCode());
                if (this.X20 != default) __h.Add(this.X20.GetHashCode());
                if (!this.X21.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X21.Span));
                if (!this.X22.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X22.Span));
                foreach (var n in this.X23)
                {
                    if (n != default) __h.Add(n.GetHashCode());
                }
                foreach (var n in this.X24)
                {
                    if (n.Key != default) __h.Add(n.Key.GetHashCode());
                    if (n.Value != default) __h.Add(n.Value.GetHashCode());
                }
                if (this.X25 != default) __h.Add(this.X25.GetHashCode());
                if (this.X26 != default) __h.Add(this.X26.GetHashCode());
                __hashCode = __h.ToHashCode();
            }
        }

        public bool X0 { get; }
        public sbyte X1 { get; }
        public short X2 { get; }
        public int X3 { get; }
        public long X4 { get; }
        public byte X5 { get; }
        public ushort X6 { get; }
        public uint X7 { get; }
        public ulong X8 { get; }
        public Enum1 X9 { get; }
        public Enum2 X10 { get; }
        public Enum3 X11 { get; }
        public Enum4 X12 { get; }
        public Enum5 X13 { get; }
        public Enum6 X14 { get; }
        public Enum7 X15 { get; }
        public Enum8 X16 { get; }
        public float X17 { get; }
        public double X18 { get; }
        public string X19 { get; }
        public Omnix.Serialization.RocketPack.Timestamp X20 { get; }
        public System.ReadOnlyMemory<byte> X21 { get; }
        public System.ReadOnlyMemory<byte> X22 { get; }
        public Omnix.Collections.ReadOnlyListSlim<string> X23 { get; }
        public Omnix.Collections.ReadOnlyDictionarySlim<byte, string> X24 { get; }
        public SmallMessageElement X25 { get; }
        public MessageElement X26 { get; }

        public static SmallMessage Import(System.Buffers.ReadOnlySequence<byte> sequence, Omnix.Base.BufferPool bufferPool)
        {
            return Formatter.Deserialize(new Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool), 0);
        }
        public void Export(System.Buffers.IBufferWriter<byte> bufferWriter, Omnix.Base.BufferPool bufferPool)
        {
            Formatter.Serialize(new Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool), this, 0);
        }
        public static bool operator ==(SmallMessage left, SmallMessage right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(SmallMessage left, SmallMessage right)
        {
            return !(left == right);
        }
        public override bool Equals(object other)
        {
            if (!(other is SmallMessage)) return false;
            return this.Equals((SmallMessage)other);
        }

        public bool Equals(SmallMessage target)
        {
            if (this.X0 != target.X0) return false;
            if (this.X1 != target.X1) return false;
            if (this.X2 != target.X2) return false;
            if (this.X3 != target.X3) return false;
            if (this.X4 != target.X4) return false;
            if (this.X5 != target.X5) return false;
            if (this.X6 != target.X6) return false;
            if (this.X7 != target.X7) return false;
            if (this.X8 != target.X8) return false;
            if (this.X9 != target.X9) return false;
            if (this.X10 != target.X10) return false;
            if (this.X11 != target.X11) return false;
            if (this.X12 != target.X12) return false;
            if (this.X13 != target.X13) return false;
            if (this.X14 != target.X14) return false;
            if (this.X15 != target.X15) return false;
            if (this.X16 != target.X16) return false;
            if (this.X17 != target.X17) return false;
            if (this.X19 != target.X19) return false;
            if (this.X20 != target.X20) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.X21.Span, target.X21.Span)) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.X22.Span, target.X22.Span)) return false;
            if (!Omnix.Base.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if (!Omnix.Base.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallMessage>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, SmallMessage value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                if (value.X0 != false)
                {
                    w.Write(value.X0);
                }
                if (value.X1 != 0)
                {
                    w.Write(value.X1);
                }
                if (value.X2 != 0)
                {
                    w.Write(value.X2);
                }
                if (value.X3 != 0)
                {
                    w.Write(value.X3);
                }
                if (value.X4 != 0)
                {
                    w.Write(value.X4);
                }
                if (value.X5 != 0)
                {
                    w.Write(value.X5);
                }
                if (value.X6 != 0)
                {
                    w.Write(value.X6);
                }
                if (value.X7 != 0)
                {
                    w.Write(value.X7);
                }
                if (value.X8 != 0)
                {
                    w.Write(value.X8);
                }
                if (value.X9 != (Enum1)0)
                {
                    w.Write((long)value.X9);
                }
                if (value.X10 != (Enum2)0)
                {
                    w.Write((long)value.X10);
                }
                if (value.X11 != (Enum3)0)
                {
                    w.Write((long)value.X11);
                }
                if (value.X12 != (Enum4)0)
                {
                    w.Write((long)value.X12);
                }
                if (value.X13 != (Enum5)0)
                {
                    w.Write((ulong)value.X13);
                }
                if (value.X14 != (Enum6)0)
                {
                    w.Write((ulong)value.X14);
                }
                if (value.X15 != (Enum7)0)
                {
                    w.Write((ulong)value.X15);
                }
                if (value.X16 != (Enum8)0)
                {
                    w.Write((ulong)value.X16);
                }
                if (value.X17 != 0.0F)
                {
                    w.Write(value.X17);
                }
                if (value.X18 != 0.0D)
                {
                    w.Write(value.X18);
                }
                if (value.X19 != string.Empty)
                {
                    w.Write(value.X19);
                }
                if (value.X20 != Omnix.Serialization.RocketPack.Timestamp.Zero)
                {
                    w.Write(value.X20);
                }
                if (!value.X21.IsEmpty)
                {
                    w.Write(value.X21.Span);
                }
                if (!value.X22.IsEmpty)
                {
                    w.Write(value.X22.Span);
                }
                if (value.X23.Count != 0)
                {
                    w.Write((uint)value.X23.Count);
                    foreach (var n in value.X23)
                    {
                        w.Write(n);
                    }
                }
                if (value.X24.Count != 0)
                {
                    w.Write((uint)value.X24.Count);
                    foreach (var n in value.X24)
                    {
                        w.Write(n.Key);
                        w.Write(n.Value);
                    }
                }
                if (value.X25 != SmallMessageElement.Empty)
                {
                    SmallMessageElement.Formatter.Serialize(w, value.X25, rank + 1);
                }
                if (value.X26 != MessageElement.Empty)
                {
                    MessageElement.Formatter.Serialize(w, value.X26, rank + 1);
                }
            }

            public SmallMessage Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                bool p_x0 = false;
                sbyte p_x1 = 0;
                short p_x2 = 0;
                int p_x3 = 0;
                long p_x4 = 0;
                byte p_x5 = 0;
                ushort p_x6 = 0;
                uint p_x7 = 0;
                ulong p_x8 = 0;
                Enum1 p_x9 = (Enum1)0;
                Enum2 p_x10 = (Enum2)0;
                Enum3 p_x11 = (Enum3)0;
                Enum4 p_x12 = (Enum4)0;
                Enum5 p_x13 = (Enum5)0;
                Enum6 p_x14 = (Enum6)0;
                Enum7 p_x15 = (Enum7)0;
                Enum8 p_x16 = (Enum8)0;
                float p_x17 = 0.0F;
                double p_x18 = 0.0D;
                string p_x19 = string.Empty;
                Omnix.Serialization.RocketPack.Timestamp p_x20 = Omnix.Serialization.RocketPack.Timestamp.Zero;
                System.ReadOnlyMemory<byte> p_x21 = System.ReadOnlyMemory<byte>.Empty;
                System.ReadOnlyMemory<byte> p_x22 = System.ReadOnlyMemory<byte>.Empty;
                string[] p_x23 = System.Array.Empty<string>();
                System.Collections.Generic.Dictionary<byte, string> p_x24 = new System.Collections.Generic.Dictionary<byte, string>();
                SmallMessageElement p_x25 = SmallMessageElement.Empty;
                MessageElement p_x26 = MessageElement.Empty;

                {
                    p_x0 = r.GetBoolean();
                }
                {
                    p_x1 = r.GetInt8();
                }
                {
                    p_x2 = r.GetInt16();
                }
                {
                    p_x3 = r.GetInt32();
                }
                {
                    p_x4 = r.GetInt64();
                }
                {
                    p_x5 = r.GetUInt8();
                }
                {
                    p_x6 = r.GetUInt16();
                }
                {
                    p_x7 = r.GetUInt32();
                }
                {
                    p_x8 = r.GetUInt64();
                }
                {
                    p_x9 = (Enum1)r.GetInt64();
                }
                {
                    p_x10 = (Enum2)r.GetInt64();
                }
                {
                    p_x11 = (Enum3)r.GetInt64();
                }
                {
                    p_x12 = (Enum4)r.GetInt64();
                }
                {
                    p_x13 = (Enum5)r.GetUInt64();
                }
                {
                    p_x14 = (Enum6)r.GetUInt64();
                }
                {
                    p_x15 = (Enum7)r.GetUInt64();
                }
                {
                    p_x16 = (Enum8)r.GetUInt64();
                }
                {
                    p_x17 = r.GetFloat32();
                }
                {
                    p_x18 = r.GetFloat64();
                }
                {
                    p_x19 = r.GetString(128);
                }
                {
                    p_x20 = r.GetTimestamp();
                }
                {
                    p_x21 = r.GetMemory(256);
                }
                {
                    p_x22 = r.GetMemory(256);
                }
                {
                    var length = r.GetUInt32();
                    p_x23 = new string[length];
                    for (int i = 0; i < p_x23.Length; i++)
                    {
                        p_x23[i] = r.GetString(128);
                    }
                }
                {
                    var length = r.GetUInt32();
                    p_x24 = new System.Collections.Generic.Dictionary<byte, string>();
                    byte t_key = 0;
                    string t_value = string.Empty;
                    for (int i = 0; i < length; i++)
                    {
                        t_key = r.GetUInt8();
                        t_value = r.GetString(128);
                        p_x24[t_key] = t_value;
                    }
                }
                {
                    p_x25 = SmallMessageElement.Formatter.Deserialize(r, rank + 1);
                }
                {
                    p_x26 = MessageElement.Formatter.Deserialize(r, rank + 1);
                }
                return new SmallMessage(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

    internal sealed partial class Message : Omnix.Serialization.RocketPack.RocketPackMessageBase<Message>, System.IDisposable
    {
        static Message()
        {
            Message.Formatter = new CustomFormatter();
            Message.Empty = new Message(false, 0, 0, 0, 0, 0, 0, 0, 0, (Enum1)0, (Enum2)0, (Enum3)0, (Enum4)0, (Enum5)0, (Enum6)0, (Enum7)0, (Enum8)0, 0.0F, 0.0D, string.Empty, Omnix.Serialization.RocketPack.Timestamp.Zero, System.ReadOnlyMemory<byte>.Empty, Omnix.Base.SimpleMemoryOwner<byte>.Empty, System.Array.Empty<string>(), new System.Collections.Generic.Dictionary<byte, string>(), SmallMessageElement.Empty, MessageElement.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public Message(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, Omnix.Serialization.RocketPack.Timestamp x20, System.ReadOnlyMemory<byte> x21, System.Buffers.IMemoryOwner<byte> x22, string[] x23, System.Collections.Generic.Dictionary<byte, string> x24, SmallMessageElement x25, MessageElement x26)
        {
            if (x19 is null) throw new System.ArgumentNullException("x19");
            if (x19.Length > 128) throw new System.ArgumentOutOfRangeException("x19");
            if (x21.Length > 256) throw new System.ArgumentOutOfRangeException("x21");
            if (x22 is null) throw new System.ArgumentNullException("x22");
            if (x22.Memory.Length > 256) throw new System.ArgumentOutOfRangeException("x22");
            if (x23 is null) throw new System.ArgumentNullException("x23");
            if (x23.Length > 16) throw new System.ArgumentOutOfRangeException("x23");
            foreach (var n in x23)
            {
                if (n is null) throw new System.ArgumentNullException("n");
                if (n.Length > 128) throw new System.ArgumentOutOfRangeException("n");
            }
            if (x24 is null) throw new System.ArgumentNullException("x24");
            if (x24.Count > 32) throw new System.ArgumentOutOfRangeException("x24");
            foreach (var n in x24)
            {
                if (n.Value is null) throw new System.ArgumentNullException("n.Value");
                if (n.Value.Length > 128) throw new System.ArgumentOutOfRangeException("n.Value");
            }
            if (x26 is null) throw new System.ArgumentNullException("x26");

            this.X0 = x0;
            this.X1 = x1;
            this.X2 = x2;
            this.X3 = x3;
            this.X4 = x4;
            this.X5 = x5;
            this.X6 = x6;
            this.X7 = x7;
            this.X8 = x8;
            this.X9 = x9;
            this.X10 = x10;
            this.X11 = x11;
            this.X12 = x12;
            this.X13 = x13;
            this.X14 = x14;
            this.X15 = x15;
            this.X16 = x16;
            this.X17 = x17;
            this.X18 = x18;
            this.X19 = x19;
            this.X20 = x20;
            this.X21 = x21;
            _x22 = x22;
            this.X23 = new Omnix.Collections.ReadOnlyListSlim<string>(x23);
            this.X24 = new Omnix.Collections.ReadOnlyDictionarySlim<byte, string>(x24);
            this.X25 = x25;
            this.X26 = x26;

            {
                var __h = new System.HashCode();
                if (this.X0 != default) __h.Add(this.X0.GetHashCode());
                if (this.X1 != default) __h.Add(this.X1.GetHashCode());
                if (this.X2 != default) __h.Add(this.X2.GetHashCode());
                if (this.X3 != default) __h.Add(this.X3.GetHashCode());
                if (this.X4 != default) __h.Add(this.X4.GetHashCode());
                if (this.X5 != default) __h.Add(this.X5.GetHashCode());
                if (this.X6 != default) __h.Add(this.X6.GetHashCode());
                if (this.X7 != default) __h.Add(this.X7.GetHashCode());
                if (this.X8 != default) __h.Add(this.X8.GetHashCode());
                if (this.X9 != default) __h.Add(this.X9.GetHashCode());
                if (this.X10 != default) __h.Add(this.X10.GetHashCode());
                if (this.X11 != default) __h.Add(this.X11.GetHashCode());
                if (this.X12 != default) __h.Add(this.X12.GetHashCode());
                if (this.X13 != default) __h.Add(this.X13.GetHashCode());
                if (this.X14 != default) __h.Add(this.X14.GetHashCode());
                if (this.X15 != default) __h.Add(this.X15.GetHashCode());
                if (this.X16 != default) __h.Add(this.X16.GetHashCode());
                if (this.X17 != default) __h.Add(this.X17.GetHashCode());
                if (this.X18 != default) __h.Add(this.X18.GetHashCode());
                if (this.X19 != default) __h.Add(this.X19.GetHashCode());
                if (this.X20 != default) __h.Add(this.X20.GetHashCode());
                if (!this.X21.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X21.Span));
                if (!this.X22.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X22.Span));
                foreach (var n in this.X23)
                {
                    if (n != default) __h.Add(n.GetHashCode());
                }
                foreach (var n in this.X24)
                {
                    if (n.Key != default) __h.Add(n.Key.GetHashCode());
                    if (n.Value != default) __h.Add(n.Value.GetHashCode());
                }
                if (this.X25 != default) __h.Add(this.X25.GetHashCode());
                if (this.X26 != default) __h.Add(this.X26.GetHashCode());
                __hashCode = __h.ToHashCode();
            }
        }

        public bool X0 { get; }
        public sbyte X1 { get; }
        public short X2 { get; }
        public int X3 { get; }
        public long X4 { get; }
        public byte X5 { get; }
        public ushort X6 { get; }
        public uint X7 { get; }
        public ulong X8 { get; }
        public Enum1 X9 { get; }
        public Enum2 X10 { get; }
        public Enum3 X11 { get; }
        public Enum4 X12 { get; }
        public Enum5 X13 { get; }
        public Enum6 X14 { get; }
        public Enum7 X15 { get; }
        public Enum8 X16 { get; }
        public float X17 { get; }
        public double X18 { get; }
        public string X19 { get; }
        public Omnix.Serialization.RocketPack.Timestamp X20 { get; }
        public System.ReadOnlyMemory<byte> X21 { get; }
        private readonly System.Buffers.IMemoryOwner<byte> _x22;
        public System.ReadOnlyMemory<byte> X22 => _x22.Memory;
        public Omnix.Collections.ReadOnlyListSlim<string> X23 { get; }
        public Omnix.Collections.ReadOnlyDictionarySlim<byte, string> X24 { get; }
        public SmallMessageElement X25 { get; }
        public MessageElement X26 { get; }

        public override bool Equals(Message? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.X0 != target.X0) return false;
            if (this.X1 != target.X1) return false;
            if (this.X2 != target.X2) return false;
            if (this.X3 != target.X3) return false;
            if (this.X4 != target.X4) return false;
            if (this.X5 != target.X5) return false;
            if (this.X6 != target.X6) return false;
            if (this.X7 != target.X7) return false;
            if (this.X8 != target.X8) return false;
            if (this.X9 != target.X9) return false;
            if (this.X10 != target.X10) return false;
            if (this.X11 != target.X11) return false;
            if (this.X12 != target.X12) return false;
            if (this.X13 != target.X13) return false;
            if (this.X14 != target.X14) return false;
            if (this.X15 != target.X15) return false;
            if (this.X16 != target.X16) return false;
            if (this.X17 != target.X17) return false;
            if (this.X19 != target.X19) return false;
            if (this.X20 != target.X20) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.X21.Span, target.X21.Span)) return false;
            if (!Omnix.Base.BytesOperations.SequenceEqual(this.X22.Span, target.X22.Span)) return false;
            if (!Omnix.Base.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if (!Omnix.Base.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<Message>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, Message value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.X0 != false)
                    {
                        propertyCount++;
                    }
                    if (value.X1 != 0)
                    {
                        propertyCount++;
                    }
                    if (value.X2 != 0)
                    {
                        propertyCount++;
                    }
                    if (value.X3 != 0)
                    {
                        propertyCount++;
                    }
                    if (value.X4 != 0)
                    {
                        propertyCount++;
                    }
                    if (value.X5 != 0)
                    {
                        propertyCount++;
                    }
                    if (value.X6 != 0)
                    {
                        propertyCount++;
                    }
                    if (value.X7 != 0)
                    {
                        propertyCount++;
                    }
                    if (value.X8 != 0)
                    {
                        propertyCount++;
                    }
                    if (value.X9 != (Enum1)0)
                    {
                        propertyCount++;
                    }
                    if (value.X10 != (Enum2)0)
                    {
                        propertyCount++;
                    }
                    if (value.X11 != (Enum3)0)
                    {
                        propertyCount++;
                    }
                    if (value.X12 != (Enum4)0)
                    {
                        propertyCount++;
                    }
                    if (value.X13 != (Enum5)0)
                    {
                        propertyCount++;
                    }
                    if (value.X14 != (Enum6)0)
                    {
                        propertyCount++;
                    }
                    if (value.X15 != (Enum7)0)
                    {
                        propertyCount++;
                    }
                    if (value.X16 != (Enum8)0)
                    {
                        propertyCount++;
                    }
                    if (value.X17 != 0.0F)
                    {
                        propertyCount++;
                    }
                    if (value.X18 != 0.0D)
                    {
                        propertyCount++;
                    }
                    if (value.X19 != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X20 != Omnix.Serialization.RocketPack.Timestamp.Zero)
                    {
                        propertyCount++;
                    }
                    if (!value.X21.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.X22.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (value.X23.Count != 0)
                    {
                        propertyCount++;
                    }
                    if (value.X24.Count != 0)
                    {
                        propertyCount++;
                    }
                    if (value.X25 != SmallMessageElement.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X26 != MessageElement.Empty)
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
                if (value.X1 != 0)
                {
                    w.Write((uint)1);
                    w.Write(value.X1);
                }
                if (value.X2 != 0)
                {
                    w.Write((uint)2);
                    w.Write(value.X2);
                }
                if (value.X3 != 0)
                {
                    w.Write((uint)3);
                    w.Write(value.X3);
                }
                if (value.X4 != 0)
                {
                    w.Write((uint)4);
                    w.Write(value.X4);
                }
                if (value.X5 != 0)
                {
                    w.Write((uint)5);
                    w.Write(value.X5);
                }
                if (value.X6 != 0)
                {
                    w.Write((uint)6);
                    w.Write(value.X6);
                }
                if (value.X7 != 0)
                {
                    w.Write((uint)7);
                    w.Write(value.X7);
                }
                if (value.X8 != 0)
                {
                    w.Write((uint)8);
                    w.Write(value.X8);
                }
                if (value.X9 != (Enum1)0)
                {
                    w.Write((uint)9);
                    w.Write((long)value.X9);
                }
                if (value.X10 != (Enum2)0)
                {
                    w.Write((uint)10);
                    w.Write((long)value.X10);
                }
                if (value.X11 != (Enum3)0)
                {
                    w.Write((uint)11);
                    w.Write((long)value.X11);
                }
                if (value.X12 != (Enum4)0)
                {
                    w.Write((uint)12);
                    w.Write((long)value.X12);
                }
                if (value.X13 != (Enum5)0)
                {
                    w.Write((uint)13);
                    w.Write((ulong)value.X13);
                }
                if (value.X14 != (Enum6)0)
                {
                    w.Write((uint)14);
                    w.Write((ulong)value.X14);
                }
                if (value.X15 != (Enum7)0)
                {
                    w.Write((uint)15);
                    w.Write((ulong)value.X15);
                }
                if (value.X16 != (Enum8)0)
                {
                    w.Write((uint)16);
                    w.Write((ulong)value.X16);
                }
                if (value.X17 != 0.0F)
                {
                    w.Write((uint)17);
                    w.Write(value.X17);
                }
                if (value.X18 != 0.0D)
                {
                    w.Write((uint)18);
                    w.Write(value.X18);
                }
                if (value.X19 != string.Empty)
                {
                    w.Write((uint)19);
                    w.Write(value.X19);
                }
                if (value.X20 != Omnix.Serialization.RocketPack.Timestamp.Zero)
                {
                    w.Write((uint)20);
                    w.Write(value.X20);
                }
                if (!value.X21.IsEmpty)
                {
                    w.Write((uint)21);
                    w.Write(value.X21.Span);
                }
                if (!value.X22.IsEmpty)
                {
                    w.Write((uint)22);
                    w.Write(value.X22.Span);
                }
                if (value.X23.Count != 0)
                {
                    w.Write((uint)23);
                    w.Write((uint)value.X23.Count);
                    foreach (var n in value.X23)
                    {
                        w.Write(n);
                    }
                }
                if (value.X24.Count != 0)
                {
                    w.Write((uint)24);
                    w.Write((uint)value.X24.Count);
                    foreach (var n in value.X24)
                    {
                        w.Write(n.Key);
                        w.Write(n.Value);
                    }
                }
                if (value.X25 != SmallMessageElement.Empty)
                {
                    w.Write((uint)25);
                    SmallMessageElement.Formatter.Serialize(w, value.X25, rank + 1);
                }
                if (value.X26 != MessageElement.Empty)
                {
                    w.Write((uint)26);
                    MessageElement.Formatter.Serialize(w, value.X26, rank + 1);
                }
            }

            public Message Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                uint propertyCount = r.GetUInt32();

                bool p_x0 = false;
                sbyte p_x1 = 0;
                short p_x2 = 0;
                int p_x3 = 0;
                long p_x4 = 0;
                byte p_x5 = 0;
                ushort p_x6 = 0;
                uint p_x7 = 0;
                ulong p_x8 = 0;
                Enum1 p_x9 = (Enum1)0;
                Enum2 p_x10 = (Enum2)0;
                Enum3 p_x11 = (Enum3)0;
                Enum4 p_x12 = (Enum4)0;
                Enum5 p_x13 = (Enum5)0;
                Enum6 p_x14 = (Enum6)0;
                Enum7 p_x15 = (Enum7)0;
                Enum8 p_x16 = (Enum8)0;
                float p_x17 = 0.0F;
                double p_x18 = 0.0D;
                string p_x19 = string.Empty;
                Omnix.Serialization.RocketPack.Timestamp p_x20 = Omnix.Serialization.RocketPack.Timestamp.Zero;
                System.ReadOnlyMemory<byte> p_x21 = System.ReadOnlyMemory<byte>.Empty;
                System.Buffers.IMemoryOwner<byte> p_x22 = Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                string[] p_x23 = System.Array.Empty<string>();
                System.Collections.Generic.Dictionary<byte, string> p_x24 = new System.Collections.Generic.Dictionary<byte, string>();
                SmallMessageElement p_x25 = SmallMessageElement.Empty;
                MessageElement p_x26 = MessageElement.Empty;

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
                        case 1:
                            {
                                p_x1 = r.GetInt8();
                                break;
                            }
                        case 2:
                            {
                                p_x2 = r.GetInt16();
                                break;
                            }
                        case 3:
                            {
                                p_x3 = r.GetInt32();
                                break;
                            }
                        case 4:
                            {
                                p_x4 = r.GetInt64();
                                break;
                            }
                        case 5:
                            {
                                p_x5 = r.GetUInt8();
                                break;
                            }
                        case 6:
                            {
                                p_x6 = r.GetUInt16();
                                break;
                            }
                        case 7:
                            {
                                p_x7 = r.GetUInt32();
                                break;
                            }
                        case 8:
                            {
                                p_x8 = r.GetUInt64();
                                break;
                            }
                        case 9:
                            {
                                p_x9 = (Enum1)r.GetInt64();
                                break;
                            }
                        case 10:
                            {
                                p_x10 = (Enum2)r.GetInt64();
                                break;
                            }
                        case 11:
                            {
                                p_x11 = (Enum3)r.GetInt64();
                                break;
                            }
                        case 12:
                            {
                                p_x12 = (Enum4)r.GetInt64();
                                break;
                            }
                        case 13:
                            {
                                p_x13 = (Enum5)r.GetUInt64();
                                break;
                            }
                        case 14:
                            {
                                p_x14 = (Enum6)r.GetUInt64();
                                break;
                            }
                        case 15:
                            {
                                p_x15 = (Enum7)r.GetUInt64();
                                break;
                            }
                        case 16:
                            {
                                p_x16 = (Enum8)r.GetUInt64();
                                break;
                            }
                        case 17:
                            {
                                p_x17 = r.GetFloat32();
                                break;
                            }
                        case 18:
                            {
                                p_x18 = r.GetFloat64();
                                break;
                            }
                        case 19:
                            {
                                p_x19 = r.GetString(128);
                                break;
                            }
                        case 20:
                            {
                                p_x20 = r.GetTimestamp();
                                break;
                            }
                        case 21:
                            {
                                p_x21 = r.GetMemory(256);
                                break;
                            }
                        case 22:
                            {
                                p_x22 = r.GetRecyclableMemory(256);
                                break;
                            }
                        case 23:
                            {
                                var length = r.GetUInt32();
                                p_x23 = new string[length];
                                for (int i = 0; i < p_x23.Length; i++)
                                {
                                    p_x23[i] = r.GetString(128);
                                }
                                break;
                            }
                        case 24:
                            {
                                var length = r.GetUInt32();
                                p_x24 = new System.Collections.Generic.Dictionary<byte, string>();
                                byte t_key = 0;
                                string t_value = string.Empty;
                                for (int i = 0; i < length; i++)
                                {
                                    t_key = r.GetUInt8();
                                    t_value = r.GetString(128);
                                    p_x24[t_key] = t_value;
                                }
                                break;
                            }
                        case 25:
                            {
                                p_x25 = SmallMessageElement.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 26:
                            {
                                p_x26 = MessageElement.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                    }
                }

                return new Message(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

    internal sealed partial class NullableMessage : Omnix.Serialization.RocketPack.RocketPackMessageBase<NullableMessage>, System.IDisposable
    {
        static NullableMessage()
        {
            NullableMessage.Formatter = new CustomFormatter();
            NullableMessage.Empty = new NullableMessage(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        private readonly int __hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public NullableMessage(bool? x0, sbyte? x1, short? x2, int? x3, long? x4, byte? x5, ushort? x6, uint? x7, ulong? x8, Enum1? x9, Enum2? x10, Enum3? x11, Enum4? x12, Enum5? x13, Enum6? x14, Enum7? x15, Enum8? x16, float? x17, double? x18, string? x19, Omnix.Serialization.RocketPack.Timestamp? x20, System.ReadOnlyMemory<byte>? x21, System.Buffers.IMemoryOwner<byte>? x22, string[]? x23, System.Collections.Generic.Dictionary<byte, string>? x24, SmallMessageElement? x25, MessageElement? x26)
        {
            if (!(x19 is null) && x19.Length > 128) throw new System.ArgumentOutOfRangeException("x19");
            if (!(x21 is null) && x21.Value.Length > 256) throw new System.ArgumentOutOfRangeException("x21");
            if (!(x22 is null) && x22.Memory.Length > 256) throw new System.ArgumentOutOfRangeException("x22");
            if (!(x23 is null) && x23.Length > 16) throw new System.ArgumentOutOfRangeException("x23");
            if (!(x23 is null))
            {
                foreach (var n in x23)
                {
                    if (n is null) throw new System.ArgumentNullException("n");
                    if (n.Length > 128) throw new System.ArgumentOutOfRangeException("n");
                }
            }
            if (!(x24 is null) && x24.Count > 32) throw new System.ArgumentOutOfRangeException("x24");
            if (!(x24 is null))
            {
                foreach (var n in x24)
                {
                    if (n.Value is null) throw new System.ArgumentNullException("n.Value");
                    if (n.Value.Length > 128) throw new System.ArgumentOutOfRangeException("n.Value");
                }
            }
            this.X0 = x0;
            this.X1 = x1;
            this.X2 = x2;
            this.X3 = x3;
            this.X4 = x4;
            this.X5 = x5;
            this.X6 = x6;
            this.X7 = x7;
            this.X8 = x8;
            this.X9 = x9;
            this.X10 = x10;
            this.X11 = x11;
            this.X12 = x12;
            this.X13 = x13;
            this.X14 = x14;
            this.X15 = x15;
            this.X16 = x16;
            this.X17 = x17;
            this.X18 = x18;
            this.X19 = x19;
            this.X20 = x20;
            this.X21 = x21;
            _x22 = x22;
            if(x23 != null)
            {
                this.X23 = new Omnix.Collections.ReadOnlyListSlim<string>(x23);
            }
            if(x24 != null)
            {
                this.X24 = new Omnix.Collections.ReadOnlyDictionarySlim<byte, string>(x24);
            }
            this.X25 = x25;
            this.X26 = x26;

            {
                var __h = new System.HashCode();
                if (this.X0 != default) __h.Add(this.X0.GetHashCode());
                if (this.X1 != default) __h.Add(this.X1.GetHashCode());
                if (this.X2 != default) __h.Add(this.X2.GetHashCode());
                if (this.X3 != default) __h.Add(this.X3.GetHashCode());
                if (this.X4 != default) __h.Add(this.X4.GetHashCode());
                if (this.X5 != default) __h.Add(this.X5.GetHashCode());
                if (this.X6 != default) __h.Add(this.X6.GetHashCode());
                if (this.X7 != default) __h.Add(this.X7.GetHashCode());
                if (this.X8 != default) __h.Add(this.X8.GetHashCode());
                if (this.X9 != default) __h.Add(this.X9.GetHashCode());
                if (this.X10 != default) __h.Add(this.X10.GetHashCode());
                if (this.X11 != default) __h.Add(this.X11.GetHashCode());
                if (this.X12 != default) __h.Add(this.X12.GetHashCode());
                if (this.X13 != default) __h.Add(this.X13.GetHashCode());
                if (this.X14 != default) __h.Add(this.X14.GetHashCode());
                if (this.X15 != default) __h.Add(this.X15.GetHashCode());
                if (this.X16 != default) __h.Add(this.X16.GetHashCode());
                if (this.X17 != default) __h.Add(this.X17.GetHashCode());
                if (this.X18 != default) __h.Add(this.X18.GetHashCode());
                if (this.X19 != default) __h.Add(this.X19.GetHashCode());
                if (this.X20 != default) __h.Add(this.X20.GetHashCode());
                if (!(this.X21 is null) && !this.X21.Value.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X21.Value.Span));
                if (!(this.X22 is null) && !this.X22.Value.IsEmpty) __h.Add(Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X22.Value.Span));
                if(this.X23 != null)
                {
                    foreach (var n in this.X23)
                    {
                        if (n != default) __h.Add(n.GetHashCode());
                    }
                }
                if(this.X24 != null)
                {
                    foreach (var n in this.X24)
                    {
                        if (n.Key != default) __h.Add(n.Key.GetHashCode());
                        if (n.Value != default) __h.Add(n.Value.GetHashCode());
                    }
                }
                if (this.X25 != default) __h.Add(this.X25.GetHashCode());
                if (this.X26 != default) __h.Add(this.X26.GetHashCode());
                __hashCode = __h.ToHashCode();
            }
        }

        public bool? X0 { get; }
        public sbyte? X1 { get; }
        public short? X2 { get; }
        public int? X3 { get; }
        public long? X4 { get; }
        public byte? X5 { get; }
        public ushort? X6 { get; }
        public uint? X7 { get; }
        public ulong? X8 { get; }
        public Enum1? X9 { get; }
        public Enum2? X10 { get; }
        public Enum3? X11 { get; }
        public Enum4? X12 { get; }
        public Enum5? X13 { get; }
        public Enum6? X14 { get; }
        public Enum7? X15 { get; }
        public Enum8? X16 { get; }
        public float? X17 { get; }
        public double? X18 { get; }
        public string? X19 { get; }
        public Omnix.Serialization.RocketPack.Timestamp? X20 { get; }
        public System.ReadOnlyMemory<byte>? X21 { get; }
        private readonly System.Buffers.IMemoryOwner<byte>? _x22;
        public System.ReadOnlyMemory<byte>? X22 => _x22?.Memory;
        public Omnix.Collections.ReadOnlyListSlim<string>? X23 { get; }
        public Omnix.Collections.ReadOnlyDictionarySlim<byte, string>? X24 { get; }
        public SmallMessageElement? X25 { get; }
        public MessageElement? X26 { get; }

        public override bool Equals(NullableMessage? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.X0 != target.X0) return false;
            if (this.X1 != target.X1) return false;
            if (this.X2 != target.X2) return false;
            if (this.X3 != target.X3) return false;
            if (this.X4 != target.X4) return false;
            if (this.X5 != target.X5) return false;
            if (this.X6 != target.X6) return false;
            if (this.X7 != target.X7) return false;
            if (this.X8 != target.X8) return false;
            if (this.X9 != target.X9) return false;
            if (this.X10 != target.X10) return false;
            if (this.X11 != target.X11) return false;
            if (this.X12 != target.X12) return false;
            if (this.X13 != target.X13) return false;
            if (this.X14 != target.X14) return false;
            if (this.X15 != target.X15) return false;
            if (this.X16 != target.X16) return false;
            if (this.X17 != target.X17) return false;
            if (this.X19 != target.X19) return false;
            if (this.X20 != target.X20) return false;
            if ((this.X21 is null) != (target.X21 is null)) return false;
            if (!(this.X21 is null) && !(target.X21 is null) && !Omnix.Base.BytesOperations.SequenceEqual(this.X21.Value.Span, target.X21.Value.Span)) return false;
            if ((this.X22 is null) != (target.X22 is null)) return false;
            if (!(this.X22 is null) && !(target.X22 is null) && !Omnix.Base.BytesOperations.SequenceEqual(this.X22.Value.Span, target.X22.Value.Span)) return false;
            if ((this.X23 is null) != (target.X23 is null)) return false;
            if (!(this.X23 is null) && !(target.X23 is null) && !Omnix.Base.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if ((this.X24 is null) != (target.X24 is null)) return false;
            if (!(this.X24 is null) && !(target.X24 is null) && !Omnix.Base.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if ((this.X25 is null) != (target.X25 is null)) return false;
            if (!(this.X25 is null) && !(target.X25 is null) && this.X25 != target.X25) return false;
            if ((this.X26 is null) != (target.X26 is null)) return false;
            if (!(this.X26 is null) && !(target.X26 is null) && this.X26 != target.X26) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class CustomFormatter : Omnix.Serialization.RocketPack.IRocketPackFormatter<NullableMessage>
        {
            public void Serialize(Omnix.Serialization.RocketPack.RocketPackWriter w, NullableMessage value, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.X0 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X1 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X2 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X3 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X4 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X5 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X6 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X7 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X8 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X9 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X10 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X11 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X12 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X13 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X14 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X15 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X16 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X17 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X18 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X19 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X20 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X21 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X22 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X23 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X24 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X25 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X26 != null)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.X0 != null)
                {
                    w.Write((uint)0);
                    w.Write(value.X0.Value);
                }
                if (value.X1 != null)
                {
                    w.Write((uint)1);
                    w.Write(value.X1.Value);
                }
                if (value.X2 != null)
                {
                    w.Write((uint)2);
                    w.Write(value.X2.Value);
                }
                if (value.X3 != null)
                {
                    w.Write((uint)3);
                    w.Write(value.X3.Value);
                }
                if (value.X4 != null)
                {
                    w.Write((uint)4);
                    w.Write(value.X4.Value);
                }
                if (value.X5 != null)
                {
                    w.Write((uint)5);
                    w.Write(value.X5.Value);
                }
                if (value.X6 != null)
                {
                    w.Write((uint)6);
                    w.Write(value.X6.Value);
                }
                if (value.X7 != null)
                {
                    w.Write((uint)7);
                    w.Write(value.X7.Value);
                }
                if (value.X8 != null)
                {
                    w.Write((uint)8);
                    w.Write(value.X8.Value);
                }
                if (value.X9 != null)
                {
                    w.Write((uint)9);
                    w.Write((long)value.X9);
                }
                if (value.X10 != null)
                {
                    w.Write((uint)10);
                    w.Write((long)value.X10);
                }
                if (value.X11 != null)
                {
                    w.Write((uint)11);
                    w.Write((long)value.X11);
                }
                if (value.X12 != null)
                {
                    w.Write((uint)12);
                    w.Write((long)value.X12);
                }
                if (value.X13 != null)
                {
                    w.Write((uint)13);
                    w.Write((ulong)value.X13);
                }
                if (value.X14 != null)
                {
                    w.Write((uint)14);
                    w.Write((ulong)value.X14);
                }
                if (value.X15 != null)
                {
                    w.Write((uint)15);
                    w.Write((ulong)value.X15);
                }
                if (value.X16 != null)
                {
                    w.Write((uint)16);
                    w.Write((ulong)value.X16);
                }
                if (value.X17 != null)
                {
                    w.Write((uint)17);
                    w.Write(value.X17.Value);
                }
                if (value.X18 != null)
                {
                    w.Write((uint)18);
                    w.Write(value.X18.Value);
                }
                if (value.X19 != null)
                {
                    w.Write((uint)19);
                    w.Write(value.X19);
                }
                if (value.X20 != null)
                {
                    w.Write((uint)20);
                    w.Write(value.X20.Value);
                }
                if (value.X21 != null)
                {
                    w.Write((uint)21);
                    w.Write(value.X21.Value.Span);
                }
                if (value.X22 != null)
                {
                    w.Write((uint)22);
                    w.Write(value.X22.Value.Span);
                }
                if (value.X23 != null)
                {
                    w.Write((uint)23);
                    w.Write((uint)value.X23.Count);
                    foreach (var n in value.X23)
                    {
                        w.Write(n);
                    }
                }
                if (value.X24 != null)
                {
                    w.Write((uint)24);
                    w.Write((uint)value.X24.Count);
                    foreach (var n in value.X24)
                    {
                        w.Write(n.Key);
                        w.Write(n.Value);
                    }
                }
                if (value.X25 != null)
                {
                    w.Write((uint)25);
                    SmallMessageElement.Formatter.Serialize(w, value.X25.Value, rank + 1);
                }
                if (value.X26 != null)
                {
                    w.Write((uint)26);
                    MessageElement.Formatter.Serialize(w, value.X26, rank + 1);
                }
            }

            public NullableMessage Deserialize(Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new System.FormatException();

                uint propertyCount = r.GetUInt32();

                bool? p_x0 = null;
                sbyte? p_x1 = null;
                short? p_x2 = null;
                int? p_x3 = null;
                long? p_x4 = null;
                byte? p_x5 = null;
                ushort? p_x6 = null;
                uint? p_x7 = null;
                ulong? p_x8 = null;
                Enum1? p_x9 = null;
                Enum2? p_x10 = null;
                Enum3? p_x11 = null;
                Enum4? p_x12 = null;
                Enum5? p_x13 = null;
                Enum6? p_x14 = null;
                Enum7? p_x15 = null;
                Enum8? p_x16 = null;
                float? p_x17 = null;
                double? p_x18 = null;
                string? p_x19 = null;
                Omnix.Serialization.RocketPack.Timestamp? p_x20 = null;
                System.ReadOnlyMemory<byte>? p_x21 = null;
                System.Buffers.IMemoryOwner<byte>? p_x22 = null;
                string[]? p_x23 = null;
                System.Collections.Generic.Dictionary<byte, string>? p_x24 = null;
                SmallMessageElement? p_x25 = null;
                MessageElement? p_x26 = null;

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
                        case 1:
                            {
                                p_x1 = r.GetInt8();
                                break;
                            }
                        case 2:
                            {
                                p_x2 = r.GetInt16();
                                break;
                            }
                        case 3:
                            {
                                p_x3 = r.GetInt32();
                                break;
                            }
                        case 4:
                            {
                                p_x4 = r.GetInt64();
                                break;
                            }
                        case 5:
                            {
                                p_x5 = r.GetUInt8();
                                break;
                            }
                        case 6:
                            {
                                p_x6 = r.GetUInt16();
                                break;
                            }
                        case 7:
                            {
                                p_x7 = r.GetUInt32();
                                break;
                            }
                        case 8:
                            {
                                p_x8 = r.GetUInt64();
                                break;
                            }
                        case 9:
                            {
                                p_x9 = (Enum1)r.GetInt64();
                                break;
                            }
                        case 10:
                            {
                                p_x10 = (Enum2)r.GetInt64();
                                break;
                            }
                        case 11:
                            {
                                p_x11 = (Enum3)r.GetInt64();
                                break;
                            }
                        case 12:
                            {
                                p_x12 = (Enum4)r.GetInt64();
                                break;
                            }
                        case 13:
                            {
                                p_x13 = (Enum5)r.GetUInt64();
                                break;
                            }
                        case 14:
                            {
                                p_x14 = (Enum6)r.GetUInt64();
                                break;
                            }
                        case 15:
                            {
                                p_x15 = (Enum7)r.GetUInt64();
                                break;
                            }
                        case 16:
                            {
                                p_x16 = (Enum8)r.GetUInt64();
                                break;
                            }
                        case 17:
                            {
                                p_x17 = r.GetFloat32();
                                break;
                            }
                        case 18:
                            {
                                p_x18 = r.GetFloat64();
                                break;
                            }
                        case 19:
                            {
                                p_x19 = r.GetString(128);
                                break;
                            }
                        case 20:
                            {
                                p_x20 = r.GetTimestamp();
                                break;
                            }
                        case 21:
                            {
                                p_x21 = r.GetMemory(256);
                                break;
                            }
                        case 22:
                            {
                                p_x22 = r.GetRecyclableMemory(256);
                                break;
                            }
                        case 23:
                            {
                                var length = r.GetUInt32();
                                p_x23 = new string[length];
                                for (int i = 0; i < p_x23.Length; i++)
                                {
                                    p_x23[i] = r.GetString(128);
                                }
                                break;
                            }
                        case 24:
                            {
                                var length = r.GetUInt32();
                                p_x24 = new System.Collections.Generic.Dictionary<byte, string>();
                                byte t_key = 0;
                                string t_value = string.Empty;
                                for (int i = 0; i < length; i++)
                                {
                                    t_key = r.GetUInt8();
                                    t_value = r.GetString(128);
                                    p_x24[t_key] = t_value;
                                }
                                break;
                            }
                        case 25:
                            {
                                p_x25 = SmallMessageElement.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                        case 26:
                            {
                                p_x26 = MessageElement.Formatter.Deserialize(r, rank + 1);
                                break;
                            }
                    }
                }

                return new NullableMessage(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

}
