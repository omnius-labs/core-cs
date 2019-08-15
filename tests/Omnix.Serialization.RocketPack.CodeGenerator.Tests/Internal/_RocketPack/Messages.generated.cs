
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

    internal readonly struct SmallStructMessage : global::Omnix.Serialization.RocketPack.IRocketPackMessage<SmallStructMessage>, global::System.IDisposable
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallStructMessage> Formatter { get; }
        public static SmallStructMessage Empty { get; }

        static SmallStructMessage()
        {
            SmallStructMessage.Formatter = new ___CustomFormatter();
            SmallStructMessage.Empty = new SmallStructMessage(false, 0, 0, 0, 0, 0, 0, 0, 0, (Enum1)0, (Enum2)0, (Enum3)0, (Enum4)0, (Enum5)0, (Enum6)0, (Enum7)0, (Enum8)0, 0.0F, 0.0D, string.Empty, global::Omnix.Serialization.RocketPack.Timestamp.Zero, global::System.ReadOnlyMemory<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::System.Array.Empty<string>(), new global::System.Collections.Generic.Dictionary<byte, string>(), SmallMessageStructElement.Empty, MessageStructElement.Empty);
        }

        private readonly int ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public SmallStructMessage(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, global::Omnix.Serialization.RocketPack.Timestamp x20, global::System.ReadOnlyMemory<byte> x21, global::System.Buffers.IMemoryOwner<byte> x22, string[] x23, global::System.Collections.Generic.Dictionary<byte, string> x24, SmallMessageStructElement x25, MessageStructElement x26)
        {
            if (x19 is null) throw new global::System.ArgumentNullException("x19");
            if (x19.Length > 128) throw new global::System.ArgumentOutOfRangeException("x19");
            if (x21.Length > 256) throw new global::System.ArgumentOutOfRangeException("x21");
            if (x22 is null) throw new global::System.ArgumentNullException("x22");
            if (x22.Memory.Length > 256) throw new global::System.ArgumentOutOfRangeException("x22");
            if (x23 is null) throw new global::System.ArgumentNullException("x23");
            if (x23.Length > 16) throw new global::System.ArgumentOutOfRangeException("x23");
            foreach (var n in x23)
            {
                if (n is null) throw new global::System.ArgumentNullException("n");
                if (n.Length > 128) throw new global::System.ArgumentOutOfRangeException("n");
            }
            if (x24 is null) throw new global::System.ArgumentNullException("x24");
            if (x24.Count > 32) throw new global::System.ArgumentOutOfRangeException("x24");
            foreach (var n in x24)
            {
                if (n.Value is null) throw new global::System.ArgumentNullException("n.Value");
                if (n.Value.Length > 128) throw new global::System.ArgumentOutOfRangeException("n.Value");
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
            this.X23 = new global::Omnix.DataStructures.ReadOnlyListSlim<string>(x23);
            this.X24 = new global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string>(x24);
            this.X25 = x25;
            this.X26 = x26;

            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                if (x1 != default) ___h.Add(x1.GetHashCode());
                if (x2 != default) ___h.Add(x2.GetHashCode());
                if (x3 != default) ___h.Add(x3.GetHashCode());
                if (x4 != default) ___h.Add(x4.GetHashCode());
                if (x5 != default) ___h.Add(x5.GetHashCode());
                if (x6 != default) ___h.Add(x6.GetHashCode());
                if (x7 != default) ___h.Add(x7.GetHashCode());
                if (x8 != default) ___h.Add(x8.GetHashCode());
                if (x9 != default) ___h.Add(x9.GetHashCode());
                if (x10 != default) ___h.Add(x10.GetHashCode());
                if (x11 != default) ___h.Add(x11.GetHashCode());
                if (x12 != default) ___h.Add(x12.GetHashCode());
                if (x13 != default) ___h.Add(x13.GetHashCode());
                if (x14 != default) ___h.Add(x14.GetHashCode());
                if (x15 != default) ___h.Add(x15.GetHashCode());
                if (x16 != default) ___h.Add(x16.GetHashCode());
                if (x17 != default) ___h.Add(x17.GetHashCode());
                if (x18 != default) ___h.Add(x18.GetHashCode());
                if (x19 != default) ___h.Add(x19.GetHashCode());
                if (x20 != default) ___h.Add(x20.GetHashCode());
                if (!x21.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x21.Span));
                if (!x22.Memory.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
                foreach (var n in x23)
                {
                    if (n != default) ___h.Add(n.GetHashCode());
                }
                foreach (var n in x24)
                {
                    if (n.Key != default) ___h.Add(n.Key.GetHashCode());
                    if (n.Value != default) ___h.Add(n.Value.GetHashCode());
                }
                if (x25 != default) ___h.Add(x25.GetHashCode());
                if (x26 != default) ___h.Add(x26.GetHashCode());
                ___hashCode = ___h.ToHashCode();
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
        public global::Omnix.Serialization.RocketPack.Timestamp X20 { get; }
        public global::System.ReadOnlyMemory<byte> X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte> _x22;
        public global::System.ReadOnlyMemory<byte> X22 => _x22.Memory;
        public global::Omnix.DataStructures.ReadOnlyListSlim<string> X23 { get; }
        public global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string> X24 { get; }
        public SmallMessageStructElement X25 { get; }
        public MessageStructElement X26 { get; }

        public static SmallStructMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SmallStructMessage left, SmallStructMessage right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(SmallStructMessage left, SmallStructMessage right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SmallStructMessage)) return false;
            return this.Equals((SmallStructMessage)other);
        }
        public bool Equals(SmallStructMessage target)
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
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X21.Span, target.X21.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X22.Span, target.X22.Span)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallStructMessage>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in SmallStructMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.X0);
                w.Write(value.X1);
                w.Write(value.X2);
                w.Write(value.X3);
                w.Write(value.X4);
                w.Write(value.X5);
                w.Write(value.X6);
                w.Write(value.X7);
                w.Write(value.X8);
                w.Write((long)value.X9);
                w.Write((long)value.X10);
                w.Write((long)value.X11);
                w.Write((long)value.X12);
                w.Write((ulong)value.X13);
                w.Write((ulong)value.X14);
                w.Write((ulong)value.X15);
                w.Write((ulong)value.X16);
                w.Write(value.X17);
                w.Write(value.X18);
                w.Write(value.X19);
                w.Write(value.X20);
                w.Write(value.X21.Span);
                w.Write(value.X22.Span);
                w.Write((uint)value.X23.Count);
                foreach (var n in value.X23)
                {
                    w.Write(n);
                }
                w.Write((uint)value.X24.Count);
                foreach (var n in value.X24)
                {
                    w.Write(n.Key);
                    w.Write(n.Value);
                }
                SmallMessageStructElement.Formatter.Serialize(ref w, value.X25, rank + 1);
                MessageStructElement.Formatter.Serialize(ref w, value.X26, rank + 1);
            }

            public SmallStructMessage Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                global::Omnix.Serialization.RocketPack.Timestamp p_x20 = global::Omnix.Serialization.RocketPack.Timestamp.Zero;
                global::System.ReadOnlyMemory<byte> p_x21 = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x22 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                string[] p_x23 = global::System.Array.Empty<string>();
                global::System.Collections.Generic.Dictionary<byte, string> p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
                SmallMessageStructElement p_x25 = SmallMessageStructElement.Empty;
                MessageStructElement p_x26 = MessageStructElement.Empty;

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
                    p_x22 = r.GetRecyclableMemory(256);
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
                    p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
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
                    p_x25 = SmallMessageStructElement.Formatter.Deserialize(ref r, rank + 1);
                }
                {
                    p_x26 = MessageStructElement.Formatter.Deserialize(ref r, rank + 1);
                }
                return new SmallStructMessage(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

    internal readonly struct MessageStruct : global::Omnix.Serialization.RocketPack.IRocketPackMessage<MessageStruct>, global::System.IDisposable
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<MessageStruct> Formatter { get; }
        public static MessageStruct Empty { get; }

        static MessageStruct()
        {
            MessageStruct.Formatter = new ___CustomFormatter();
            MessageStruct.Empty = new MessageStruct(false, 0, 0, 0, 0, 0, 0, 0, 0, (Enum1)0, (Enum2)0, (Enum3)0, (Enum4)0, (Enum5)0, (Enum6)0, (Enum7)0, (Enum8)0, 0.0F, 0.0D, string.Empty, global::Omnix.Serialization.RocketPack.Timestamp.Zero, global::System.ReadOnlyMemory<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::System.Array.Empty<string>(), new global::System.Collections.Generic.Dictionary<byte, string>(), SmallMessageStructElement.Empty, MessageStructElement.Empty);
        }

        private readonly int ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public MessageStruct(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, global::Omnix.Serialization.RocketPack.Timestamp x20, global::System.ReadOnlyMemory<byte> x21, global::System.Buffers.IMemoryOwner<byte> x22, string[] x23, global::System.Collections.Generic.Dictionary<byte, string> x24, SmallMessageStructElement x25, MessageStructElement x26)
        {
            if (x19 is null) throw new global::System.ArgumentNullException("x19");
            if (x19.Length > 128) throw new global::System.ArgumentOutOfRangeException("x19");
            if (x21.Length > 256) throw new global::System.ArgumentOutOfRangeException("x21");
            if (x22 is null) throw new global::System.ArgumentNullException("x22");
            if (x22.Memory.Length > 256) throw new global::System.ArgumentOutOfRangeException("x22");
            if (x23 is null) throw new global::System.ArgumentNullException("x23");
            if (x23.Length > 16) throw new global::System.ArgumentOutOfRangeException("x23");
            foreach (var n in x23)
            {
                if (n is null) throw new global::System.ArgumentNullException("n");
                if (n.Length > 128) throw new global::System.ArgumentOutOfRangeException("n");
            }
            if (x24 is null) throw new global::System.ArgumentNullException("x24");
            if (x24.Count > 32) throw new global::System.ArgumentOutOfRangeException("x24");
            foreach (var n in x24)
            {
                if (n.Value is null) throw new global::System.ArgumentNullException("n.Value");
                if (n.Value.Length > 128) throw new global::System.ArgumentOutOfRangeException("n.Value");
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
            this.X23 = new global::Omnix.DataStructures.ReadOnlyListSlim<string>(x23);
            this.X24 = new global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string>(x24);
            this.X25 = x25;
            this.X26 = x26;

            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                if (x1 != default) ___h.Add(x1.GetHashCode());
                if (x2 != default) ___h.Add(x2.GetHashCode());
                if (x3 != default) ___h.Add(x3.GetHashCode());
                if (x4 != default) ___h.Add(x4.GetHashCode());
                if (x5 != default) ___h.Add(x5.GetHashCode());
                if (x6 != default) ___h.Add(x6.GetHashCode());
                if (x7 != default) ___h.Add(x7.GetHashCode());
                if (x8 != default) ___h.Add(x8.GetHashCode());
                if (x9 != default) ___h.Add(x9.GetHashCode());
                if (x10 != default) ___h.Add(x10.GetHashCode());
                if (x11 != default) ___h.Add(x11.GetHashCode());
                if (x12 != default) ___h.Add(x12.GetHashCode());
                if (x13 != default) ___h.Add(x13.GetHashCode());
                if (x14 != default) ___h.Add(x14.GetHashCode());
                if (x15 != default) ___h.Add(x15.GetHashCode());
                if (x16 != default) ___h.Add(x16.GetHashCode());
                if (x17 != default) ___h.Add(x17.GetHashCode());
                if (x18 != default) ___h.Add(x18.GetHashCode());
                if (x19 != default) ___h.Add(x19.GetHashCode());
                if (x20 != default) ___h.Add(x20.GetHashCode());
                if (!x21.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x21.Span));
                if (!x22.Memory.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
                foreach (var n in x23)
                {
                    if (n != default) ___h.Add(n.GetHashCode());
                }
                foreach (var n in x24)
                {
                    if (n.Key != default) ___h.Add(n.Key.GetHashCode());
                    if (n.Value != default) ___h.Add(n.Value.GetHashCode());
                }
                if (x25 != default) ___h.Add(x25.GetHashCode());
                if (x26 != default) ___h.Add(x26.GetHashCode());
                ___hashCode = ___h.ToHashCode();
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
        public global::Omnix.Serialization.RocketPack.Timestamp X20 { get; }
        public global::System.ReadOnlyMemory<byte> X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte> _x22;
        public global::System.ReadOnlyMemory<byte> X22 => _x22.Memory;
        public global::Omnix.DataStructures.ReadOnlyListSlim<string> X23 { get; }
        public global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string> X24 { get; }
        public SmallMessageStructElement X25 { get; }
        public MessageStructElement X26 { get; }

        public static MessageStruct Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(MessageStruct left, MessageStruct right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(MessageStruct left, MessageStruct right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is MessageStruct)) return false;
            return this.Equals((MessageStruct)other);
        }
        public bool Equals(MessageStruct target)
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
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X21.Span, target.X21.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X22.Span, target.X22.Span)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<MessageStruct>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in MessageStruct value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                    if (value.X20 != global::Omnix.Serialization.RocketPack.Timestamp.Zero)
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
                    if (value.X25 != SmallMessageStructElement.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X26 != MessageStructElement.Empty)
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
                if (value.X20 != global::Omnix.Serialization.RocketPack.Timestamp.Zero)
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
                if (value.X25 != SmallMessageStructElement.Empty)
                {
                    w.Write((uint)25);
                    SmallMessageStructElement.Formatter.Serialize(ref w, value.X25, rank + 1);
                }
                if (value.X26 != MessageStructElement.Empty)
                {
                    w.Write((uint)26);
                    MessageStructElement.Formatter.Serialize(ref w, value.X26, rank + 1);
                }
            }

            public MessageStruct Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                global::Omnix.Serialization.RocketPack.Timestamp p_x20 = global::Omnix.Serialization.RocketPack.Timestamp.Zero;
                global::System.ReadOnlyMemory<byte> p_x21 = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x22 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                string[] p_x23 = global::System.Array.Empty<string>();
                global::System.Collections.Generic.Dictionary<byte, string> p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
                SmallMessageStructElement p_x25 = SmallMessageStructElement.Empty;
                MessageStructElement p_x26 = MessageStructElement.Empty;

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
                                p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
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
                                p_x25 = SmallMessageStructElement.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 26:
                            {
                                p_x26 = MessageStructElement.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                    }
                }

                return new MessageStruct(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

    internal readonly struct NullableMessageStruct : global::Omnix.Serialization.RocketPack.IRocketPackMessage<NullableMessageStruct>, global::System.IDisposable
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<NullableMessageStruct> Formatter { get; }
        public static NullableMessageStruct Empty { get; }

        static NullableMessageStruct()
        {
            NullableMessageStruct.Formatter = new ___CustomFormatter();
            NullableMessageStruct.Empty = new NullableMessageStruct(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        private readonly int ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public NullableMessageStruct(bool? x0, sbyte? x1, short? x2, int? x3, long? x4, byte? x5, ushort? x6, uint? x7, ulong? x8, Enum1? x9, Enum2? x10, Enum3? x11, Enum4? x12, Enum5? x13, Enum6? x14, Enum7? x15, Enum8? x16, float? x17, double? x18, string? x19, global::Omnix.Serialization.RocketPack.Timestamp? x20, global::System.ReadOnlyMemory<byte>? x21, global::System.Buffers.IMemoryOwner<byte>? x22, string[]? x23, global::System.Collections.Generic.Dictionary<byte, string>? x24, SmallMessageStructElement? x25, MessageStructElement? x26)
        {
            if (!(x19 is null) && x19.Length > 128) throw new global::System.ArgumentOutOfRangeException("x19");
            if (!(x21 is null) && x21.Value.Length > 256) throw new global::System.ArgumentOutOfRangeException("x21");
            if (!(x22 is null) && x22.Memory.Length > 256) throw new global::System.ArgumentOutOfRangeException("x22");
            if (!(x23 is null) && x23.Length > 16) throw new global::System.ArgumentOutOfRangeException("x23");
            if (!(x23 is null))
            {
                foreach (var n in x23)
                {
                    if (n is null) throw new global::System.ArgumentNullException("n");
                    if (n.Length > 128) throw new global::System.ArgumentOutOfRangeException("n");
                }
            }
            if (!(x24 is null) && x24.Count > 32) throw new global::System.ArgumentOutOfRangeException("x24");
            if (!(x24 is null))
            {
                foreach (var n in x24)
                {
                    if (n.Value is null) throw new global::System.ArgumentNullException("n.Value");
                    if (n.Value.Length > 128) throw new global::System.ArgumentOutOfRangeException("n.Value");
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
                this.X23 = new global::Omnix.DataStructures.ReadOnlyListSlim<string>(x23);
            }
            else
            {
                this.X23 = null;
            }
            if(x24 != null)
            {
                this.X24 = new global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string>(x24);
            }
            else
            {
                this.X24 = null;
            }
            this.X25 = x25;
            this.X26 = x26;

            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                if (x1 != default) ___h.Add(x1.GetHashCode());
                if (x2 != default) ___h.Add(x2.GetHashCode());
                if (x3 != default) ___h.Add(x3.GetHashCode());
                if (x4 != default) ___h.Add(x4.GetHashCode());
                if (x5 != default) ___h.Add(x5.GetHashCode());
                if (x6 != default) ___h.Add(x6.GetHashCode());
                if (x7 != default) ___h.Add(x7.GetHashCode());
                if (x8 != default) ___h.Add(x8.GetHashCode());
                if (x9 != default) ___h.Add(x9.GetHashCode());
                if (x10 != default) ___h.Add(x10.GetHashCode());
                if (x11 != default) ___h.Add(x11.GetHashCode());
                if (x12 != default) ___h.Add(x12.GetHashCode());
                if (x13 != default) ___h.Add(x13.GetHashCode());
                if (x14 != default) ___h.Add(x14.GetHashCode());
                if (x15 != default) ___h.Add(x15.GetHashCode());
                if (x16 != default) ___h.Add(x16.GetHashCode());
                if (x17 != default) ___h.Add(x17.GetHashCode());
                if (x18 != default) ___h.Add(x18.GetHashCode());
                if (x19 != default) ___h.Add(x19.GetHashCode());
                if (x20 != default) ___h.Add(x20.GetHashCode());
                if (!(x21 is null) && !x21.Value.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x21.Value.Span));
                if (!(x22 is null) && !x22.Memory.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
                if(x23 != null)
                {
                    foreach (var n in x23)
                    {
                        if (n != default) ___h.Add(n.GetHashCode());
                    }
                }
                if(x24 != null)
                {
                    foreach (var n in x24)
                    {
                        if (n.Key != default) ___h.Add(n.Key.GetHashCode());
                        if (n.Value != default) ___h.Add(n.Value.GetHashCode());
                    }
                }
                if (x25 != default) ___h.Add(x25.Value.GetHashCode());
                if (x26 != default) ___h.Add(x26.Value.GetHashCode());
                ___hashCode = ___h.ToHashCode();
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
        public global::Omnix.Serialization.RocketPack.Timestamp? X20 { get; }
        public global::System.ReadOnlyMemory<byte>? X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte>? _x22;
        public global::System.ReadOnlyMemory<byte>? X22 => _x22?.Memory;
        public global::Omnix.DataStructures.ReadOnlyListSlim<string>? X23 { get; }
        public global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string>? X24 { get; }
        public SmallMessageStructElement? X25 { get; }
        public MessageStructElement? X26 { get; }

        public static NullableMessageStruct Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(NullableMessageStruct left, NullableMessageStruct right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(NullableMessageStruct left, NullableMessageStruct right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is NullableMessageStruct)) return false;
            return this.Equals((NullableMessageStruct)other);
        }
        public bool Equals(NullableMessageStruct target)
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
            if ((this.X21 is null) != (target.X21 is null)) return false;
            if (!(this.X21 is null) && !(target.X21 is null) && !global::Omnix.Base.BytesOperations.SequenceEqual(this.X21.Value.Span, target.X21.Value.Span)) return false;
            if ((this.X22 is null) != (target.X22 is null)) return false;
            if (!(this.X22 is null) && !(target.X22 is null) && !global::Omnix.Base.BytesOperations.SequenceEqual(this.X22.Value.Span, target.X22.Value.Span)) return false;
            if ((this.X23 is null) != (target.X23 is null)) return false;
            if (!(this.X23 is null) && !(target.X23 is null) && !global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if ((this.X24 is null) != (target.X24 is null)) return false;
            if (!(this.X24 is null) && !(target.X24 is null) && !global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if ((this.X25 is null) != (target.X25 is null)) return false;
            if (!(this.X25 is null) && !(target.X25 is null) && this.X25 != target.X25) return false;
            if ((this.X26 is null) != (target.X26 is null)) return false;
            if (!(this.X26 is null) && !(target.X26 is null) && this.X26 != target.X26) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<NullableMessageStruct>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in NullableMessageStruct value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                    SmallMessageStructElement.Formatter.Serialize(ref w, value.X25.Value, rank + 1);
                }
                if (value.X26 != null)
                {
                    w.Write((uint)26);
                    MessageStructElement.Formatter.Serialize(ref w, value.X26.Value, rank + 1);
                }
            }

            public NullableMessageStruct Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                global::Omnix.Serialization.RocketPack.Timestamp? p_x20 = null;
                global::System.ReadOnlyMemory<byte>? p_x21 = null;
                global::System.Buffers.IMemoryOwner<byte>? p_x22 = null;
                string[]? p_x23 = null;
                global::System.Collections.Generic.Dictionary<byte, string>? p_x24 = null;
                SmallMessageStructElement? p_x25 = null;
                MessageStructElement? p_x26 = null;

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
                                p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
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
                                p_x25 = SmallMessageStructElement.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 26:
                            {
                                p_x26 = MessageStructElement.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                    }
                }

                return new NullableMessageStruct(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

    internal sealed partial class SmallMessageClassElement : global::Omnix.Serialization.RocketPack.IRocketPackMessage<SmallMessageClassElement>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallMessageClassElement> Formatter { get; }
        public static SmallMessageClassElement Empty { get; }

        static SmallMessageClassElement()
        {
            SmallMessageClassElement.Formatter = new ___CustomFormatter();
            SmallMessageClassElement.Empty = new SmallMessageClassElement(false);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public SmallMessageClassElement(bool x0)
        {
            this.X0 = x0;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public bool X0 { get; }

        public static SmallMessageClassElement Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SmallMessageClassElement? left, SmallMessageClassElement? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(SmallMessageClassElement? left, SmallMessageClassElement? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SmallMessageClassElement)) return false;
            return this.Equals((SmallMessageClassElement)other);
        }
        public bool Equals(SmallMessageClassElement? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value!;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallMessageClassElement>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in SmallMessageClassElement value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.X0);
            }

            public SmallMessageClassElement Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                bool p_x0 = false;

                {
                    p_x0 = r.GetBoolean();
                }
                return new SmallMessageClassElement(p_x0);
            }
        }
    }

    internal sealed partial class MessageClassElement : global::Omnix.Serialization.RocketPack.IRocketPackMessage<MessageClassElement>
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<MessageClassElement> Formatter { get; }
        public static MessageClassElement Empty { get; }

        static MessageClassElement()
        {
            MessageClassElement.Formatter = new ___CustomFormatter();
            MessageClassElement.Empty = new MessageClassElement(false);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public MessageClassElement(bool x0)
        {
            this.X0 = x0;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                return ___h.ToHashCode();
            });
        }

        public bool X0 { get; }

        public static MessageClassElement Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(MessageClassElement? left, MessageClassElement? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(MessageClassElement? left, MessageClassElement? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is MessageClassElement)) return false;
            return this.Equals((MessageClassElement)other);
        }
        public bool Equals(MessageClassElement? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value!;

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<MessageClassElement>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in MessageClassElement value, in int rank)
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

            public MessageClassElement Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
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

                return new MessageClassElement(p_x0);
            }
        }
    }

    internal sealed partial class SmallClassMessage : global::Omnix.Serialization.RocketPack.IRocketPackMessage<SmallClassMessage>, global::System.IDisposable
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallClassMessage> Formatter { get; }
        public static SmallClassMessage Empty { get; }

        static SmallClassMessage()
        {
            SmallClassMessage.Formatter = new ___CustomFormatter();
            SmallClassMessage.Empty = new SmallClassMessage(false, 0, 0, 0, 0, 0, 0, 0, 0, (Enum1)0, (Enum2)0, (Enum3)0, (Enum4)0, (Enum5)0, (Enum6)0, (Enum7)0, (Enum8)0, 0.0F, 0.0D, string.Empty, global::Omnix.Serialization.RocketPack.Timestamp.Zero, global::System.ReadOnlyMemory<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::System.Array.Empty<string>(), new global::System.Collections.Generic.Dictionary<byte, string>(), SmallMessageClassElement.Empty, MessageClassElement.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public SmallClassMessage(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, global::Omnix.Serialization.RocketPack.Timestamp x20, global::System.ReadOnlyMemory<byte> x21, global::System.Buffers.IMemoryOwner<byte> x22, string[] x23, global::System.Collections.Generic.Dictionary<byte, string> x24, SmallMessageClassElement x25, MessageClassElement x26)
        {
            if (x19 is null) throw new global::System.ArgumentNullException("x19");
            if (x19.Length > 128) throw new global::System.ArgumentOutOfRangeException("x19");
            if (x21.Length > 256) throw new global::System.ArgumentOutOfRangeException("x21");
            if (x22 is null) throw new global::System.ArgumentNullException("x22");
            if (x22.Memory.Length > 256) throw new global::System.ArgumentOutOfRangeException("x22");
            if (x23 is null) throw new global::System.ArgumentNullException("x23");
            if (x23.Length > 16) throw new global::System.ArgumentOutOfRangeException("x23");
            foreach (var n in x23)
            {
                if (n is null) throw new global::System.ArgumentNullException("n");
                if (n.Length > 128) throw new global::System.ArgumentOutOfRangeException("n");
            }
            if (x24 is null) throw new global::System.ArgumentNullException("x24");
            if (x24.Count > 32) throw new global::System.ArgumentOutOfRangeException("x24");
            foreach (var n in x24)
            {
                if (n.Value is null) throw new global::System.ArgumentNullException("n.Value");
                if (n.Value.Length > 128) throw new global::System.ArgumentOutOfRangeException("n.Value");
            }
            if (x25 is null) throw new global::System.ArgumentNullException("x25");
            if (x26 is null) throw new global::System.ArgumentNullException("x26");

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
            this.X23 = new global::Omnix.DataStructures.ReadOnlyListSlim<string>(x23);
            this.X24 = new global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string>(x24);
            this.X25 = x25;
            this.X26 = x26;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                if (x1 != default) ___h.Add(x1.GetHashCode());
                if (x2 != default) ___h.Add(x2.GetHashCode());
                if (x3 != default) ___h.Add(x3.GetHashCode());
                if (x4 != default) ___h.Add(x4.GetHashCode());
                if (x5 != default) ___h.Add(x5.GetHashCode());
                if (x6 != default) ___h.Add(x6.GetHashCode());
                if (x7 != default) ___h.Add(x7.GetHashCode());
                if (x8 != default) ___h.Add(x8.GetHashCode());
                if (x9 != default) ___h.Add(x9.GetHashCode());
                if (x10 != default) ___h.Add(x10.GetHashCode());
                if (x11 != default) ___h.Add(x11.GetHashCode());
                if (x12 != default) ___h.Add(x12.GetHashCode());
                if (x13 != default) ___h.Add(x13.GetHashCode());
                if (x14 != default) ___h.Add(x14.GetHashCode());
                if (x15 != default) ___h.Add(x15.GetHashCode());
                if (x16 != default) ___h.Add(x16.GetHashCode());
                if (x17 != default) ___h.Add(x17.GetHashCode());
                if (x18 != default) ___h.Add(x18.GetHashCode());
                if (x19 != default) ___h.Add(x19.GetHashCode());
                if (x20 != default) ___h.Add(x20.GetHashCode());
                if (!x21.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x21.Span));
                if (!x22.Memory.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
                foreach (var n in x23)
                {
                    if (n != default) ___h.Add(n.GetHashCode());
                }
                foreach (var n in x24)
                {
                    if (n.Key != default) ___h.Add(n.Key.GetHashCode());
                    if (n.Value != default) ___h.Add(n.Value.GetHashCode());
                }
                if (x25 != default) ___h.Add(x25.GetHashCode());
                if (x26 != default) ___h.Add(x26.GetHashCode());
                return ___h.ToHashCode();
            });
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
        public global::Omnix.Serialization.RocketPack.Timestamp X20 { get; }
        public global::System.ReadOnlyMemory<byte> X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte> _x22;
        public global::System.ReadOnlyMemory<byte> X22 => _x22.Memory;
        public global::Omnix.DataStructures.ReadOnlyListSlim<string> X23 { get; }
        public global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string> X24 { get; }
        public SmallMessageClassElement X25 { get; }
        public MessageClassElement X26 { get; }

        public static SmallClassMessage Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(SmallClassMessage? left, SmallClassMessage? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(SmallClassMessage? left, SmallClassMessage? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is SmallClassMessage)) return false;
            return this.Equals((SmallClassMessage)other);
        }
        public bool Equals(SmallClassMessage? target)
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
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X21.Span, target.X21.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X22.Span, target.X22.Span)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value!;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<SmallClassMessage>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in SmallClassMessage value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.X0);
                w.Write(value.X1);
                w.Write(value.X2);
                w.Write(value.X3);
                w.Write(value.X4);
                w.Write(value.X5);
                w.Write(value.X6);
                w.Write(value.X7);
                w.Write(value.X8);
                w.Write((long)value.X9);
                w.Write((long)value.X10);
                w.Write((long)value.X11);
                w.Write((long)value.X12);
                w.Write((ulong)value.X13);
                w.Write((ulong)value.X14);
                w.Write((ulong)value.X15);
                w.Write((ulong)value.X16);
                w.Write(value.X17);
                w.Write(value.X18);
                w.Write(value.X19);
                w.Write(value.X20);
                w.Write(value.X21.Span);
                w.Write(value.X22.Span);
                w.Write((uint)value.X23.Count);
                foreach (var n in value.X23)
                {
                    w.Write(n);
                }
                w.Write((uint)value.X24.Count);
                foreach (var n in value.X24)
                {
                    w.Write(n.Key);
                    w.Write(n.Value);
                }
                SmallMessageClassElement.Formatter.Serialize(ref w, value.X25, rank + 1);
                MessageClassElement.Formatter.Serialize(ref w, value.X26, rank + 1);
            }

            public SmallClassMessage Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                global::Omnix.Serialization.RocketPack.Timestamp p_x20 = global::Omnix.Serialization.RocketPack.Timestamp.Zero;
                global::System.ReadOnlyMemory<byte> p_x21 = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x22 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                string[] p_x23 = global::System.Array.Empty<string>();
                global::System.Collections.Generic.Dictionary<byte, string> p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
                SmallMessageClassElement p_x25 = SmallMessageClassElement.Empty;
                MessageClassElement p_x26 = MessageClassElement.Empty;

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
                    p_x22 = r.GetRecyclableMemory(256);
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
                    p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
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
                    p_x25 = SmallMessageClassElement.Formatter.Deserialize(ref r, rank + 1);
                }
                {
                    p_x26 = MessageClassElement.Formatter.Deserialize(ref r, rank + 1);
                }
                return new SmallClassMessage(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

    internal sealed partial class MessageClass : global::Omnix.Serialization.RocketPack.IRocketPackMessage<MessageClass>, global::System.IDisposable
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<MessageClass> Formatter { get; }
        public static MessageClass Empty { get; }

        static MessageClass()
        {
            MessageClass.Formatter = new ___CustomFormatter();
            MessageClass.Empty = new MessageClass(false, 0, 0, 0, 0, 0, 0, 0, 0, (Enum1)0, (Enum2)0, (Enum3)0, (Enum4)0, (Enum5)0, (Enum6)0, (Enum7)0, (Enum8)0, 0.0F, 0.0D, string.Empty, global::Omnix.Serialization.RocketPack.Timestamp.Zero, global::System.ReadOnlyMemory<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::System.Array.Empty<string>(), new global::System.Collections.Generic.Dictionary<byte, string>(), SmallMessageClassElement.Empty, MessageClassElement.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public MessageClass(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, global::Omnix.Serialization.RocketPack.Timestamp x20, global::System.ReadOnlyMemory<byte> x21, global::System.Buffers.IMemoryOwner<byte> x22, string[] x23, global::System.Collections.Generic.Dictionary<byte, string> x24, SmallMessageClassElement x25, MessageClassElement x26)
        {
            if (x19 is null) throw new global::System.ArgumentNullException("x19");
            if (x19.Length > 128) throw new global::System.ArgumentOutOfRangeException("x19");
            if (x21.Length > 256) throw new global::System.ArgumentOutOfRangeException("x21");
            if (x22 is null) throw new global::System.ArgumentNullException("x22");
            if (x22.Memory.Length > 256) throw new global::System.ArgumentOutOfRangeException("x22");
            if (x23 is null) throw new global::System.ArgumentNullException("x23");
            if (x23.Length > 16) throw new global::System.ArgumentOutOfRangeException("x23");
            foreach (var n in x23)
            {
                if (n is null) throw new global::System.ArgumentNullException("n");
                if (n.Length > 128) throw new global::System.ArgumentOutOfRangeException("n");
            }
            if (x24 is null) throw new global::System.ArgumentNullException("x24");
            if (x24.Count > 32) throw new global::System.ArgumentOutOfRangeException("x24");
            foreach (var n in x24)
            {
                if (n.Value is null) throw new global::System.ArgumentNullException("n.Value");
                if (n.Value.Length > 128) throw new global::System.ArgumentOutOfRangeException("n.Value");
            }
            if (x25 is null) throw new global::System.ArgumentNullException("x25");
            if (x26 is null) throw new global::System.ArgumentNullException("x26");

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
            this.X23 = new global::Omnix.DataStructures.ReadOnlyListSlim<string>(x23);
            this.X24 = new global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string>(x24);
            this.X25 = x25;
            this.X26 = x26;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                if (x1 != default) ___h.Add(x1.GetHashCode());
                if (x2 != default) ___h.Add(x2.GetHashCode());
                if (x3 != default) ___h.Add(x3.GetHashCode());
                if (x4 != default) ___h.Add(x4.GetHashCode());
                if (x5 != default) ___h.Add(x5.GetHashCode());
                if (x6 != default) ___h.Add(x6.GetHashCode());
                if (x7 != default) ___h.Add(x7.GetHashCode());
                if (x8 != default) ___h.Add(x8.GetHashCode());
                if (x9 != default) ___h.Add(x9.GetHashCode());
                if (x10 != default) ___h.Add(x10.GetHashCode());
                if (x11 != default) ___h.Add(x11.GetHashCode());
                if (x12 != default) ___h.Add(x12.GetHashCode());
                if (x13 != default) ___h.Add(x13.GetHashCode());
                if (x14 != default) ___h.Add(x14.GetHashCode());
                if (x15 != default) ___h.Add(x15.GetHashCode());
                if (x16 != default) ___h.Add(x16.GetHashCode());
                if (x17 != default) ___h.Add(x17.GetHashCode());
                if (x18 != default) ___h.Add(x18.GetHashCode());
                if (x19 != default) ___h.Add(x19.GetHashCode());
                if (x20 != default) ___h.Add(x20.GetHashCode());
                if (!x21.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x21.Span));
                if (!x22.Memory.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
                foreach (var n in x23)
                {
                    if (n != default) ___h.Add(n.GetHashCode());
                }
                foreach (var n in x24)
                {
                    if (n.Key != default) ___h.Add(n.Key.GetHashCode());
                    if (n.Value != default) ___h.Add(n.Value.GetHashCode());
                }
                if (x25 != default) ___h.Add(x25.GetHashCode());
                if (x26 != default) ___h.Add(x26.GetHashCode());
                return ___h.ToHashCode();
            });
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
        public global::Omnix.Serialization.RocketPack.Timestamp X20 { get; }
        public global::System.ReadOnlyMemory<byte> X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte> _x22;
        public global::System.ReadOnlyMemory<byte> X22 => _x22.Memory;
        public global::Omnix.DataStructures.ReadOnlyListSlim<string> X23 { get; }
        public global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string> X24 { get; }
        public SmallMessageClassElement X25 { get; }
        public MessageClassElement X26 { get; }

        public static MessageClass Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(MessageClass? left, MessageClass? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(MessageClass? left, MessageClass? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is MessageClass)) return false;
            return this.Equals((MessageClass)other);
        }
        public bool Equals(MessageClass? target)
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
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X21.Span, target.X21.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X22.Span, target.X22.Span)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value!;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<MessageClass>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in MessageClass value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                    if (value.X20 != global::Omnix.Serialization.RocketPack.Timestamp.Zero)
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
                    if (value.X25 != SmallMessageClassElement.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X26 != MessageClassElement.Empty)
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
                if (value.X20 != global::Omnix.Serialization.RocketPack.Timestamp.Zero)
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
                if (value.X25 != SmallMessageClassElement.Empty)
                {
                    w.Write((uint)25);
                    SmallMessageClassElement.Formatter.Serialize(ref w, value.X25, rank + 1);
                }
                if (value.X26 != MessageClassElement.Empty)
                {
                    w.Write((uint)26);
                    MessageClassElement.Formatter.Serialize(ref w, value.X26, rank + 1);
                }
            }

            public MessageClass Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                global::Omnix.Serialization.RocketPack.Timestamp p_x20 = global::Omnix.Serialization.RocketPack.Timestamp.Zero;
                global::System.ReadOnlyMemory<byte> p_x21 = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x22 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                string[] p_x23 = global::System.Array.Empty<string>();
                global::System.Collections.Generic.Dictionary<byte, string> p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
                SmallMessageClassElement p_x25 = SmallMessageClassElement.Empty;
                MessageClassElement p_x26 = MessageClassElement.Empty;

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
                                p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
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
                                p_x25 = SmallMessageClassElement.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 26:
                            {
                                p_x26 = MessageClassElement.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                    }
                }

                return new MessageClass(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

    internal sealed partial class NullableMessageClass : global::Omnix.Serialization.RocketPack.IRocketPackMessage<NullableMessageClass>, global::System.IDisposable
    {
        public static global::Omnix.Serialization.RocketPack.IRocketPackFormatter<NullableMessageClass> Formatter { get; }
        public static NullableMessageClass Empty { get; }

        static NullableMessageClass()
        {
            NullableMessageClass.Formatter = new ___CustomFormatter();
            NullableMessageClass.Empty = new NullableMessageClass(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public NullableMessageClass(bool? x0, sbyte? x1, short? x2, int? x3, long? x4, byte? x5, ushort? x6, uint? x7, ulong? x8, Enum1? x9, Enum2? x10, Enum3? x11, Enum4? x12, Enum5? x13, Enum6? x14, Enum7? x15, Enum8? x16, float? x17, double? x18, string? x19, global::Omnix.Serialization.RocketPack.Timestamp? x20, global::System.ReadOnlyMemory<byte>? x21, global::System.Buffers.IMemoryOwner<byte>? x22, string[]? x23, global::System.Collections.Generic.Dictionary<byte, string>? x24, SmallMessageClassElement? x25, MessageClassElement? x26)
        {
            if (!(x19 is null) && x19.Length > 128) throw new global::System.ArgumentOutOfRangeException("x19");
            if (!(x21 is null) && x21.Value.Length > 256) throw new global::System.ArgumentOutOfRangeException("x21");
            if (!(x22 is null) && x22.Memory.Length > 256) throw new global::System.ArgumentOutOfRangeException("x22");
            if (!(x23 is null) && x23.Length > 16) throw new global::System.ArgumentOutOfRangeException("x23");
            if (!(x23 is null))
            {
                foreach (var n in x23)
                {
                    if (n is null) throw new global::System.ArgumentNullException("n");
                    if (n.Length > 128) throw new global::System.ArgumentOutOfRangeException("n");
                }
            }
            if (!(x24 is null) && x24.Count > 32) throw new global::System.ArgumentOutOfRangeException("x24");
            if (!(x24 is null))
            {
                foreach (var n in x24)
                {
                    if (n.Value is null) throw new global::System.ArgumentNullException("n.Value");
                    if (n.Value.Length > 128) throw new global::System.ArgumentOutOfRangeException("n.Value");
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
                this.X23 = new global::Omnix.DataStructures.ReadOnlyListSlim<string>(x23);
            }
            else
            {
                this.X23 = null;
            }
            if(x24 != null)
            {
                this.X24 = new global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string>(x24);
            }
            else
            {
                this.X24 = null;
            }
            this.X25 = x25;
            this.X26 = x26;

            ___hashCode = new global::System.Lazy<int>(() =>
            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                if (x1 != default) ___h.Add(x1.GetHashCode());
                if (x2 != default) ___h.Add(x2.GetHashCode());
                if (x3 != default) ___h.Add(x3.GetHashCode());
                if (x4 != default) ___h.Add(x4.GetHashCode());
                if (x5 != default) ___h.Add(x5.GetHashCode());
                if (x6 != default) ___h.Add(x6.GetHashCode());
                if (x7 != default) ___h.Add(x7.GetHashCode());
                if (x8 != default) ___h.Add(x8.GetHashCode());
                if (x9 != default) ___h.Add(x9.GetHashCode());
                if (x10 != default) ___h.Add(x10.GetHashCode());
                if (x11 != default) ___h.Add(x11.GetHashCode());
                if (x12 != default) ___h.Add(x12.GetHashCode());
                if (x13 != default) ___h.Add(x13.GetHashCode());
                if (x14 != default) ___h.Add(x14.GetHashCode());
                if (x15 != default) ___h.Add(x15.GetHashCode());
                if (x16 != default) ___h.Add(x16.GetHashCode());
                if (x17 != default) ___h.Add(x17.GetHashCode());
                if (x18 != default) ___h.Add(x18.GetHashCode());
                if (x19 != default) ___h.Add(x19.GetHashCode());
                if (x20 != default) ___h.Add(x20.GetHashCode());
                if (!(x21 is null) && !x21.Value.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x21.Value.Span));
                if (!(x22 is null) && !x22.Memory.IsEmpty) ___h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
                if(x23 != null)
                {
                    foreach (var n in x23)
                    {
                        if (n != default) ___h.Add(n.GetHashCode());
                    }
                }
                if(x24 != null)
                {
                    foreach (var n in x24)
                    {
                        if (n.Key != default) ___h.Add(n.Key.GetHashCode());
                        if (n.Value != default) ___h.Add(n.Value.GetHashCode());
                    }
                }
                if (x25 != default) ___h.Add(x25.GetHashCode());
                if (x26 != default) ___h.Add(x26.GetHashCode());
                return ___h.ToHashCode();
            });
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
        public global::Omnix.Serialization.RocketPack.Timestamp? X20 { get; }
        public global::System.ReadOnlyMemory<byte>? X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte>? _x22;
        public global::System.ReadOnlyMemory<byte>? X22 => _x22?.Memory;
        public global::Omnix.DataStructures.ReadOnlyListSlim<string>? X23 { get; }
        public global::Omnix.DataStructures.ReadOnlyDictionarySlim<byte, string>? X24 { get; }
        public SmallMessageClassElement? X25 { get; }
        public MessageClassElement? X26 { get; }

        public static NullableMessageClass Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnix.Base.BufferPool bufferPool)
        {
            var reader = new global::Omnix.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnix.Base.BufferPool bufferPool)
        {
            var writer = new global::Omnix.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(NullableMessageClass? left, NullableMessageClass? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(NullableMessageClass? left, NullableMessageClass? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is NullableMessageClass)) return false;
            return this.Equals((NullableMessageClass)other);
        }
        public bool Equals(NullableMessageClass? target)
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
            if (!(this.X21 is null) && !(target.X21 is null) && !global::Omnix.Base.BytesOperations.SequenceEqual(this.X21.Value.Span, target.X21.Value.Span)) return false;
            if ((this.X22 is null) != (target.X22 is null)) return false;
            if (!(this.X22 is null) && !(target.X22 is null) && !global::Omnix.Base.BytesOperations.SequenceEqual(this.X22.Value.Span, target.X22.Value.Span)) return false;
            if ((this.X23 is null) != (target.X23 is null)) return false;
            if (!(this.X23 is null) && !(target.X23 is null) && !global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if ((this.X24 is null) != (target.X24 is null)) return false;
            if (!(this.X24 is null) && !(target.X24 is null) && !global::Omnix.Base.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if ((this.X25 is null) != (target.X25 is null)) return false;
            if (!(this.X25 is null) && !(target.X25 is null) && this.X25 != target.X25) return false;
            if ((this.X26 is null) != (target.X26 is null)) return false;
            if (!(this.X26 is null) && !(target.X26 is null) && this.X26 != target.X26) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value!;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<NullableMessageClass>
        {
            public void Serialize(ref global::Omnix.Serialization.RocketPack.RocketPackWriter w, in NullableMessageClass value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                    SmallMessageClassElement.Formatter.Serialize(ref w, value.X25, rank + 1);
                }
                if (value.X26 != null)
                {
                    w.Write((uint)26);
                    MessageClassElement.Formatter.Serialize(ref w, value.X26, rank + 1);
                }
            }

            public NullableMessageClass Deserialize(ref global::Omnix.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

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
                global::Omnix.Serialization.RocketPack.Timestamp? p_x20 = null;
                global::System.ReadOnlyMemory<byte>? p_x21 = null;
                global::System.Buffers.IMemoryOwner<byte>? p_x22 = null;
                string[]? p_x23 = null;
                global::System.Collections.Generic.Dictionary<byte, string>? p_x24 = null;
                SmallMessageClassElement? p_x25 = null;
                MessageClassElement? p_x26 = null;

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
                                p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
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
                                p_x25 = SmallMessageClassElement.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 26:
                            {
                                p_x26 = MessageClassElement.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                    }
                }

                return new NullableMessageClass(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

}
