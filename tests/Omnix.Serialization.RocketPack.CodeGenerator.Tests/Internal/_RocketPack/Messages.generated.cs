using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Serialization.RocketPack.CodeGenerator.Tests.Internal
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

    internal sealed partial class HelloMessage : RocketPackMessageBase<HelloMessage>, IDisposable
    {
        static HelloMessage()
        {
            HelloMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public HelloMessage(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, Timestamp x20, ReadOnlyMemory<byte> x21, IMemoryOwner<byte> x22, IList<string> x23, IDictionary<byte, string> x24)
        {
            if (x19 is null) throw new ArgumentNullException("x19");
            if (x19.Length > 128) throw new ArgumentOutOfRangeException("x19");
            if (x21.Length > 256) throw new ArgumentOutOfRangeException("x21");
            if (x22 is null) throw new ArgumentNullException("x22");
            if (x22.Memory.Length > 256) throw new ArgumentOutOfRangeException("x22");
            if (x23 is null) throw new ArgumentNullException("x23");
            if (x23.Count > 16) throw new ArgumentOutOfRangeException("x23");
            foreach (var n in x23)
            {
                if (n is null) throw new ArgumentNullException("n");
                if (n.Length > 128) throw new ArgumentOutOfRangeException("n");
            }
            if (x24 is null) throw new ArgumentNullException("x24");
            if (x24.Count > 32) throw new ArgumentOutOfRangeException("x24");
            foreach (var n in x24)
            {
                if (n.Value is null) throw new ArgumentNullException("n.Value");
                if (n.Value.Length > 128) throw new ArgumentOutOfRangeException("n.Value");
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
            this.X23 = new ReadOnlyCollection<string>(x23);
            this.X24 = new ReadOnlyDictionary<byte, string>(x24);

            {
                var hashCode = new HashCode();
                if (this.X0 != default) hashCode.Add(this.X0.GetHashCode());
                if (this.X1 != default) hashCode.Add(this.X1.GetHashCode());
                if (this.X2 != default) hashCode.Add(this.X2.GetHashCode());
                if (this.X3 != default) hashCode.Add(this.X3.GetHashCode());
                if (this.X4 != default) hashCode.Add(this.X4.GetHashCode());
                if (this.X5 != default) hashCode.Add(this.X5.GetHashCode());
                if (this.X6 != default) hashCode.Add(this.X6.GetHashCode());
                if (this.X7 != default) hashCode.Add(this.X7.GetHashCode());
                if (this.X8 != default) hashCode.Add(this.X8.GetHashCode());
                if (this.X9 != default) hashCode.Add(this.X9.GetHashCode());
                if (this.X10 != default) hashCode.Add(this.X10.GetHashCode());
                if (this.X11 != default) hashCode.Add(this.X11.GetHashCode());
                if (this.X12 != default) hashCode.Add(this.X12.GetHashCode());
                if (this.X13 != default) hashCode.Add(this.X13.GetHashCode());
                if (this.X14 != default) hashCode.Add(this.X14.GetHashCode());
                if (this.X15 != default) hashCode.Add(this.X15.GetHashCode());
                if (this.X16 != default) hashCode.Add(this.X16.GetHashCode());
                if (this.X17 != default) hashCode.Add(this.X17.GetHashCode());
                if (this.X18 != default) hashCode.Add(this.X18.GetHashCode());
                if (this.X19 != default) hashCode.Add(this.X19.GetHashCode());
                if (this.X20 != default) hashCode.Add(this.X20.GetHashCode());
                if (!this.X21.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.X21.Span));
                if (!this.X22.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.X22.Span));
                foreach (var n in this.X23)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                foreach (var n in this.X24)
                {
                    if (n.Key != default) hashCode.Add(n.Key.GetHashCode());
                    if (n.Value != default) hashCode.Add(n.Value.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
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
        public Timestamp X20 { get; }
        public ReadOnlyMemory<byte> X21 { get; }
        private readonly IMemoryOwner<byte> _x22;
        public ReadOnlyMemory<byte> X22 => _x22.Memory;
        public IReadOnlyList<string> X23 { get; }
        public IReadOnlyDictionary<byte, string> X24 { get; }

        public override bool Equals(HelloMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
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
            if (!BytesOperations.SequenceEqual(this.X21.Span, target.X21.Span)) return false;
            if (!BytesOperations.SequenceEqual(this.X22.Span, target.X22.Span)) return false;
            if ((this.X23 is null) != (target.X23 is null)) return false;
            if (!(this.X23 is null) && !(target.X23 is null) && !CollectionHelper.Equals(this.X23, target.X23)) return false;
            if ((this.X24 is null) != (target.X24 is null)) return false;
            if (!(this.X24 is null) && !(target.X24 is null) && !CollectionHelper.Equals(this.X24, target.X24)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        public void Dispose()
        {
            _x22?.Dispose();
        }

        private sealed class CustomFormatter : IRocketPackFormatter<HelloMessage>
        {
            public void Serialize(RocketPackWriter w, HelloMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Write property count
                {
                    uint propertyCount = 0;
                    if (value.X0 != default) propertyCount++;
                    if (value.X1 != default) propertyCount++;
                    if (value.X2 != default) propertyCount++;
                    if (value.X3 != default) propertyCount++;
                    if (value.X4 != default) propertyCount++;
                    if (value.X5 != default) propertyCount++;
                    if (value.X6 != default) propertyCount++;
                    if (value.X7 != default) propertyCount++;
                    if (value.X8 != default) propertyCount++;
                    if (value.X9 != default) propertyCount++;
                    if (value.X10 != default) propertyCount++;
                    if (value.X11 != default) propertyCount++;
                    if (value.X12 != default) propertyCount++;
                    if (value.X13 != default) propertyCount++;
                    if (value.X14 != default) propertyCount++;
                    if (value.X15 != default) propertyCount++;
                    if (value.X16 != default) propertyCount++;
                    if (value.X17 != default) propertyCount++;
                    if (value.X18 != default) propertyCount++;
                    if (value.X19 != default) propertyCount++;
                    if (value.X20 != default) propertyCount++;
                    if (!value.X21.IsEmpty) propertyCount++;
                    if (!value.X22.IsEmpty) propertyCount++;
                    if (value.X23.Count != 0) propertyCount++;
                    if (value.X24.Count != 0) propertyCount++;
                    w.Write(propertyCount);
                }

                // X0
                if (value.X0 != default)
                {
                    w.Write((uint)0);
                    w.Write(value.X0);
                }
                // X1
                if (value.X1 != default)
                {
                    w.Write((uint)1);
                    w.Write(value.X1);
                }
                // X2
                if (value.X2 != default)
                {
                    w.Write((uint)2);
                    w.Write(value.X2);
                }
                // X3
                if (value.X3 != default)
                {
                    w.Write((uint)3);
                    w.Write(value.X3);
                }
                // X4
                if (value.X4 != default)
                {
                    w.Write((uint)4);
                    w.Write(value.X4);
                }
                // X5
                if (value.X5 != default)
                {
                    w.Write((uint)5);
                    w.Write(value.X5);
                }
                // X6
                if (value.X6 != default)
                {
                    w.Write((uint)6);
                    w.Write(value.X6);
                }
                // X7
                if (value.X7 != default)
                {
                    w.Write((uint)7);
                    w.Write(value.X7);
                }
                // X8
                if (value.X8 != default)
                {
                    w.Write((uint)8);
                    w.Write(value.X8);
                }
                // X9
                if (value.X9 != default)
                {
                    w.Write((uint)9);
                    w.Write((long)value.X9);
                }
                // X10
                if (value.X10 != default)
                {
                    w.Write((uint)10);
                    w.Write((long)value.X10);
                }
                // X11
                if (value.X11 != default)
                {
                    w.Write((uint)11);
                    w.Write((long)value.X11);
                }
                // X12
                if (value.X12 != default)
                {
                    w.Write((uint)12);
                    w.Write((long)value.X12);
                }
                // X13
                if (value.X13 != default)
                {
                    w.Write((uint)13);
                    w.Write((ulong)value.X13);
                }
                // X14
                if (value.X14 != default)
                {
                    w.Write((uint)14);
                    w.Write((ulong)value.X14);
                }
                // X15
                if (value.X15 != default)
                {
                    w.Write((uint)15);
                    w.Write((ulong)value.X15);
                }
                // X16
                if (value.X16 != default)
                {
                    w.Write((uint)16);
                    w.Write((ulong)value.X16);
                }
                // X17
                if (value.X17 != default)
                {
                    w.Write((uint)17);
                    w.Write(value.X17);
                }
                // X18
                if (value.X18 != default)
                {
                    w.Write((uint)18);
                    w.Write(value.X18);
                }
                // X19
                if (value.X19 != default)
                {
                    w.Write((uint)19);
                    w.Write(value.X19);
                }
                // X20
                if (value.X20 != default)
                {
                    w.Write((uint)20);
                    w.Write(value.X20);
                }
                // X21
                if (!value.X21.IsEmpty)
                {
                    w.Write((uint)21);
                    w.Write(value.X21.Span);
                }
                // X22
                if (!value.X22.IsEmpty)
                {
                    w.Write((uint)22);
                    w.Write(value.X22.Span);
                }
                // X23
                if (value.X23.Count != 0)
                {
                    w.Write((uint)23);
                    w.Write((uint)value.X23.Count);
                    foreach (var n in value.X23)
                    {
                        w.Write(n);
                    }
                }
                // X24
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
            }

            public HelloMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                bool p_x0 = default;
                sbyte p_x1 = default;
                short p_x2 = default;
                int p_x3 = default;
                long p_x4 = default;
                byte p_x5 = default;
                ushort p_x6 = default;
                uint p_x7 = default;
                ulong p_x8 = default;
                Enum1 p_x9 = default;
                Enum2 p_x10 = default;
                Enum3 p_x11 = default;
                Enum4 p_x12 = default;
                Enum5 p_x13 = default;
                Enum6 p_x14 = default;
                Enum7 p_x15 = default;
                Enum8 p_x16 = default;
                float p_x17 = default;
                double p_x18 = default;
                string p_x19 = default;
                Timestamp p_x20 = default;
                ReadOnlyMemory<byte> p_x21 = default;
                IMemoryOwner<byte> p_x22 = default;
                IList<string> p_x23 = default;
                IDictionary<byte, string> p_x24 = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // X0
                            {
                                p_x0 = r.GetBoolean();
                                break;
                            }
                        case 1: // X1
                            {
                                p_x1 = r.GetInt8();
                                break;
                            }
                        case 2: // X2
                            {
                                p_x2 = r.GetInt16();
                                break;
                            }
                        case 3: // X3
                            {
                                p_x3 = r.GetInt32();
                                break;
                            }
                        case 4: // X4
                            {
                                p_x4 = r.GetInt64();
                                break;
                            }
                        case 5: // X5
                            {
                                p_x5 = r.GetUInt8();
                                break;
                            }
                        case 6: // X6
                            {
                                p_x6 = r.GetUInt16();
                                break;
                            }
                        case 7: // X7
                            {
                                p_x7 = r.GetUInt32();
                                break;
                            }
                        case 8: // X8
                            {
                                p_x8 = r.GetUInt64();
                                break;
                            }
                        case 9: // X9
                            {
                                p_x9 = (Enum1)r.GetInt64();
                                break;
                            }
                        case 10: // X10
                            {
                                p_x10 = (Enum2)r.GetInt64();
                                break;
                            }
                        case 11: // X11
                            {
                                p_x11 = (Enum3)r.GetInt64();
                                break;
                            }
                        case 12: // X12
                            {
                                p_x12 = (Enum4)r.GetInt64();
                                break;
                            }
                        case 13: // X13
                            {
                                p_x13 = (Enum5)r.GetUInt64();
                                break;
                            }
                        case 14: // X14
                            {
                                p_x14 = (Enum6)r.GetUInt64();
                                break;
                            }
                        case 15: // X15
                            {
                                p_x15 = (Enum7)r.GetUInt64();
                                break;
                            }
                        case 16: // X16
                            {
                                p_x16 = (Enum8)r.GetUInt64();
                                break;
                            }
                        case 17: // X17
                            {
                                p_x17 = r.GetFloat32();
                                break;
                            }
                        case 18: // X18
                            {
                                p_x18 = r.GetFloat64();
                                break;
                            }
                        case 19: // X19
                            {
                                p_x19 = r.GetString(128);
                                break;
                            }
                        case 20: // X20
                            {
                                p_x20 = r.GetTimestamp();
                                break;
                            }
                        case 21: // X21
                            {
                                p_x21 = r.GetMemory(256);
                                break;
                            }
                        case 22: // X22
                            {
                                p_x22 = r.GetRecyclableMemory(256);
                                break;
                            }
                        case 23: // X23
                            {
                                var length = r.GetUInt32();
                                p_x23 = new string[length];
                                for (int i = 0; i < p_x23.Count; i++)
                                {
                                    p_x23[i] = r.GetString(128);
                                }
                                break;
                            }
                        case 24: // X24
                            {
                                var length = r.GetUInt32();
                                p_x24 = new Dictionary<byte, string>();
                                byte t_key = default;
                                string t_value = default;
                                for (int i = 0; i < length; i++)
                                {
                                    t_key = r.GetUInt8();
                                    t_value = r.GetString(128);
                                    p_x24[t_key] = t_value;
                                }
                                break;
                            }
                    }
                }

                return new HelloMessage(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24);
            }
        }
    }

    internal readonly struct SmallHelloMessage
    {
        public static IRocketPackFormatter<SmallHelloMessage> Formatter { get; }

        static SmallHelloMessage()
        {
            SmallHelloMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxX19Length = 128;
        public static readonly int MaxX21Length = 256;
        public static readonly int MaxX22Length = 256;
        public static readonly int MaxX23Count = 16;
        public static readonly int MaxX24Count = 32;

        public SmallHelloMessage(bool x0, sbyte x1, short x2, int x3, long x4, byte x5, ushort x6, uint x7, ulong x8, Enum1 x9, Enum2 x10, Enum3 x11, Enum4 x12, Enum5 x13, Enum6 x14, Enum7 x15, Enum8 x16, float x17, double x18, string x19, Timestamp x20, ReadOnlyMemory<byte> x21, ReadOnlyMemory<byte> x22, IList<string> x23, IDictionary<byte, string> x24)
        {
            if (x19 is null) throw new ArgumentNullException("x19");
            if (x19.Length > 128) throw new ArgumentOutOfRangeException("x19");
            if (x21.Length > 256) throw new ArgumentOutOfRangeException("x21");
            if (x22.Length > 256) throw new ArgumentOutOfRangeException("x22");
            if (x23 is null) throw new ArgumentNullException("x23");
            if (x23.Count > 16) throw new ArgumentOutOfRangeException("x23");
            foreach (var n in x23)
            {
                if (n is null) throw new ArgumentNullException("n");
                if (n.Length > 128) throw new ArgumentOutOfRangeException("n");
            }
            if (x24 is null) throw new ArgumentNullException("x24");
            if (x24.Count > 32) throw new ArgumentOutOfRangeException("x24");
            foreach (var n in x24)
            {
                if (n.Value is null) throw new ArgumentNullException("n.Value");
                if (n.Value.Length > 128) throw new ArgumentOutOfRangeException("n.Value");
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
            this.X22 = x22;
            this.X23 = new ReadOnlyCollection<string>(x23);
            this.X24 = new ReadOnlyDictionary<byte, string>(x24);

            {
                var hashCode = new HashCode();
                if (this.X0 != default) hashCode.Add(this.X0.GetHashCode());
                if (this.X1 != default) hashCode.Add(this.X1.GetHashCode());
                if (this.X2 != default) hashCode.Add(this.X2.GetHashCode());
                if (this.X3 != default) hashCode.Add(this.X3.GetHashCode());
                if (this.X4 != default) hashCode.Add(this.X4.GetHashCode());
                if (this.X5 != default) hashCode.Add(this.X5.GetHashCode());
                if (this.X6 != default) hashCode.Add(this.X6.GetHashCode());
                if (this.X7 != default) hashCode.Add(this.X7.GetHashCode());
                if (this.X8 != default) hashCode.Add(this.X8.GetHashCode());
                if (this.X9 != default) hashCode.Add(this.X9.GetHashCode());
                if (this.X10 != default) hashCode.Add(this.X10.GetHashCode());
                if (this.X11 != default) hashCode.Add(this.X11.GetHashCode());
                if (this.X12 != default) hashCode.Add(this.X12.GetHashCode());
                if (this.X13 != default) hashCode.Add(this.X13.GetHashCode());
                if (this.X14 != default) hashCode.Add(this.X14.GetHashCode());
                if (this.X15 != default) hashCode.Add(this.X15.GetHashCode());
                if (this.X16 != default) hashCode.Add(this.X16.GetHashCode());
                if (this.X17 != default) hashCode.Add(this.X17.GetHashCode());
                if (this.X18 != default) hashCode.Add(this.X18.GetHashCode());
                if (this.X19 != default) hashCode.Add(this.X19.GetHashCode());
                if (this.X20 != default) hashCode.Add(this.X20.GetHashCode());
                if (!this.X21.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.X21.Span));
                if (!this.X22.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.X22.Span));
                foreach (var n in this.X23)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                foreach (var n in this.X24)
                {
                    if (n.Key != default) hashCode.Add(n.Key.GetHashCode());
                    if (n.Value != default) hashCode.Add(n.Value.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public static SmallHelloMessage Import(ReadOnlySequence<byte> sequence, BufferPool bufferPool)
        {
            return Formatter.Deserialize(new RocketPackReader(sequence, bufferPool), 0);
        }

        public void Export(IBufferWriter<byte> bufferWriter, BufferPool bufferPool)
        {
            Formatter.Serialize(new RocketPackWriter(bufferWriter, bufferPool), (SmallHelloMessage)this, 0);
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
        public Timestamp X20 { get; }
        public ReadOnlyMemory<byte> X21 { get; }
        public ReadOnlyMemory<byte> X22 { get; }
        public IReadOnlyList<string> X23 { get; }
        public IReadOnlyDictionary<byte, string> X24 { get; }

        public static bool operator ==(SmallHelloMessage x, SmallHelloMessage y) => x.Equals(y);
        public static bool operator !=(SmallHelloMessage x, SmallHelloMessage y) => !x.Equals(y);

        public override bool Equals(object other)
        {
            if (!(other is SmallHelloMessage)) return false;
            return this.Equals((SmallHelloMessage)other);
        }

        public bool Equals(SmallHelloMessage target)
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
            if (!BytesOperations.SequenceEqual(this.X21.Span, target.X21.Span)) return false;
            if (!BytesOperations.SequenceEqual(this.X22.Span, target.X22.Span)) return false;
            if ((this.X23 is null) != (target.X23 is null)) return false;
            if (!(this.X23 is null) && !(target.X23 is null) && !CollectionHelper.Equals(this.X23, target.X23)) return false;
            if ((this.X24 is null) != (target.X24 is null)) return false;
            if (!(this.X24 is null) && !(target.X24 is null) && !CollectionHelper.Equals(this.X24, target.X24)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<SmallHelloMessage>
        {
            public void Serialize(RocketPackWriter w, SmallHelloMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // X0
                w.Write(value.X0);
                // X1
                w.Write(value.X1);
                // X2
                w.Write(value.X2);
                // X3
                w.Write(value.X3);
                // X4
                w.Write(value.X4);
                // X5
                w.Write(value.X5);
                // X6
                w.Write(value.X6);
                // X7
                w.Write(value.X7);
                // X8
                w.Write(value.X8);
                // X9
                w.Write((long)value.X9);
                // X10
                w.Write((long)value.X10);
                // X11
                w.Write((long)value.X11);
                // X12
                w.Write((long)value.X12);
                // X13
                w.Write((ulong)value.X13);
                // X14
                w.Write((ulong)value.X14);
                // X15
                w.Write((ulong)value.X15);
                // X16
                w.Write((ulong)value.X16);
                // X17
                w.Write(value.X17);
                // X18
                w.Write(value.X18);
                // X19
                w.Write(value.X19);
                // X20
                w.Write(value.X20);
                // X21
                w.Write(value.X21.Span);
                // X22
                w.Write(value.X22.Span);
                // X23
                w.Write((uint)value.X23.Count);
                foreach (var n in value.X23)
                {
                    w.Write(n);
                }
                // X24
                w.Write((uint)value.X24.Count);
                foreach (var n in value.X24)
                {
                    w.Write(n.Key);
                    w.Write(n.Value);
                }
            }

            public SmallHelloMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                bool p_x0 = default;
                sbyte p_x1 = default;
                short p_x2 = default;
                int p_x3 = default;
                long p_x4 = default;
                byte p_x5 = default;
                ushort p_x6 = default;
                uint p_x7 = default;
                ulong p_x8 = default;
                Enum1 p_x9 = default;
                Enum2 p_x10 = default;
                Enum3 p_x11 = default;
                Enum4 p_x12 = default;
                Enum5 p_x13 = default;
                Enum6 p_x14 = default;
                Enum7 p_x15 = default;
                Enum8 p_x16 = default;
                float p_x17 = default;
                double p_x18 = default;
                string p_x19 = default;
                Timestamp p_x20 = default;
                ReadOnlyMemory<byte> p_x21 = default;
                ReadOnlyMemory<byte> p_x22 = default;
                IList<string> p_x23 = default;
                IDictionary<byte, string> p_x24 = default;

                // X0
                {
                    p_x0 = r.GetBoolean();
                }
                // X1
                {
                    p_x1 = r.GetInt8();
                }
                // X2
                {
                    p_x2 = r.GetInt16();
                }
                // X3
                {
                    p_x3 = r.GetInt32();
                }
                // X4
                {
                    p_x4 = r.GetInt64();
                }
                // X5
                {
                    p_x5 = r.GetUInt8();
                }
                // X6
                {
                    p_x6 = r.GetUInt16();
                }
                // X7
                {
                    p_x7 = r.GetUInt32();
                }
                // X8
                {
                    p_x8 = r.GetUInt64();
                }
                // X9
                {
                    p_x9 = (Enum1)r.GetInt64();
                }
                // X10
                {
                    p_x10 = (Enum2)r.GetInt64();
                }
                // X11
                {
                    p_x11 = (Enum3)r.GetInt64();
                }
                // X12
                {
                    p_x12 = (Enum4)r.GetInt64();
                }
                // X13
                {
                    p_x13 = (Enum5)r.GetUInt64();
                }
                // X14
                {
                    p_x14 = (Enum6)r.GetUInt64();
                }
                // X15
                {
                    p_x15 = (Enum7)r.GetUInt64();
                }
                // X16
                {
                    p_x16 = (Enum8)r.GetUInt64();
                }
                // X17
                {
                    p_x17 = r.GetFloat32();
                }
                // X18
                {
                    p_x18 = r.GetFloat64();
                }
                // X19
                {
                    p_x19 = r.GetString(128);
                }
                // X20
                {
                    p_x20 = r.GetTimestamp();
                }
                // X21
                {
                    p_x21 = r.GetMemory(256);
                }
                // X22
                {
                    p_x22 = r.GetMemory(256);
                }
                // X23
                {
                    var length = r.GetUInt32();
                    p_x23 = new string[length];
                    for (int i = 0; i < p_x23.Count; i++)
                    {
                        p_x23[i] = r.GetString(128);
                    }
                }
                // X24
                {
                    var length = r.GetUInt32();
                    p_x24 = new Dictionary<byte, string>();
                    byte t_key = default;
                    string t_value = default;
                    for (int i = 0; i < length; i++)
                    {
                        t_key = r.GetUInt8();
                        t_value = r.GetString(128);
                        p_x24[t_key] = t_value;
                    }
                }

                return new SmallHelloMessage(p_x0, p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x18, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24);
            }
        }
    }

    internal sealed partial class IntPropertiesListMessage : RocketPackMessageBase<IntPropertiesListMessage>
    {
        static IntPropertiesListMessage()
        {
            IntPropertiesListMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxListCount = 100000;

        public IntPropertiesListMessage(IList<IntPropertiesMessage> list)
        {
            if (list is null) throw new ArgumentNullException("list");
            if (list.Count > 100000) throw new ArgumentOutOfRangeException("list");
            foreach (var n in list)
            {
                if (n is null) throw new ArgumentNullException("n");
            }

            this.List = new ReadOnlyCollection<IntPropertiesMessage>(list);

            {
                var hashCode = new HashCode();
                foreach (var n in this.List)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyList<IntPropertiesMessage> List { get; }

        public override bool Equals(IntPropertiesListMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.List is null) != (target.List is null)) return false;
            if (!(this.List is null) && !(target.List is null) && !CollectionHelper.Equals(this.List, target.List)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<IntPropertiesListMessage>
        {
            public void Serialize(RocketPackWriter w, IntPropertiesListMessage value, int rank)
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
                        IntPropertiesMessage.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public IntPropertiesListMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IList<IntPropertiesMessage> p_list = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // List
                            {
                                var length = r.GetUInt32();
                                p_list = new IntPropertiesMessage[length];
                                for (int i = 0; i < p_list.Count; i++)
                                {
                                    p_list[i] = IntPropertiesMessage.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new IntPropertiesListMessage(p_list);
            }
        }
    }

    internal sealed partial class StringPropertiesListMessage : RocketPackMessageBase<StringPropertiesListMessage>
    {
        static StringPropertiesListMessage()
        {
            StringPropertiesListMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxListCount = 100000;

        public StringPropertiesListMessage(IList<StringPropertiesMessage> list)
        {
            if (list is null) throw new ArgumentNullException("list");
            if (list.Count > 100000) throw new ArgumentOutOfRangeException("list");
            foreach (var n in list)
            {
                if (n is null) throw new ArgumentNullException("n");
            }

            this.List = new ReadOnlyCollection<StringPropertiesMessage>(list);

            {
                var hashCode = new HashCode();
                foreach (var n in this.List)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                _hashCode = hashCode.ToHashCode();
            }
        }

        public IReadOnlyList<StringPropertiesMessage> List { get; }

        public override bool Equals(StringPropertiesListMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
            if ((this.List is null) != (target.List is null)) return false;
            if (!(this.List is null) && !(target.List is null) && !CollectionHelper.Equals(this.List, target.List)) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<StringPropertiesListMessage>
        {
            public void Serialize(RocketPackWriter w, StringPropertiesListMessage value, int rank)
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
                        StringPropertiesMessage.Formatter.Serialize(w, n, rank + 1);
                    }
                }
            }

            public StringPropertiesListMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                // Read property count
                uint propertyCount = r.GetUInt32();

                IList<StringPropertiesMessage> p_list = default;

                for (; propertyCount > 0; propertyCount--)
                {
                    uint id = r.GetUInt32();
                    switch (id)
                    {
                        case 0: // List
                            {
                                var length = r.GetUInt32();
                                p_list = new StringPropertiesMessage[length];
                                for (int i = 0; i < p_list.Count; i++)
                                {
                                    p_list[i] = StringPropertiesMessage.Formatter.Deserialize(r, rank + 1);
                                }
                                break;
                            }
                    }
                }

                return new StringPropertiesListMessage(p_list);
            }
        }
    }

    internal sealed partial class IntPropertiesMessage : RocketPackMessageBase<IntPropertiesMessage>
    {
        static IntPropertiesMessage()
        {
            IntPropertiesMessage.Formatter = new CustomFormatter();
        }

        public IntPropertiesMessage(uint myProperty1, uint myProperty2, uint myProperty3, uint myProperty4, uint myProperty5, uint myProperty6, uint myProperty7, uint myProperty8, uint myProperty9)
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

        public override bool Equals(IntPropertiesMessage target)
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

        private sealed class CustomFormatter : IRocketPackFormatter<IntPropertiesMessage>
        {
            public void Serialize(RocketPackWriter w, IntPropertiesMessage value, int rank)
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

            public IntPropertiesMessage Deserialize(RocketPackReader r, int rank)
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

                return new IntPropertiesMessage(p_myProperty1, p_myProperty2, p_myProperty3, p_myProperty4, p_myProperty5, p_myProperty6, p_myProperty7, p_myProperty8, p_myProperty9);
            }
        }
    }

    internal sealed partial class StringPropertiesMessage : RocketPackMessageBase<StringPropertiesMessage>
    {
        static StringPropertiesMessage()
        {
            StringPropertiesMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxMyProperty1Length = 8192;
        public static readonly int MaxMyProperty2Length = 8192;
        public static readonly int MaxMyProperty3Length = 8192;

        public StringPropertiesMessage(string myProperty1, string myProperty2, string myProperty3)
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

        public override bool Equals(StringPropertiesMessage target)
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

        private sealed class CustomFormatter : IRocketPackFormatter<StringPropertiesMessage>
        {
            public void Serialize(RocketPackWriter w, StringPropertiesMessage value, int rank)
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

            public StringPropertiesMessage Deserialize(RocketPackReader r, int rank)
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

                return new StringPropertiesMessage(p_myProperty1, p_myProperty2, p_myProperty3);
            }
        }
    }

}
