
#nullable enable

namespace FormatterBenchmarks
{
    internal sealed partial class RocketPack_BytesElementsList : global::Omnix.Serialization.RocketPack.RocketPackMessageBase<RocketPack_BytesElementsList>
    {
        static RocketPack_BytesElementsList()
        {
            RocketPack_BytesElementsList.Formatter = new CustomFormatter();
            RocketPack_BytesElementsList.Empty = new RocketPack_BytesElementsList(global::System.Array.Empty<RocketPack_BytesElements>());
        }

        private readonly int __hashCode;

        public static readonly int MaxListCount = 1048576;

        public RocketPack_BytesElementsList(RocketPack_BytesElements[] list)
        {
            if (list is null) throw new global::System.ArgumentNullException("list");
            if (list.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("list");
            foreach (var n in list)
            {
                if (n is null) throw new global::System.ArgumentNullException("n");
            }

            this.List = new global::Omnix.DataStructures.ReadOnlyListSlim<RocketPack_BytesElements>(list);

            {
                var __h = new global::System.HashCode();
                foreach (var n in this.List)
                {
                    if (n != default) __h.Add(n.GetHashCode());
                }
                __hashCode = __h.ToHashCode();
            }
        }

        public global::Omnix.DataStructures.ReadOnlyListSlim<RocketPack_BytesElements> List { get; }

        public override bool Equals(RocketPack_BytesElementsList target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.List, target.List)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<RocketPack_BytesElementsList>
        {
            public void Serialize(global::Omnix.Serialization.RocketPack.RocketPackWriter w, RocketPack_BytesElementsList value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.List.Count != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.List.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.List.Count);
                    foreach (var n in value.List)
                    {
                        RocketPack_BytesElements.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public RocketPack_BytesElementsList Deserialize(global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                RocketPack_BytesElements[] p_list = global::System.Array.Empty<RocketPack_BytesElements>();

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                var length = r.GetUInt32();
                                p_list = new RocketPack_BytesElements[length];
                                for (int i = 0; i < p_list.Length; i++)
                                {
                                    p_list[i] = RocketPack_BytesElements.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new RocketPack_BytesElementsList(p_list);
            }
        }
    }

    internal sealed partial class RocketPack_BytesElements : global::Omnix.Serialization.RocketPack.RocketPackMessageBase<RocketPack_BytesElements>, global::System.IDisposable
    {
        static RocketPack_BytesElements()
        {
            RocketPack_BytesElements.Formatter = new CustomFormatter();
            RocketPack_BytesElements.Empty = new RocketPack_BytesElements(global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty, global::Omnix.Base.SimpleMemoryOwner<byte>.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxX0Length = 1048576;
        public static readonly int MaxX1Length = 1048576;
        public static readonly int MaxX2Length = 1048576;
        public static readonly int MaxX3Length = 1048576;
        public static readonly int MaxX4Length = 1048576;
        public static readonly int MaxX5Length = 1048576;
        public static readonly int MaxX6Length = 1048576;
        public static readonly int MaxX7Length = 1048576;
        public static readonly int MaxX8Length = 1048576;
        public static readonly int MaxX9Length = 1048576;

        public RocketPack_BytesElements(global::System.Buffers.IMemoryOwner<byte> x0, global::System.Buffers.IMemoryOwner<byte> x1, global::System.Buffers.IMemoryOwner<byte> x2, global::System.Buffers.IMemoryOwner<byte> x3, global::System.Buffers.IMemoryOwner<byte> x4, global::System.Buffers.IMemoryOwner<byte> x5, global::System.Buffers.IMemoryOwner<byte> x6, global::System.Buffers.IMemoryOwner<byte> x7, global::System.Buffers.IMemoryOwner<byte> x8, global::System.Buffers.IMemoryOwner<byte> x9)
        {
            if (x0 is null) throw new global::System.ArgumentNullException("x0");
            if (x0.Memory.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("x0");
            if (x1 is null) throw new global::System.ArgumentNullException("x1");
            if (x1.Memory.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("x1");
            if (x2 is null) throw new global::System.ArgumentNullException("x2");
            if (x2.Memory.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("x2");
            if (x3 is null) throw new global::System.ArgumentNullException("x3");
            if (x3.Memory.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("x3");
            if (x4 is null) throw new global::System.ArgumentNullException("x4");
            if (x4.Memory.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("x4");
            if (x5 is null) throw new global::System.ArgumentNullException("x5");
            if (x5.Memory.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("x5");
            if (x6 is null) throw new global::System.ArgumentNullException("x6");
            if (x6.Memory.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("x6");
            if (x7 is null) throw new global::System.ArgumentNullException("x7");
            if (x7.Memory.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("x7");
            if (x8 is null) throw new global::System.ArgumentNullException("x8");
            if (x8.Memory.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("x8");
            if (x9 is null) throw new global::System.ArgumentNullException("x9");
            if (x9.Memory.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("x9");

            _x0 = x0;
            _x1 = x1;
            _x2 = x2;
            _x3 = x3;
            _x4 = x4;
            _x5 = x5;
            _x6 = x6;
            _x7 = x7;
            _x8 = x8;
            _x9 = x9;

            {
                var __h = new global::System.HashCode();
                if (!this.X0.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X0.Span));
                if (!this.X1.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X1.Span));
                if (!this.X2.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X2.Span));
                if (!this.X3.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X3.Span));
                if (!this.X4.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X4.Span));
                if (!this.X5.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X5.Span));
                if (!this.X6.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X6.Span));
                if (!this.X7.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X7.Span));
                if (!this.X8.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X8.Span));
                if (!this.X9.IsEmpty) __h.Add(global::Omnix.Base.Helpers.ObjectHelper.GetHashCode(this.X9.Span));
                __hashCode = __h.ToHashCode();
            }
        }

        private readonly global::System.Buffers.IMemoryOwner<byte> _x0;
        public global::System.ReadOnlyMemory<byte> X0 => _x0.Memory;
        private readonly global::System.Buffers.IMemoryOwner<byte> _x1;
        public global::System.ReadOnlyMemory<byte> X1 => _x1.Memory;
        private readonly global::System.Buffers.IMemoryOwner<byte> _x2;
        public global::System.ReadOnlyMemory<byte> X2 => _x2.Memory;
        private readonly global::System.Buffers.IMemoryOwner<byte> _x3;
        public global::System.ReadOnlyMemory<byte> X3 => _x3.Memory;
        private readonly global::System.Buffers.IMemoryOwner<byte> _x4;
        public global::System.ReadOnlyMemory<byte> X4 => _x4.Memory;
        private readonly global::System.Buffers.IMemoryOwner<byte> _x5;
        public global::System.ReadOnlyMemory<byte> X5 => _x5.Memory;
        private readonly global::System.Buffers.IMemoryOwner<byte> _x6;
        public global::System.ReadOnlyMemory<byte> X6 => _x6.Memory;
        private readonly global::System.Buffers.IMemoryOwner<byte> _x7;
        public global::System.ReadOnlyMemory<byte> X7 => _x7.Memory;
        private readonly global::System.Buffers.IMemoryOwner<byte> _x8;
        public global::System.ReadOnlyMemory<byte> X8 => _x8.Memory;
        private readonly global::System.Buffers.IMemoryOwner<byte> _x9;
        public global::System.ReadOnlyMemory<byte> X9 => _x9.Memory;

        public override bool Equals(RocketPack_BytesElements target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X0.Span, target.X0.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X1.Span, target.X1.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X2.Span, target.X2.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X3.Span, target.X3.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X4.Span, target.X4.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X5.Span, target.X5.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X6.Span, target.X6.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X7.Span, target.X7.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X8.Span, target.X8.Span)) return false;
            if (!global::Omnix.Base.BytesOperations.SequenceEqual(this.X9.Span, target.X9.Span)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        public void Dispose()
        {
            _x0?.Dispose();
            _x1?.Dispose();
            _x2?.Dispose();
            _x3?.Dispose();
            _x4?.Dispose();
            _x5?.Dispose();
            _x6?.Dispose();
            _x7?.Dispose();
            _x8?.Dispose();
            _x9?.Dispose();
        }

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<RocketPack_BytesElements>
        {
            public void Serialize(global::Omnix.Serialization.RocketPack.RocketPackWriter w, RocketPack_BytesElements value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (!value.X0.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.X1.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.X2.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.X3.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.X4.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.X5.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.X6.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.X7.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.X8.IsEmpty)
                    {
                        propertyCount++;
                    }
                    if (!value.X9.IsEmpty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (!value.X0.IsEmpty)
                {
                    w.Write((uint)0);
                    w.Write(value.X0.Span);
                }
                if (!value.X1.IsEmpty)
                {
                    w.Write((uint)1);
                    w.Write(value.X1.Span);
                }
                if (!value.X2.IsEmpty)
                {
                    w.Write((uint)2);
                    w.Write(value.X2.Span);
                }
                if (!value.X3.IsEmpty)
                {
                    w.Write((uint)3);
                    w.Write(value.X3.Span);
                }
                if (!value.X4.IsEmpty)
                {
                    w.Write((uint)4);
                    w.Write(value.X4.Span);
                }
                if (!value.X5.IsEmpty)
                {
                    w.Write((uint)5);
                    w.Write(value.X5.Span);
                }
                if (!value.X6.IsEmpty)
                {
                    w.Write((uint)6);
                    w.Write(value.X6.Span);
                }
                if (!value.X7.IsEmpty)
                {
                    w.Write((uint)7);
                    w.Write(value.X7.Span);
                }
                if (!value.X8.IsEmpty)
                {
                    w.Write((uint)8);
                    w.Write(value.X8.Span);
                }
                if (!value.X9.IsEmpty)
                {
                    w.Write((uint)9);
                    w.Write(value.X9.Span);
                }
            }

            public RocketPack_BytesElements Deserialize(global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                global::System.Buffers.IMemoryOwner<byte> p_x0 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x1 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x2 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x3 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x4 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x5 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x6 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x7 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x8 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;
                global::System.Buffers.IMemoryOwner<byte> p_x9 = global::Omnix.Base.SimpleMemoryOwner<byte>.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_x0 = r.GetRecyclableMemory(1048576);
                                break;
                            }
                        case 1:
                            {
                                p_x1 = r.GetRecyclableMemory(1048576);
                                break;
                            }
                        case 2:
                            {
                                p_x2 = r.GetRecyclableMemory(1048576);
                                break;
                            }
                        case 3:
                            {
                                p_x3 = r.GetRecyclableMemory(1048576);
                                break;
                            }
                        case 4:
                            {
                                p_x4 = r.GetRecyclableMemory(1048576);
                                break;
                            }
                        case 5:
                            {
                                p_x5 = r.GetRecyclableMemory(1048576);
                                break;
                            }
                        case 6:
                            {
                                p_x6 = r.GetRecyclableMemory(1048576);
                                break;
                            }
                        case 7:
                            {
                                p_x7 = r.GetRecyclableMemory(1048576);
                                break;
                            }
                        case 8:
                            {
                                p_x8 = r.GetRecyclableMemory(1048576);
                                break;
                            }
                        case 9:
                            {
                                p_x9 = r.GetRecyclableMemory(1048576);
                                break;
                            }
                    }
                }

                return new RocketPack_BytesElements(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9);
            }
        }
    }

    internal sealed partial class RocketPack_IntElementsList : global::Omnix.Serialization.RocketPack.RocketPackMessageBase<RocketPack_IntElementsList>
    {
        static RocketPack_IntElementsList()
        {
            RocketPack_IntElementsList.Formatter = new CustomFormatter();
            RocketPack_IntElementsList.Empty = new RocketPack_IntElementsList(global::System.Array.Empty<RocketPack_IntElements>());
        }

        private readonly int __hashCode;

        public static readonly int MaxListCount = 1048576;

        public RocketPack_IntElementsList(RocketPack_IntElements[] list)
        {
            if (list is null) throw new global::System.ArgumentNullException("list");
            if (list.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("list");
            foreach (var n in list)
            {
                if (n is null) throw new global::System.ArgumentNullException("n");
            }

            this.List = new global::Omnix.DataStructures.ReadOnlyListSlim<RocketPack_IntElements>(list);

            {
                var __h = new global::System.HashCode();
                foreach (var n in this.List)
                {
                    if (n != default) __h.Add(n.GetHashCode());
                }
                __hashCode = __h.ToHashCode();
            }
        }

        public global::Omnix.DataStructures.ReadOnlyListSlim<RocketPack_IntElements> List { get; }

        public override bool Equals(RocketPack_IntElementsList target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.List, target.List)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<RocketPack_IntElementsList>
        {
            public void Serialize(global::Omnix.Serialization.RocketPack.RocketPackWriter w, RocketPack_IntElementsList value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.List.Count != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.List.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.List.Count);
                    foreach (var n in value.List)
                    {
                        RocketPack_IntElements.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public RocketPack_IntElementsList Deserialize(global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                RocketPack_IntElements[] p_list = global::System.Array.Empty<RocketPack_IntElements>();

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                var length = r.GetUInt32();
                                p_list = new RocketPack_IntElements[length];
                                for (int i = 0; i < p_list.Length; i++)
                                {
                                    p_list[i] = RocketPack_IntElements.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new RocketPack_IntElementsList(p_list);
            }
        }
    }

    internal sealed partial class RocketPack_IntElements : global::Omnix.Serialization.RocketPack.RocketPackMessageBase<RocketPack_IntElements>
    {
        static RocketPack_IntElements()
        {
            RocketPack_IntElements.Formatter = new CustomFormatter();
            RocketPack_IntElements.Empty = new RocketPack_IntElements(0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        private readonly int __hashCode;

        public RocketPack_IntElements(uint x0, uint x1, uint x2, uint x3, uint x4, uint x5, uint x6, uint x7, uint x8, uint x9)
        {
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

            {
                var __h = new global::System.HashCode();
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
                __hashCode = __h.ToHashCode();
            }
        }

        public uint X0 { get; }
        public uint X1 { get; }
        public uint X2 { get; }
        public uint X3 { get; }
        public uint X4 { get; }
        public uint X5 { get; }
        public uint X6 { get; }
        public uint X7 { get; }
        public uint X8 { get; }
        public uint X9 { get; }

        public override bool Equals(RocketPack_IntElements target)
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

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<RocketPack_IntElements>
        {
            public void Serialize(global::Omnix.Serialization.RocketPack.RocketPackWriter w, RocketPack_IntElements value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.X0 != 0)
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
                    if (value.X9 != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.X0 != 0)
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
                if (value.X9 != 0)
                {
                    w.Write((uint)9);
                    w.Write(value.X9);
                }
            }

            public RocketPack_IntElements Deserialize(global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                uint p_x0 = 0;
                uint p_x1 = 0;
                uint p_x2 = 0;
                uint p_x3 = 0;
                uint p_x4 = 0;
                uint p_x5 = 0;
                uint p_x6 = 0;
                uint p_x7 = 0;
                uint p_x8 = 0;
                uint p_x9 = 0;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_x0 = r.GetUInt32();
                                break;
                            }
                        case 1:
                            {
                                p_x1 = r.GetUInt32();
                                break;
                            }
                        case 2:
                            {
                                p_x2 = r.GetUInt32();
                                break;
                            }
                        case 3:
                            {
                                p_x3 = r.GetUInt32();
                                break;
                            }
                        case 4:
                            {
                                p_x4 = r.GetUInt32();
                                break;
                            }
                        case 5:
                            {
                                p_x5 = r.GetUInt32();
                                break;
                            }
                        case 6:
                            {
                                p_x6 = r.GetUInt32();
                                break;
                            }
                        case 7:
                            {
                                p_x7 = r.GetUInt32();
                                break;
                            }
                        case 8:
                            {
                                p_x8 = r.GetUInt32();
                                break;
                            }
                        case 9:
                            {
                                p_x9 = r.GetUInt32();
                                break;
                            }
                    }
                }

                return new RocketPack_IntElements(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9);
            }
        }
    }

    internal sealed partial class RocketPack_StringElementsList : global::Omnix.Serialization.RocketPack.RocketPackMessageBase<RocketPack_StringElementsList>
    {
        static RocketPack_StringElementsList()
        {
            RocketPack_StringElementsList.Formatter = new CustomFormatter();
            RocketPack_StringElementsList.Empty = new RocketPack_StringElementsList(global::System.Array.Empty<RocketPack_StringElements>());
        }

        private readonly int __hashCode;

        public static readonly int MaxListCount = 1048576;

        public RocketPack_StringElementsList(RocketPack_StringElements[] list)
        {
            if (list is null) throw new global::System.ArgumentNullException("list");
            if (list.Length > 1048576) throw new global::System.ArgumentOutOfRangeException("list");
            foreach (var n in list)
            {
                if (n is null) throw new global::System.ArgumentNullException("n");
            }

            this.List = new global::Omnix.DataStructures.ReadOnlyListSlim<RocketPack_StringElements>(list);

            {
                var __h = new global::System.HashCode();
                foreach (var n in this.List)
                {
                    if (n != default) __h.Add(n.GetHashCode());
                }
                __hashCode = __h.ToHashCode();
            }
        }

        public global::Omnix.DataStructures.ReadOnlyListSlim<RocketPack_StringElements> List { get; }

        public override bool Equals(RocketPack_StringElementsList target)
        {
            if (target is null) return false;
            if (object.ReferenceEquals(this, target)) return true;
            if (!global::Omnix.Base.Helpers.CollectionHelper.Equals(this.List, target.List)) return false;

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<RocketPack_StringElementsList>
        {
            public void Serialize(global::Omnix.Serialization.RocketPack.RocketPackWriter w, RocketPack_StringElementsList value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.List.Count != 0)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.List.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.List.Count);
                    foreach (var n in value.List)
                    {
                        RocketPack_StringElements.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public RocketPack_StringElementsList Deserialize(global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                RocketPack_StringElements[] p_list = global::System.Array.Empty<RocketPack_StringElements>();

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                var length = r.GetUInt32();
                                p_list = new RocketPack_StringElements[length];
                                for (int i = 0; i < p_list.Length; i++)
                                {
                                    p_list[i] = RocketPack_StringElements.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new RocketPack_StringElementsList(p_list);
            }
        }
    }

    internal sealed partial class RocketPack_StringElements : global::Omnix.Serialization.RocketPack.RocketPackMessageBase<RocketPack_StringElements>
    {
        static RocketPack_StringElements()
        {
            RocketPack_StringElements.Formatter = new CustomFormatter();
            RocketPack_StringElements.Empty = new RocketPack_StringElements(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        private readonly int __hashCode;

        public static readonly int MaxX0Length = 8192;
        public static readonly int MaxX1Length = 8192;
        public static readonly int MaxX2Length = 8192;
        public static readonly int MaxX3Length = 8192;
        public static readonly int MaxX4Length = 8192;
        public static readonly int MaxX5Length = 8192;
        public static readonly int MaxX6Length = 8192;
        public static readonly int MaxX7Length = 8192;
        public static readonly int MaxX8Length = 8192;
        public static readonly int MaxX9Length = 8192;

        public RocketPack_StringElements(string x0, string x1, string x2, string x3, string x4, string x5, string x6, string x7, string x8, string x9)
        {
            if (x0 is null) throw new global::System.ArgumentNullException("x0");
            if (x0.Length > 8192) throw new global::System.ArgumentOutOfRangeException("x0");
            if (x1 is null) throw new global::System.ArgumentNullException("x1");
            if (x1.Length > 8192) throw new global::System.ArgumentOutOfRangeException("x1");
            if (x2 is null) throw new global::System.ArgumentNullException("x2");
            if (x2.Length > 8192) throw new global::System.ArgumentOutOfRangeException("x2");
            if (x3 is null) throw new global::System.ArgumentNullException("x3");
            if (x3.Length > 8192) throw new global::System.ArgumentOutOfRangeException("x3");
            if (x4 is null) throw new global::System.ArgumentNullException("x4");
            if (x4.Length > 8192) throw new global::System.ArgumentOutOfRangeException("x4");
            if (x5 is null) throw new global::System.ArgumentNullException("x5");
            if (x5.Length > 8192) throw new global::System.ArgumentOutOfRangeException("x5");
            if (x6 is null) throw new global::System.ArgumentNullException("x6");
            if (x6.Length > 8192) throw new global::System.ArgumentOutOfRangeException("x6");
            if (x7 is null) throw new global::System.ArgumentNullException("x7");
            if (x7.Length > 8192) throw new global::System.ArgumentOutOfRangeException("x7");
            if (x8 is null) throw new global::System.ArgumentNullException("x8");
            if (x8.Length > 8192) throw new global::System.ArgumentOutOfRangeException("x8");
            if (x9 is null) throw new global::System.ArgumentNullException("x9");
            if (x9.Length > 8192) throw new global::System.ArgumentOutOfRangeException("x9");

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

            {
                var __h = new global::System.HashCode();
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
                __hashCode = __h.ToHashCode();
            }
        }

        public string X0 { get; }
        public string X1 { get; }
        public string X2 { get; }
        public string X3 { get; }
        public string X4 { get; }
        public string X5 { get; }
        public string X6 { get; }
        public string X7 { get; }
        public string X8 { get; }
        public string X9 { get; }

        public override bool Equals(RocketPack_StringElements target)
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

            return true;
        }

        public override int GetHashCode() => __hashCode;

        private sealed class CustomFormatter : global::Omnix.Serialization.RocketPack.IRocketPackFormatter<RocketPack_StringElements>
        {
            public void Serialize(global::Omnix.Serialization.RocketPack.RocketPackWriter w, RocketPack_StringElements value, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                {
                    uint propertyCount = 0;
                    if (value.X0 != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X1 != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X2 != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X3 != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X4 != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X5 != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X6 != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X7 != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X8 != string.Empty)
                    {
                        propertyCount++;
                    }
                    if (value.X9 != string.Empty)
                    {
                        propertyCount++;
                    }
                    w.Write(propertyCount);
                }

                if (value.X0 != string.Empty)
                {
                    w.Write((uint)0);
                    w.Write(value.X0);
                }
                if (value.X1 != string.Empty)
                {
                    w.Write((uint)1);
                    w.Write(value.X1);
                }
                if (value.X2 != string.Empty)
                {
                    w.Write((uint)2);
                    w.Write(value.X2);
                }
                if (value.X3 != string.Empty)
                {
                    w.Write((uint)3);
                    w.Write(value.X3);
                }
                if (value.X4 != string.Empty)
                {
                    w.Write((uint)4);
                    w.Write(value.X4);
                }
                if (value.X5 != string.Empty)
                {
                    w.Write((uint)5);
                    w.Write(value.X5);
                }
                if (value.X6 != string.Empty)
                {
                    w.Write((uint)6);
                    w.Write(value.X6);
                }
                if (value.X7 != string.Empty)
                {
                    w.Write((uint)7);
                    w.Write(value.X7);
                }
                if (value.X8 != string.Empty)
                {
                    w.Write((uint)8);
                    w.Write(value.X8);
                }
                if (value.X9 != string.Empty)
                {
                    w.Write((uint)9);
                    w.Write(value.X9);
                }
            }

            public RocketPack_StringElements Deserialize(global::Omnix.Serialization.RocketPack.RocketPackReader r, int rank)
            {
                if (rank > 256) throw new global::System.FormatException();

                uint propertyCount = r.GetUInt32();

                string p_x0 = string.Empty;
                string p_x1 = string.Empty;
                string p_x2 = string.Empty;
                string p_x3 = string.Empty;
                string p_x4 = string.Empty;
                string p_x5 = string.Empty;
                string p_x6 = string.Empty;
                string p_x7 = string.Empty;
                string p_x8 = string.Empty;
                string p_x9 = string.Empty;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0:
                            {
                                p_x0 = r.GetString(8192);
                                break;
                            }
                        case 1:
                            {
                                p_x1 = r.GetString(8192);
                                break;
                            }
                        case 2:
                            {
                                p_x2 = r.GetString(8192);
                                break;
                            }
                        case 3:
                            {
                                p_x3 = r.GetString(8192);
                                break;
                            }
                        case 4:
                            {
                                p_x4 = r.GetString(8192);
                                break;
                            }
                        case 5:
                            {
                                p_x5 = r.GetString(8192);
                                break;
                            }
                        case 6:
                            {
                                p_x6 = r.GetString(8192);
                                break;
                            }
                        case 7:
                            {
                                p_x7 = r.GetString(8192);
                                break;
                            }
                        case 8:
                            {
                                p_x8 = r.GetString(8192);
                                break;
                            }
                        case 9:
                            {
                                p_x9 = r.GetString(8192);
                                break;
                            }
                    }
                }

                return new RocketPack_StringElements(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9);
            }
        }
    }

}
