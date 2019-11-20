
#nullable enable

namespace Omnius.Core.Serialization.RocketPack.CodeGenerator.Internal
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

    internal readonly struct StructMessageElement_Struct : global::Omnius.Core.Serialization.RocketPack.IRocketPackMessage<StructMessageElement_Struct>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<StructMessageElement_Struct> Formatter { get; }
        public static StructMessageElement_Struct Empty { get; }

        static StructMessageElement_Struct()
        {
            StructMessageElement_Struct.Formatter = new ___CustomFormatter();
            StructMessageElement_Struct.Empty = new StructMessageElement_Struct(false);
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

        public static StructMessageElement_Struct Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(StructMessageElement_Struct left, StructMessageElement_Struct right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(StructMessageElement_Struct left, StructMessageElement_Struct right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is StructMessageElement_Struct)) return false;
            return this.Equals((StructMessageElement_Struct)other);
        }
        public bool Equals(StructMessageElement_Struct target)
        {
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<StructMessageElement_Struct>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in StructMessageElement_Struct value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.X0);
            }

            public StructMessageElement_Struct Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                bool p_x0 = false;

                {
                    p_x0 = r.GetBoolean();
                }
                return new StructMessageElement_Struct(p_x0);
            }
        }
    }

    internal readonly struct TableMessageElement_Struct : global::Omnius.Core.Serialization.RocketPack.IRocketPackMessage<TableMessageElement_Struct>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<TableMessageElement_Struct> Formatter { get; }
        public static TableMessageElement_Struct Empty { get; }

        static TableMessageElement_Struct()
        {
            TableMessageElement_Struct.Formatter = new ___CustomFormatter();
            TableMessageElement_Struct.Empty = new TableMessageElement_Struct(false);
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

        public static TableMessageElement_Struct Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(TableMessageElement_Struct left, TableMessageElement_Struct right)
        {
            return right.Equals(left);
        }
        public static bool operator !=(TableMessageElement_Struct left, TableMessageElement_Struct right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is TableMessageElement_Struct)) return false;
            return this.Equals((TableMessageElement_Struct)other);
        }
        public bool Equals(TableMessageElement_Struct target)
        {
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<TableMessageElement_Struct>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in TableMessageElement_Struct value, in int rank)
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

            public TableMessageElement_Struct Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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

                return new TableMessageElement_Struct(p_x0);
            }
        }
    }

    internal sealed partial class StructMessageElement_Class : global::Omnius.Core.Serialization.RocketPack.IRocketPackMessage<StructMessageElement_Class>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<StructMessageElement_Class> Formatter { get; }
        public static StructMessageElement_Class Empty { get; }

        static StructMessageElement_Class()
        {
            StructMessageElement_Class.Formatter = new ___CustomFormatter();
            StructMessageElement_Class.Empty = new StructMessageElement_Class(false);
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

        public static StructMessageElement_Class Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(StructMessageElement_Class? left, StructMessageElement_Class? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(StructMessageElement_Class? left, StructMessageElement_Class? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is StructMessageElement_Class)) return false;
            return this.Equals((StructMessageElement_Class)other);
        }
        public bool Equals(StructMessageElement_Class? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<StructMessageElement_Class>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in StructMessageElement_Class value, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                w.Write(value.X0);
            }

            public StructMessageElement_Class Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                bool p_x0 = false;

                {
                    p_x0 = r.GetBoolean();
                }
                return new StructMessageElement_Class(p_x0);
            }
        }
    }

    internal sealed partial class TableMessageElement_Class : global::Omnius.Core.Serialization.RocketPack.IRocketPackMessage<TableMessageElement_Class>
    {
        public static global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<TableMessageElement_Class> Formatter { get; }
        public static TableMessageElement_Class Empty { get; }

        static TableMessageElement_Class()
        {
            TableMessageElement_Class.Formatter = new ___CustomFormatter();
            TableMessageElement_Class.Empty = new TableMessageElement_Class(false);
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

        public static TableMessageElement_Class Import(global::System.Buffers.ReadOnlySequence<byte> sequence, global::Omnius.Core.IBufferPool<byte> bufferPool)
        {
            var reader = new global::Omnius.Core.Serialization.RocketPack.RocketPackReader(sequence, bufferPool);
            return Formatter.Deserialize(ref reader, 0);
        }
        public void Export(global::System.Buffers.IBufferWriter<byte> bufferWriter, global::Omnius.Core.IBufferPool<byte> bufferPool)
        {
            var writer = new global::Omnius.Core.Serialization.RocketPack.RocketPackWriter(bufferWriter, bufferPool);
            Formatter.Serialize(ref writer, this, 0);
        }

        public static bool operator ==(TableMessageElement_Class? left, TableMessageElement_Class? right)
        {
            return (right is null) ? (left is null) : right.Equals(left);
        }
        public static bool operator !=(TableMessageElement_Class? left, TableMessageElement_Class? right)
        {
            return !(left == right);
        }
        public override bool Equals(object? other)
        {
            if (!(other is TableMessageElement_Class)) return false;
            return this.Equals((TableMessageElement_Class)other);
        }
        public bool Equals(TableMessageElement_Class? target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (this.X0 != target.X0) return false;

            return true;
        }
        public override int GetHashCode() => ___hashCode.Value;

        private sealed class ___CustomFormatter : global::Omnius.Core.Serialization.RocketPack.IRocketPackFormatter<TableMessageElement_Class>
        {
            public void Serialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackWriter w, in TableMessageElement_Class value, in int rank)
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

            public TableMessageElement_Class Deserialize(ref global::Omnius.Core.Serialization.RocketPack.RocketPackReader r, in int rank)
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

                return new TableMessageElement_Class(p_x0);
            }
        }
    }

}
