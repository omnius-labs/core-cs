using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Collections;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FormatterBenchmarks
{
    public sealed partial class RocketPack_BytesMessage : RocketPackMessageBase<RocketPack_BytesMessage>, IDisposable
    {
        static RocketPack_BytesMessage()
        {
            RocketPack_BytesMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxBytesLength = 1048576;

        public RocketPack_BytesMessage(IMemoryOwner<byte> bytes)
        {
            if (bytes is null) throw new ArgumentNullException("bytes");
            if (bytes.Memory.Length > 1048576) throw new ArgumentOutOfRangeException("bytes");

            _bytes = bytes;

            {
                var hashCode = new HashCode();
                if (!this.Bytes.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.Bytes.Span));
                _hashCode = hashCode.ToHashCode();
            }
        }

        private readonly IMemoryOwner<byte> _bytes;
        public ReadOnlyMemory<byte> Bytes => _bytes.Memory;

        public override bool Equals(RocketPack_BytesMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (!BytesOperations.SequenceEqual(this.Bytes.Span, target.Bytes.Span)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        public void Dispose()
        {
            _bytes?.Dispose();
        }

        private sealed class CustomFormatter : IRocketPackFormatter<RocketPack_BytesMessage>
        {
            public void Serialize(RocketPackWriter w, RocketPack_BytesMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (!value.Bytes.IsEmpty) propertyCount++;
                    w.Write(propertyCount);
                }

                // Bytes
                if (!value.Bytes.IsEmpty)
                {
                    w.Write((uint)0);
                    w.Write(value.Bytes.Span);
                }
            }

            public RocketPack_BytesMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IMemoryOwner<byte> p_bytes = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // Bytes
                            {
                                p_bytes = r.GetRecyclableMemory(1048576);
                                break;
                            }
                    }
                }

                return new RocketPack_BytesMessage(p_bytes);
            }
        }
    }

    public sealed partial class RocketPack_IntPropertiesListMessage : RocketPackMessageBase<RocketPack_IntPropertiesListMessage>
    {
        static RocketPack_IntPropertiesListMessage()
        {
            RocketPack_IntPropertiesListMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxListCount = 100000;

        public RocketPack_IntPropertiesListMessage(RocketPack_IntPropertiesMessage[] list)
        {
            if (list is null) throw new ArgumentNullException("list");
            if (list.Length > 100000) throw new ArgumentOutOfRangeException("list");
            foreach (var n in list)
            {
                if (n is null) throw new ArgumentNullException("n");
            }

            this.List = new ReadOnlyListSlim<RocketPack_IntPropertiesMessage>(list);

            {
                var hashCode = new HashCode();
                foreach (var n in this.List)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ReadOnlyListSlim<RocketPack_IntPropertiesMessage> List { get; }

        public override bool Equals(RocketPack_IntPropertiesListMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.List is null) != (target.List is null)) return false;
            if (!(this.List is null) && !(target.List is null) && !CollectionHelper.Equals(this.List, target.List)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<RocketPack_IntPropertiesListMessage>
        {
            public void Serialize(RocketPackWriter w, RocketPack_IntPropertiesListMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.List.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // List
                if (value.List.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.List.Count);
                    foreach (var n in value.List)
                    {
                        RocketPack_IntPropertiesMessage.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public RocketPack_IntPropertiesListMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                RocketPack_IntPropertiesMessage[] p_list = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // List
                            {
                                var length = r.GetUInt32();
                                p_list = new RocketPack_IntPropertiesMessage[length];
                                for (int i = 0; i < p_list.Length; i++)
                                {
                                    p_list[i] = RocketPack_IntPropertiesMessage.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new RocketPack_IntPropertiesListMessage(p_list);
            }
        }
    }

    public sealed partial class RocketPack_StringPropertiesListMessage : RocketPackMessageBase<RocketPack_StringPropertiesListMessage>
    {
        static RocketPack_StringPropertiesListMessage()
        {
            RocketPack_StringPropertiesListMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxListCount = 100000;

        public RocketPack_StringPropertiesListMessage(RocketPack_StringPropertiesMessage[] list)
        {
            if (list is null) throw new ArgumentNullException("list");
            if (list.Length > 100000) throw new ArgumentOutOfRangeException("list");
            foreach (var n in list)
            {
                if (n is null) throw new ArgumentNullException("n");
            }

            this.List = new ReadOnlyListSlim<RocketPack_StringPropertiesMessage>(list);

            {
                var hashCode = new HashCode();
                foreach (var n in this.List)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public ReadOnlyListSlim<RocketPack_StringPropertiesMessage> List { get; }

        public override bool Equals(RocketPack_StringPropertiesListMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.List is null) != (target.List is null)) return false;
            if (!(this.List is null) && !(target.List is null) && !CollectionHelper.Equals(this.List, target.List)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<RocketPack_StringPropertiesListMessage>
        {
            public void Serialize(RocketPackWriter w, RocketPack_StringPropertiesListMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.List.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // List
                if (value.List.Count != 0)
                {
                    w.Write((uint)0);
                    w.Write((uint)value.List.Count);
                    foreach (var n in value.List)
                    {
                        RocketPack_StringPropertiesMessage.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public RocketPack_StringPropertiesListMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                RocketPack_StringPropertiesMessage[] p_list = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // List
                            {
                                var length = r.GetUInt32();
                                p_list = new RocketPack_StringPropertiesMessage[length];
                                for (int i = 0; i < p_list.Length; i++)
                                {
                                    p_list[i] = RocketPack_StringPropertiesMessage.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new RocketPack_StringPropertiesListMessage(p_list);
            }
        }
    }

    public sealed partial class RocketPack_IntPropertiesMessage : RocketPackMessageBase<RocketPack_IntPropertiesMessage>
    {
        static RocketPack_IntPropertiesMessage()
        {
            RocketPack_IntPropertiesMessage.Formatter = new CustomFormatter();
        }

        public RocketPack_IntPropertiesMessage(uint myProperty1, uint myProperty2, uint myProperty3, uint myProperty4, uint myProperty5, uint myProperty6, uint myProperty7, uint myProperty8, uint myProperty9)
        {
            this.MyProperty1 = myProperty1;
            this.MyProperty2 = myProperty2;
            this.MyProperty3 = myProperty3;
            this.MyProperty4 = myProperty4;
            this.MyProperty5 = myProperty5;
            this.MyProperty6 = myProperty6;
            this.MyProperty7 = myProperty7;
            this.MyProperty8 = myProperty8;
            this.MyProperty9 = myProperty9;

            {
                var hashCode = new HashCode();
                if (this.MyProperty1 != default) hashCode.Add(this.MyProperty1.GetHashCode());
                if (this.MyProperty2 != default) hashCode.Add(this.MyProperty2.GetHashCode());
                if (this.MyProperty3 != default) hashCode.Add(this.MyProperty3.GetHashCode());
                if (this.MyProperty4 != default) hashCode.Add(this.MyProperty4.GetHashCode());
                if (this.MyProperty5 != default) hashCode.Add(this.MyProperty5.GetHashCode());
                if (this.MyProperty6 != default) hashCode.Add(this.MyProperty6.GetHashCode());
                if (this.MyProperty7 != default) hashCode.Add(this.MyProperty7.GetHashCode());
                if (this.MyProperty8 != default) hashCode.Add(this.MyProperty8.GetHashCode());
                if (this.MyProperty9 != default) hashCode.Add(this.MyProperty9.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public uint MyProperty1 { get; }
        public uint MyProperty2 { get; }
        public uint MyProperty3 { get; }
        public uint MyProperty4 { get; }
        public uint MyProperty5 { get; }
        public uint MyProperty6 { get; }
        public uint MyProperty7 { get; }
        public uint MyProperty8 { get; }
        public uint MyProperty9 { get; }

        public override bool Equals(RocketPack_IntPropertiesMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.MyProperty1 != target.MyProperty1) return false;
            if (this.MyProperty2 != target.MyProperty2) return false;
            if (this.MyProperty3 != target.MyProperty3) return false;
            if (this.MyProperty4 != target.MyProperty4) return false;
            if (this.MyProperty5 != target.MyProperty5) return false;
            if (this.MyProperty6 != target.MyProperty6) return false;
            if (this.MyProperty7 != target.MyProperty7) return false;
            if (this.MyProperty8 != target.MyProperty8) return false;
            if (this.MyProperty9 != target.MyProperty9) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<RocketPack_IntPropertiesMessage>
        {
            public void Serialize(RocketPackWriter w, RocketPack_IntPropertiesMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.MyProperty1 != default) propertyCount++;
                    if (value.MyProperty2 != default) propertyCount++;
                    if (value.MyProperty3 != default) propertyCount++;
                    if (value.MyProperty4 != default) propertyCount++;
                    if (value.MyProperty5 != default) propertyCount++;
                    if (value.MyProperty6 != default) propertyCount++;
                    if (value.MyProperty7 != default) propertyCount++;
                    if (value.MyProperty8 != default) propertyCount++;
                    if (value.MyProperty9 != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // MyProperty1
                if (value.MyProperty1 != default)
                {
                    w.Write((uint)0);
                    w.Write(value.MyProperty1);
                }
                // MyProperty2
                if (value.MyProperty2 != default)
                {
                    w.Write((uint)1);
                    w.Write(value.MyProperty2);
                }
                // MyProperty3
                if (value.MyProperty3 != default)
                {
                    w.Write((uint)2);
                    w.Write(value.MyProperty3);
                }
                // MyProperty4
                if (value.MyProperty4 != default)
                {
                    w.Write((uint)3);
                    w.Write(value.MyProperty4);
                }
                // MyProperty5
                if (value.MyProperty5 != default)
                {
                    w.Write((uint)4);
                    w.Write(value.MyProperty5);
                }
                // MyProperty6
                if (value.MyProperty6 != default)
                {
                    w.Write((uint)5);
                    w.Write(value.MyProperty6);
                }
                // MyProperty7
                if (value.MyProperty7 != default)
                {
                    w.Write((uint)6);
                    w.Write(value.MyProperty7);
                }
                // MyProperty8
                if (value.MyProperty8 != default)
                {
                    w.Write((uint)7);
                    w.Write(value.MyProperty8);
                }
                // MyProperty9
                if (value.MyProperty9 != default)
                {
                    w.Write((uint)8);
                    w.Write(value.MyProperty9);
                }
            }

            public RocketPack_IntPropertiesMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                uint p_myProperty1 = default;
                uint p_myProperty2 = default;
                uint p_myProperty3 = default;
                uint p_myProperty4 = default;
                uint p_myProperty5 = default;
                uint p_myProperty6 = default;
                uint p_myProperty7 = default;
                uint p_myProperty8 = default;
                uint p_myProperty9 = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // MyProperty1
                            {
                                p_myProperty1 = r.GetUInt32();
                                break;
                            }
                        case 1: // MyProperty2
                            {
                                p_myProperty2 = r.GetUInt32();
                                break;
                            }
                        case 2: // MyProperty3
                            {
                                p_myProperty3 = r.GetUInt32();
                                break;
                            }
                        case 3: // MyProperty4
                            {
                                p_myProperty4 = r.GetUInt32();
                                break;
                            }
                        case 4: // MyProperty5
                            {
                                p_myProperty5 = r.GetUInt32();
                                break;
                            }
                        case 5: // MyProperty6
                            {
                                p_myProperty6 = r.GetUInt32();
                                break;
                            }
                        case 6: // MyProperty7
                            {
                                p_myProperty7 = r.GetUInt32();
                                break;
                            }
                        case 7: // MyProperty8
                            {
                                p_myProperty8 = r.GetUInt32();
                                break;
                            }
                        case 8: // MyProperty9
                            {
                                p_myProperty9 = r.GetUInt32();
                                break;
                            }
                    }
                }

                return new RocketPack_IntPropertiesMessage(p_myProperty1, p_myProperty2, p_myProperty3, p_myProperty4, p_myProperty5, p_myProperty6, p_myProperty7, p_myProperty8, p_myProperty9);
            }
        }
    }

    public sealed partial class RocketPack_StringPropertiesMessage : RocketPackMessageBase<RocketPack_StringPropertiesMessage>
    {
        static RocketPack_StringPropertiesMessage()
        {
            RocketPack_StringPropertiesMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxMyProperty1Length = 8192;
        public static readonly int MaxMyProperty2Length = 8192;
        public static readonly int MaxMyProperty3Length = 8192;

        public RocketPack_StringPropertiesMessage(string myProperty1, string myProperty2, string myProperty3)
        {
            if (myProperty1 is null) throw new ArgumentNullException("myProperty1");
            if (myProperty1.Length > 8192) throw new ArgumentOutOfRangeException("myProperty1");
            if (myProperty2 is null) throw new ArgumentNullException("myProperty2");
            if (myProperty2.Length > 8192) throw new ArgumentOutOfRangeException("myProperty2");
            if (myProperty3 is null) throw new ArgumentNullException("myProperty3");
            if (myProperty3.Length > 8192) throw new ArgumentOutOfRangeException("myProperty3");

            this.MyProperty1 = myProperty1;
            this.MyProperty2 = myProperty2;
            this.MyProperty3 = myProperty3;

            {
                var hashCode = new HashCode();
                if (this.MyProperty1 != default) hashCode.Add(this.MyProperty1.GetHashCode());
                if (this.MyProperty2 != default) hashCode.Add(this.MyProperty2.GetHashCode());
                if (this.MyProperty3 != default) hashCode.Add(this.MyProperty3.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public string MyProperty1 { get; }
        public string MyProperty2 { get; }
        public string MyProperty3 { get; }

        public override bool Equals(RocketPack_StringPropertiesMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if (this.MyProperty1 != target.MyProperty1) return false;
            if (this.MyProperty2 != target.MyProperty2) return false;
            if (this.MyProperty3 != target.MyProperty3) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<RocketPack_StringPropertiesMessage>
        {
            public void Serialize(RocketPackWriter w, RocketPack_StringPropertiesMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.MyProperty1 != default) propertyCount++;
                    if (value.MyProperty2 != default) propertyCount++;
                    if (value.MyProperty3 != default) propertyCount++;
                    w.Write(propertyCount);
                }

                // MyProperty1
                if (value.MyProperty1 != default)
                {
                    w.Write((uint)0);
                    w.Write(value.MyProperty1);
                }
                // MyProperty2
                if (value.MyProperty2 != default)
                {
                    w.Write((uint)1);
                    w.Write(value.MyProperty2);
                }
                // MyProperty3
                if (value.MyProperty3 != default)
                {
                    w.Write((uint)2);
                    w.Write(value.MyProperty3);
                }
            }

            public RocketPack_StringPropertiesMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                string p_myProperty1 = default;
                string p_myProperty2 = default;
                string p_myProperty3 = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // MyProperty1
                            {
                                p_myProperty1 = r.GetString(8192);
                                break;
                            }
                        case 1: // MyProperty2
                            {
                                p_myProperty2 = r.GetString(8192);
                                break;
                            }
                        case 2: // MyProperty3
                            {
                                p_myProperty3 = r.GetString(8192);
                                break;
                            }
                    }
                }

                return new RocketPack_StringPropertiesMessage(p_myProperty1, p_myProperty2, p_myProperty3);
            }
        }
    }

}
