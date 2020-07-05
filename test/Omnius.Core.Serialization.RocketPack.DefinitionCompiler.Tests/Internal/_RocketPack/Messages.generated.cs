
#nullable enable

namespace Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal
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

    internal readonly partial struct StructMessageElement_Struct : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct>.Formatter;
        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct>.Empty;

        static StructMessageElement_Struct()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct>.Empty = new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct(false);
        }

        private readonly int ___hashCode;

        public StructMessageElement_Struct(bool x0)
        {
            this.X0 = x0;

            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                ___hashCode = ___h.ToHashCode();
            }
        }

        public bool X0 { get; }

        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct)) return false;
            return this.Equals((global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct)other);
        }
        public bool Equals(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct target)
        {
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.X0);
            }

            public global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                bool p_x0 = false;

                {
                    p_x0 = r.GetBoolean();
                }
                return new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct(p_x0);
            }
        }
    }

    internal readonly partial struct TableMessageElement_Struct : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct>.Formatter;
        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct>.Empty;

        static TableMessageElement_Struct()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct>.Empty = new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct(false);
        }

        private readonly int ___hashCode;

        public TableMessageElement_Struct(bool x0)
        {
            this.X0 = x0;

            {
                var ___h = new global::System.HashCode();
                if (x0 != default) ___h.Add(x0.GetHashCode());
                ___hashCode = ___h.ToHashCode();
            }
        }

        public bool X0 { get; }

        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct)) return false;
            return this.Equals((global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct)other);
        }
        public bool Equals(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct target)
        {
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct value, in int rank)
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

            public global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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

                return new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct(p_x0);
            }
        }
    }

    internal sealed partial class StructMessageElement_Class : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class>.Formatter;
        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class>.Empty;

        static StructMessageElement_Class()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class>.Empty = new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class(false);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public StructMessageElement_Class(bool x0)
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

        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class? left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class? left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class)) return false;
            return this.Equals((global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class)other);
        }
        public bool Equals(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.X0);
            }

            public global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                bool p_x0 = false;

                {
                    p_x0 = r.GetBoolean();
                }
                return new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class(p_x0);
            }
        }
    }

    internal sealed partial class TableMessageElement_Class : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class>.Formatter;
        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class>.Empty;

        static TableMessageElement_Class()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class>.Empty = new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class(false);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public TableMessageElement_Class(bool x0)
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

        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class? left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class? left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class)) return false;
            return this.Equals((global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class)other);
        }
        public bool Equals(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class value, in int rank)
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

            public global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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

                return new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class(p_x0);
            }
        }
    }

    internal readonly partial struct StructMessage_Struct : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct>, global::System.IDisposable
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct>.Formatter;
        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct>.Empty;

        static StructMessage_Struct()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct>.Empty = new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct(false, 0, 0, 0, 0, 0, 0, 0, 0, (Enum1)0, (Enum2)0, (Enum3)0, (Enum4)0, (Enum5)0, (Enum6)0, (Enum7)0, (Enum8)0, 0.0F, 0.0D, string.Empty, global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero, global::System.ReadOnlyMemory<byte>.Empty, global::Omnius.Core.SimpleMemoryOwner<byte>.Empty, global::System.Array.Empty<string>(), new global::System.Collections.Generic.Dictionary<byte, string>(), StructMessageElement_Struct.Empty, TableMessageElement_Struct.Empty, StructMessageElement_Class.Empty, TableMessageElement_Class.Empty);
        }

        private readonly int ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public StructMessage_Struct(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, global::Omnius.Core.Serialization.RocketPack.Timestamp x20, global::System.ReadOnlyMemory<byte> x21, global::System.Buffers.IMemoryOwner<byte> x22, string[] x23, global::System.Collections.Generic.Dictionary<byte, string> x24, StructMessageElement_Struct x25, TableMessageElement_Struct x26, StructMessageElement_Class x27, TableMessageElement_Class x28)
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
            if (x27 is null) throw new global::System.ArgumentNullException("x27");
            if (x28 is null) throw new global::System.ArgumentNullException("x28");

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
            this.X23 = new global::Omnius.Core.Collections.ReadOnlyListSlim<string>(x23);
            this.X24 = new global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string>(x24);
            this.X25 = x25;
            this.X26 = x26;
            this.X27 = x27;
            this.X28 = x28;

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
                if (!x21.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x21.Span));
                if (!x22.Memory.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
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
                if (x27 != default) ___h.Add(x27.GetHashCode());
                if (x28 != default) ___h.Add(x28.GetHashCode());
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
        public global::Omnius.Core.Serialization.RocketPack.Timestamp X20 { get; }
        public global::System.ReadOnlyMemory<byte> X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte> _x22;
        public global::System.ReadOnlyMemory<byte> X22 => _x22.Memory;
        public global::Omnius.Core.Collections.ReadOnlyListSlim<string> X23 { get; }
        public global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string> X24 { get; }
        public StructMessageElement_Struct X25 { get; }
        public TableMessageElement_Struct X26 { get; }
        public StructMessageElement_Class X27 { get; }
        public TableMessageElement_Class X28 { get; }

        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct)) return false;
            return this.Equals((global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct)other);
        }
        public bool Equals(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct target)
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
            if (!global::Omnius.Core.BytesOperations.Equals(this.X21.Span, target.X21.Span)) return false;
            if (!global::Omnius.Core.BytesOperations.Equals(this.X22.Span, target.X22.Span)) return false;
            if (!global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if (!global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;
            if (this.X27 != target.X27) return false;
            if (this.X28 != target.X28) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct value, in int rank)
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
                global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Serialize(ref w, value.X25, rank + 1);
                global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Serialize(ref w, value.X26, rank + 1);
                global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Serialize(ref w, value.X27, rank + 1);
                global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Serialize(ref w, value.X28, rank + 1);
            }

            public global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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
                global::Omnius.Core.Serialization.RocketPack.Timestamp p_x20 = global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero;
                global::System.ReadOnlyMemory<byte> p_x21 = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x22 = global::Omnius.Core.SimpleMemoryOwner<byte>.Empty;
                string[] p_x23 = global::System.Array.Empty<string>();
                global::System.Collections.Generic.Dictionary<byte, string> p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
                StructMessageElement_Struct p_x25 = StructMessageElement_Struct.Empty;
                TableMessageElement_Struct p_x26 = TableMessageElement_Struct.Empty;
                StructMessageElement_Class p_x27 = StructMessageElement_Class.Empty;
                TableMessageElement_Class p_x28 = TableMessageElement_Class.Empty;

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
                    p_x25 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                }
                {
                    p_x26 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                }
                {
                    p_x27 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                }
                {
                    p_x28 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                }
                return new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Struct(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26, p_x27, p_x28);
            }
        }
    }

    internal readonly partial struct TableMessage_Struct : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct>, global::System.IDisposable
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct>.Formatter;
        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct>.Empty;

        static TableMessage_Struct()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct>.Empty = new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct(false, 0, 0, 0, 0, 0, 0, 0, 0, (Enum1)0, (Enum2)0, (Enum3)0, (Enum4)0, (Enum5)0, (Enum6)0, (Enum7)0, (Enum8)0, 0.0F, 0.0D, string.Empty, global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero, global::System.ReadOnlyMemory<byte>.Empty, global::Omnius.Core.SimpleMemoryOwner<byte>.Empty, global::System.Array.Empty<string>(), new global::System.Collections.Generic.Dictionary<byte, string>(), StructMessageElement_Struct.Empty, TableMessageElement_Struct.Empty, StructMessageElement_Class.Empty, TableMessageElement_Class.Empty);
        }

        private readonly int ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public TableMessage_Struct(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, global::Omnius.Core.Serialization.RocketPack.Timestamp x20, global::System.ReadOnlyMemory<byte> x21, global::System.Buffers.IMemoryOwner<byte> x22, string[] x23, global::System.Collections.Generic.Dictionary<byte, string> x24, StructMessageElement_Struct x25, TableMessageElement_Struct x26, StructMessageElement_Class x27, TableMessageElement_Class x28)
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
            if (x27 is null) throw new global::System.ArgumentNullException("x27");
            if (x28 is null) throw new global::System.ArgumentNullException("x28");

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
            this.X23 = new global::Omnius.Core.Collections.ReadOnlyListSlim<string>(x23);
            this.X24 = new global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string>(x24);
            this.X25 = x25;
            this.X26 = x26;
            this.X27 = x27;
            this.X28 = x28;

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
                if (!x21.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x21.Span));
                if (!x22.Memory.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
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
                if (x27 != default) ___h.Add(x27.GetHashCode());
                if (x28 != default) ___h.Add(x28.GetHashCode());
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
        public global::Omnius.Core.Serialization.RocketPack.Timestamp X20 { get; }
        public global::System.ReadOnlyMemory<byte> X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte> _x22;
        public global::System.ReadOnlyMemory<byte> X22 => _x22.Memory;
        public global::Omnius.Core.Collections.ReadOnlyListSlim<string> X23 { get; }
        public global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string> X24 { get; }
        public StructMessageElement_Struct X25 { get; }
        public TableMessageElement_Struct X26 { get; }
        public StructMessageElement_Class X27 { get; }
        public TableMessageElement_Class X28 { get; }

        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct)) return false;
            return this.Equals((global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct)other);
        }
        public bool Equals(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct target)
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
            if (!global::Omnius.Core.BytesOperations.Equals(this.X21.Span, target.X21.Span)) return false;
            if (!global::Omnius.Core.BytesOperations.Equals(this.X22.Span, target.X22.Span)) return false;
            if (!global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if (!global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;
            if (this.X27 != target.X27) return false;
            if (this.X28 != target.X28) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct value, in int rank)
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
                    if (value.X20 != global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero)
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
                    if (value.X25 != StructMessageElement_Struct.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X26 != TableMessageElement_Struct.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X27 != StructMessageElement_Class.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X28 != TableMessageElement_Class.Empty)
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
                if (value.X20 != global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero)
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
                if (value.X25 != StructMessageElement_Struct.Empty)
                {
                    w.Write((uint)25);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Serialize(ref w, value.X25, rank + 1);
                }
                if (value.X26 != TableMessageElement_Struct.Empty)
                {
                    w.Write((uint)26);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Serialize(ref w, value.X26, rank + 1);
                }
                if (value.X27 != StructMessageElement_Class.Empty)
                {
                    w.Write((uint)27);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Serialize(ref w, value.X27, rank + 1);
                }
                if (value.X28 != TableMessageElement_Class.Empty)
                {
                    w.Write((uint)28);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Serialize(ref w, value.X28, rank + 1);
                }
            }

            public global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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
                global::Omnius.Core.Serialization.RocketPack.Timestamp p_x20 = global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero;
                global::System.ReadOnlyMemory<byte> p_x21 = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x22 = global::Omnius.Core.SimpleMemoryOwner<byte>.Empty;
                string[] p_x23 = global::System.Array.Empty<string>();
                global::System.Collections.Generic.Dictionary<byte, string> p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
                StructMessageElement_Struct p_x25 = StructMessageElement_Struct.Empty;
                TableMessageElement_Struct p_x26 = TableMessageElement_Struct.Empty;
                StructMessageElement_Class p_x27 = StructMessageElement_Class.Empty;
                TableMessageElement_Class p_x28 = TableMessageElement_Class.Empty;

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
                                p_x25 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 26:
                            {
                                p_x26 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 27:
                            {
                                p_x27 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 28:
                            {
                                p_x28 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                    }
                }

                return new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Struct(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26, p_x27, p_x28);
            }
        }
    }

    internal readonly partial struct NullableTableMessage_Struct : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct>, global::System.IDisposable
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct>.Formatter;
        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct>.Empty;

        static NullableTableMessage_Struct()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct>.Empty = new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        private readonly int ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public NullableTableMessage_Struct(bool? x0, sbyte? x1, short? x2, int? x3, long? x4, byte? x5, ushort? x6, uint? x7, ulong? x8, Enum1? x9, Enum2? x10, Enum3? x11, Enum4? x12, Enum5? x13, Enum6? x14, Enum7? x15, Enum8? x16, float? x17, double? x18, string? x19, global::Omnius.Core.Serialization.RocketPack.Timestamp? x20, global::System.ReadOnlyMemory<byte>? x21, global::System.Buffers.IMemoryOwner<byte>? x22, string[]? x23, global::System.Collections.Generic.Dictionary<byte, string>? x24, StructMessageElement_Struct? x25, TableMessageElement_Struct? x26, StructMessageElement_Class? x27, TableMessageElement_Class? x28)
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
                this.X23 = new global::Omnius.Core.Collections.ReadOnlyListSlim<string>(x23);
            }
            else
            {
                this.X23 = null;
            }
            if(x24 != null)
            {
                this.X24 = new global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string>(x24);
            }
            else
            {
                this.X24 = null;
            }
            this.X25 = x25;
            this.X26 = x26;
            this.X27 = x27;
            this.X28 = x28;

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
                if (!(x21 is null) && !x21.Value.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x21.Value.Span));
                if (!(x22 is null) && !x22.Memory.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
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
                if (x27 != default) ___h.Add(x27.GetHashCode());
                if (x28 != default) ___h.Add(x28.GetHashCode());
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
        public global::Omnius.Core.Serialization.RocketPack.Timestamp? X20 { get; }
        public global::System.ReadOnlyMemory<byte>? X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte>? _x22;
        public global::System.ReadOnlyMemory<byte>? X22 => _x22?.Memory;
        public global::Omnius.Core.Collections.ReadOnlyListSlim<string>? X23 { get; }
        public global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string>? X24 { get; }
        public StructMessageElement_Struct? X25 { get; }
        public TableMessageElement_Struct? X26 { get; }
        public StructMessageElement_Class? X27 { get; }
        public TableMessageElement_Class? X28 { get; }

        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct)) return false;
            return this.Equals((global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct)other);
        }
        public bool Equals(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct target)
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
            if (!(this.X21 is null) && !(target.X21 is null) && !global::Omnius.Core.BytesOperations.Equals(this.X21.Value.Span, target.X21.Value.Span)) return false;
            if ((this.X22 is null) != (target.X22 is null)) return false;
            if (!(this.X22 is null) && !(target.X22 is null) && !global::Omnius.Core.BytesOperations.Equals(this.X22.Value.Span, target.X22.Value.Span)) return false;
            if ((this.X23 is null) != (target.X23 is null)) return false;
            if (!(this.X23 is null) && !(target.X23 is null) && !global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if ((this.X24 is null) != (target.X24 is null)) return false;
            if (!(this.X24 is null) && !(target.X24 is null) && !global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if ((this.X25 is null) != (target.X25 is null)) return false;
            if (!(this.X25 is null) && !(target.X25 is null) && this.X25 != target.X25) return false;
            if ((this.X26 is null) != (target.X26 is null)) return false;
            if (!(this.X26 is null) && !(target.X26 is null) && this.X26 != target.X26) return false;
            if ((this.X27 is null) != (target.X27 is null)) return false;
            if (!(this.X27 is null) && !(target.X27 is null) && this.X27 != target.X27) return false;
            if ((this.X28 is null) != (target.X28 is null)) return false;
            if (!(this.X28 is null) && !(target.X28 is null) && this.X28 != target.X28) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct value, in int rank)
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
                    if (value.X27 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X28 != null)
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
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Serialize(ref w, value.X25.Value, rank + 1);
                }
                if (value.X26 != null)
                {
                    w.Write((uint)26);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Serialize(ref w, value.X26.Value, rank + 1);
                }
                if (value.X27 != null)
                {
                    w.Write((uint)27);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Serialize(ref w, value.X27, rank + 1);
                }
                if (value.X28 != null)
                {
                    w.Write((uint)28);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Serialize(ref w, value.X28, rank + 1);
                }
            }

            public global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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
                global::Omnius.Core.Serialization.RocketPack.Timestamp? p_x20 = null;
                global::System.ReadOnlyMemory<byte>? p_x21 = null;
                global::System.Buffers.IMemoryOwner<byte>? p_x22 = null;
                string[]? p_x23 = null;
                global::System.Collections.Generic.Dictionary<byte, string>? p_x24 = null;
                StructMessageElement_Struct? p_x25 = null;
                TableMessageElement_Struct? p_x26 = null;
                StructMessageElement_Class? p_x27 = null;
                TableMessageElement_Class? p_x28 = null;

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
                                p_x25 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 26:
                            {
                                p_x26 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 27:
                            {
                                p_x27 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 28:
                            {
                                p_x28 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                    }
                }

                return new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Struct(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26, p_x27, p_x28);
            }
        }
    }

    internal sealed partial class StructMessage_Class : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class>, global::System.IDisposable
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class>.Formatter;
        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class>.Empty;

        static StructMessage_Class()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class>.Empty = new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class(false, 0, 0, 0, 0, 0, 0, 0, 0, (Enum1)0, (Enum2)0, (Enum3)0, (Enum4)0, (Enum5)0, (Enum6)0, (Enum7)0, (Enum8)0, 0.0F, 0.0D, string.Empty, global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero, global::System.ReadOnlyMemory<byte>.Empty, global::Omnius.Core.SimpleMemoryOwner<byte>.Empty, global::System.Array.Empty<string>(), new global::System.Collections.Generic.Dictionary<byte, string>(), StructMessageElement_Struct.Empty, TableMessageElement_Struct.Empty, StructMessageElement_Class.Empty, TableMessageElement_Class.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public StructMessage_Class(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, global::Omnius.Core.Serialization.RocketPack.Timestamp x20, global::System.ReadOnlyMemory<byte> x21, global::System.Buffers.IMemoryOwner<byte> x22, string[] x23, global::System.Collections.Generic.Dictionary<byte, string> x24, StructMessageElement_Struct x25, TableMessageElement_Struct x26, StructMessageElement_Class x27, TableMessageElement_Class x28)
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
            if (x27 is null) throw new global::System.ArgumentNullException("x27");
            if (x28 is null) throw new global::System.ArgumentNullException("x28");

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
            this.X23 = new global::Omnius.Core.Collections.ReadOnlyListSlim<string>(x23);
            this.X24 = new global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string>(x24);
            this.X25 = x25;
            this.X26 = x26;
            this.X27 = x27;
            this.X28 = x28;

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
                if (!x21.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x21.Span));
                if (!x22.Memory.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
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
                if (x27 != default) ___h.Add(x27.GetHashCode());
                if (x28 != default) ___h.Add(x28.GetHashCode());
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
        public global::Omnius.Core.Serialization.RocketPack.Timestamp X20 { get; }
        public global::System.ReadOnlyMemory<byte> X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte> _x22;
        public global::System.ReadOnlyMemory<byte> X22 => _x22.Memory;
        public global::Omnius.Core.Collections.ReadOnlyListSlim<string> X23 { get; }
        public global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string> X24 { get; }
        public StructMessageElement_Struct X25 { get; }
        public TableMessageElement_Struct X26 { get; }
        public StructMessageElement_Class X27 { get; }
        public TableMessageElement_Class X28 { get; }

        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class? left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class? left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class)) return false;
            return this.Equals((global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class)other);
        }
        public bool Equals(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class? target)
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
            if (!global::Omnius.Core.BytesOperations.Equals(this.X21.Span, target.X21.Span)) return false;
            if (!global::Omnius.Core.BytesOperations.Equals(this.X22.Span, target.X22.Span)) return false;
            if (!global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if (!global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;
            if (this.X27 != target.X27) return false;
            if (this.X28 != target.X28) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class value, in int rank)
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
                global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Serialize(ref w, value.X25, rank + 1);
                global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Serialize(ref w, value.X26, rank + 1);
                global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Serialize(ref w, value.X27, rank + 1);
                global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Serialize(ref w, value.X28, rank + 1);
            }

            public global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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
                global::Omnius.Core.Serialization.RocketPack.Timestamp p_x20 = global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero;
                global::System.ReadOnlyMemory<byte> p_x21 = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x22 = global::Omnius.Core.SimpleMemoryOwner<byte>.Empty;
                string[] p_x23 = global::System.Array.Empty<string>();
                global::System.Collections.Generic.Dictionary<byte, string> p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
                StructMessageElement_Struct p_x25 = StructMessageElement_Struct.Empty;
                TableMessageElement_Struct p_x26 = TableMessageElement_Struct.Empty;
                StructMessageElement_Class p_x27 = StructMessageElement_Class.Empty;
                TableMessageElement_Class p_x28 = TableMessageElement_Class.Empty;

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
                    p_x25 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                }
                {
                    p_x26 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                }
                {
                    p_x27 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                }
                {
                    p_x28 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                }
                return new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessage_Class(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26, p_x27, p_x28);
            }
        }
    }

    internal sealed partial class TableMessage_Class : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class>, global::System.IDisposable
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class>.Formatter;
        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class>.Empty;

        static TableMessage_Class()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class>.Empty = new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class(false, 0, 0, 0, 0, 0, 0, 0, 0, (Enum1)0, (Enum2)0, (Enum3)0, (Enum4)0, (Enum5)0, (Enum6)0, (Enum7)0, (Enum8)0, 0.0F, 0.0D, string.Empty, global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero, global::System.ReadOnlyMemory<byte>.Empty, global::Omnius.Core.SimpleMemoryOwner<byte>.Empty, global::System.Array.Empty<string>(), new global::System.Collections.Generic.Dictionary<byte, string>(), StructMessageElement_Struct.Empty, TableMessageElement_Struct.Empty, StructMessageElement_Class.Empty, TableMessageElement_Class.Empty);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public TableMessage_Class(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, global::Omnius.Core.Serialization.RocketPack.Timestamp x20, global::System.ReadOnlyMemory<byte> x21, global::System.Buffers.IMemoryOwner<byte> x22, string[] x23, global::System.Collections.Generic.Dictionary<byte, string> x24, StructMessageElement_Struct x25, TableMessageElement_Struct x26, StructMessageElement_Class x27, TableMessageElement_Class x28)
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
            if (x27 is null) throw new global::System.ArgumentNullException("x27");
            if (x28 is null) throw new global::System.ArgumentNullException("x28");

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
            this.X23 = new global::Omnius.Core.Collections.ReadOnlyListSlim<string>(x23);
            this.X24 = new global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string>(x24);
            this.X25 = x25;
            this.X26 = x26;
            this.X27 = x27;
            this.X28 = x28;

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
                if (!x21.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x21.Span));
                if (!x22.Memory.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
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
                if (x27 != default) ___h.Add(x27.GetHashCode());
                if (x28 != default) ___h.Add(x28.GetHashCode());
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
        public global::Omnius.Core.Serialization.RocketPack.Timestamp X20 { get; }
        public global::System.ReadOnlyMemory<byte> X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte> _x22;
        public global::System.ReadOnlyMemory<byte> X22 => _x22.Memory;
        public global::Omnius.Core.Collections.ReadOnlyListSlim<string> X23 { get; }
        public global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string> X24 { get; }
        public StructMessageElement_Struct X25 { get; }
        public TableMessageElement_Struct X26 { get; }
        public StructMessageElement_Class X27 { get; }
        public TableMessageElement_Class X28 { get; }

        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class? left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class? left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class)) return false;
            return this.Equals((global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class)other);
        }
        public bool Equals(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class? target)
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
            if (!global::Omnius.Core.BytesOperations.Equals(this.X21.Span, target.X21.Span)) return false;
            if (!global::Omnius.Core.BytesOperations.Equals(this.X22.Span, target.X22.Span)) return false;
            if (!global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if (!global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;
            if (this.X27 != target.X27) return false;
            if (this.X28 != target.X28) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class value, in int rank)
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
                    if (value.X20 != global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero)
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
                    if (value.X25 != StructMessageElement_Struct.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X26 != TableMessageElement_Struct.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X27 != StructMessageElement_Class.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X28 != TableMessageElement_Class.Empty)
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
                if (value.X20 != global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero)
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
                if (value.X25 != StructMessageElement_Struct.Empty)
                {
                    w.Write((uint)25);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Serialize(ref w, value.X25, rank + 1);
                }
                if (value.X26 != TableMessageElement_Struct.Empty)
                {
                    w.Write((uint)26);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Serialize(ref w, value.X26, rank + 1);
                }
                if (value.X27 != StructMessageElement_Class.Empty)
                {
                    w.Write((uint)27);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Serialize(ref w, value.X27, rank + 1);
                }
                if (value.X28 != TableMessageElement_Class.Empty)
                {
                    w.Write((uint)28);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Serialize(ref w, value.X28, rank + 1);
                }
            }

            public global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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
                global::Omnius.Core.Serialization.RocketPack.Timestamp p_x20 = global::Omnius.Core.Serialization.RocketPack.Timestamp.Zero;
                global::System.ReadOnlyMemory<byte> p_x21 = global::System.ReadOnlyMemory<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x22 = global::Omnius.Core.SimpleMemoryOwner<byte>.Empty;
                string[] p_x23 = global::System.Array.Empty<string>();
                global::System.Collections.Generic.Dictionary<byte, string> p_x24 = new global::System.Collections.Generic.Dictionary<byte, string>();
                StructMessageElement_Struct p_x25 = StructMessageElement_Struct.Empty;
                TableMessageElement_Struct p_x26 = TableMessageElement_Struct.Empty;
                StructMessageElement_Class p_x27 = StructMessageElement_Class.Empty;
                TableMessageElement_Class p_x28 = TableMessageElement_Class.Empty;

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
                                p_x25 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 26:
                            {
                                p_x26 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 27:
                            {
                                p_x27 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 28:
                            {
                                p_x28 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                    }
                }

                return new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessage_Class(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26, p_x27, p_x28);
            }
        }
    }

    internal sealed partial class NullableTableMessage_Class : global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class>, global::System.IDisposable
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class> Formatter => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class>.Formatter;
        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class Empty => global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class>.Empty;

        static NullableTableMessage_Class()
        {
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class>.Formatter = new ___CustomFormatter();
            global::Omnius.Core.Serialization.RocketPack.IRocketPackObject<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class>.Empty = new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        private readonly global::System.Lazy<int> ___hashCode;

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public NullableTableMessage_Class(bool? x0, sbyte? x1, short? x2, int? x3, long? x4, byte? x5, ushort? x6, uint? x7, ulong? x8, Enum1? x9, Enum2? x10, Enum3? x11, Enum4? x12, Enum5? x13, Enum6? x14, Enum7? x15, Enum8? x16, float? x17, double? x18, string? x19, global::Omnius.Core.Serialization.RocketPack.Timestamp? x20, global::System.ReadOnlyMemory<byte>? x21, global::System.Buffers.IMemoryOwner<byte>? x22, string[]? x23, global::System.Collections.Generic.Dictionary<byte, string>? x24, StructMessageElement_Struct? x25, TableMessageElement_Struct? x26, StructMessageElement_Class? x27, TableMessageElement_Class? x28)
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
                this.X23 = new global::Omnius.Core.Collections.ReadOnlyListSlim<string>(x23);
            }
            else
            {
                this.X23 = null;
            }
            if(x24 != null)
            {
                this.X24 = new global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string>(x24);
            }
            else
            {
                this.X24 = null;
            }
            this.X25 = x25;
            this.X26 = x26;
            this.X27 = x27;
            this.X28 = x28;

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
                if (!(x21 is null) && !x21.Value.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x21.Value.Span));
                if (!(x22 is null) && !x22.Memory.IsEmpty) ___h.Add(global::Omnius.Core.Helpers.ObjectHelper.GetHashCode(x22.Memory.Span));
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
                if (x27 != default) ___h.Add(x27.GetHashCode());
                if (x28 != default) ___h.Add(x28.GetHashCode());
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
        public global::Omnius.Core.Serialization.RocketPack.Timestamp? X20 { get; }
        public global::System.ReadOnlyMemory<byte>? X21 { get; }
        private readonly global::System.Buffers.IMemoryOwner<byte>? _x22;
        public global::System.ReadOnlyMemory<byte>? X22 => _x22?.Memory;
        public global::Omnius.Core.Collections.ReadOnlyListSlim<string>? X23 { get; }
        public global::Omnius.Core.Collections.ReadOnlyDictionarySlim<byte, string>? X24 { get; }
        public StructMessageElement_Struct? X25 { get; }
        public TableMessageElement_Struct? X26 { get; }
        public StructMessageElement_Class? X27 { get; }
        public TableMessageElement_Class? X28 { get; }

        public static global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBytesPool bytesPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bytesPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBytesPool bytesPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bytesPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class? left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class? left, global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class)) return false;
            return this.Equals((global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class)other);
        }
        public bool Equals(global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class? target)
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
            if (!(this.X21 is null) && !(target.X21 is null) && !global::Omnius.Core.BytesOperations.Equals(this.X21.Value.Span, target.X21.Value.Span)) return false;
            if ((this.X22 is null) != (target.X22 is null)) return false;
            if (!(this.X22 is null) && !(target.X22 is null) && !global::Omnius.Core.BytesOperations.Equals(this.X22.Value.Span, target.X22.Value.Span)) return false;
            if ((this.X23 is null) != (target.X23 is null)) return false;
            if (!(this.X23 is null) && !(target.X23 is null) && !global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X23, target.X23)) return false;
            if ((this.X24 is null) != (target.X24 is null)) return false;
            if (!(this.X24 is null) && !(target.X24 is null) && !global::Omnius.Core.Helpers.CollectionHelper.Equals(this.X24, target.X24)) return false;
            if ((this.X25 is null) != (target.X25 is null)) return false;
            if (!(this.X25 is null) && !(target.X25 is null) && this.X25 != target.X25) return false;
            if ((this.X26 is null) != (target.X26 is null)) return false;
            if (!(this.X26 is null) && !(target.X26 is null) && this.X26 != target.X26) return false;
            if ((this.X27 is null) != (target.X27 is null)) return false;
            if (!(this.X27 is null) && !(target.X27 is null) && this.X27 != target.X27) return false;
            if ((this.X28 is null) != (target.X28 is null)) return false;
            if (!(this.X28 is null) && !(target.X28 is null) && this.X28 != target.X28) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class value, in int rank)
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
                    if (value.X27 != null)
                    {
                        propertyCount++;
                    }
                    if (value.X28 != null)
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
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Serialize(ref w, value.X25.Value, rank + 1);
                }
                if (value.X26 != null)
                {
                    w.Write((uint)26);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Serialize(ref w, value.X26.Value, rank + 1);
                }
                if (value.X27 != null)
                {
                    w.Write((uint)27);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Serialize(ref w, value.X27, rank + 1);
                }
                if (value.X28 != null)
                {
                    w.Write((uint)28);
                    global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Serialize(ref w, value.X28, rank + 1);
                }
            }

            public global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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
                global::Omnius.Core.Serialization.RocketPack.Timestamp? p_x20 = null;
                global::System.ReadOnlyMemory<byte>? p_x21 = null;
                global::System.Buffers.IMemoryOwner<byte>? p_x22 = null;
                string[]? p_x23 = null;
                global::System.Collections.Generic.Dictionary<byte, string>? p_x24 = null;
                StructMessageElement_Struct? p_x25 = null;
                TableMessageElement_Struct? p_x26 = null;
                StructMessageElement_Class? p_x27 = null;
                TableMessageElement_Class? p_x28 = null;

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
                                p_x25 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 26:
                            {
                                p_x26 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Struct.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 27:
                            {
                                p_x27 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.StructMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                        case 28:
                            {
                                p_x28 = global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.TableMessageElement_Class.Formatter.Deserialize(ref r, rank + 1);
                                break;
                            }
                    }
                }

                return new global::Omnius.Core.Serialization.OmniPack.CodeGenerator.Internal.NullableTableMessage_Class(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26, p_x27, p_x28);
            }
        }
    }

}
