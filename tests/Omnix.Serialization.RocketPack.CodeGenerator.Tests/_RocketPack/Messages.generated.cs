using Omnix.Base;
using Omnix.Base.Helpers;
using Omnix.Serialization;
using Omnix.Serialization.RocketPack;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Omnix.Serialization.RocketPack.CodeGenerator.Tests
{
    public enum Enum1 : sbyte
    {
        Yes = 0,
        No = 1,
    }

    public enum Enum2 : short
    {
        Yes = 0,
        No = 1,
    }

    public enum Enum3 : int
    {
        Yes = 0,
        No = 1,
    }

    public enum Enum4 : long
    {
        Yes = 0,
        No = 1,
    }

    public enum Enum5 : byte
    {
        Yes = 0,
        No = 1,
    }

    public enum Enum6 : ushort
    {
        Yes = 0,
        No = 1,
    }

    public enum Enum7 : uint
    {
        Yes = 0,
        No = 1,
    }

    public enum Enum8 : ulong
    {
        Yes = 0,
        No = 1,
    }

    public sealed partial class HelloMessage : RocketPackMessageBase<HelloMessage>, IDisposable
    {
        static HelloMessage()
        {
            HelloMessage.Formatter = new CustomFormatter();
        }

        public static readonly int MaxX12Length = 128;
        public static readonly int MaxX14Length = 256;
        public static readonly int MaxX15Length = 256;
        public static readonly int MaxX16Count = 16;
        public static readonly int MaxX17Count = 32;

        public HelloMessage(bool x1, sbyte x2, short x3, int x4, long x5, byte x6, ushort x7, uint x8, ulong x9, float x10, double x11, string x12, Timestamp x13, ReadOnlyMemory<byte> x14, IMemoryOwner<byte> x15, IList<string> x16, IDictionary<sbyte, ReadOnlyMemory<byte>> x17, Enum1 x19, Enum2 x20, Enum3 x21, Enum4 x22, Enum5 x23, Enum6 x24, Enum7 x25, Enum8 x26)
        {
            if (x12 is null) throw new ArgumentNullException("x12");
            if (x12.Length > 128) throw new ArgumentOutOfRangeException("x12");
            if (x14.Length > 256) throw new ArgumentOutOfRangeException("x14");
            if (x15 is null) throw new ArgumentNullException("x15");
            if (x15.Memory.Length > 256) throw new ArgumentOutOfRangeException("x15");
            if (x16 is null) throw new ArgumentNullException("x16");
            if (x16.Count > 16) throw new ArgumentOutOfRangeException("x16");
            foreach (var n in x16)
            {
                if (n is null) throw new ArgumentNullException("n");
                if (n.Length > 128) throw new ArgumentOutOfRangeException("n");
            }
            if (x17 is null) throw new ArgumentNullException("x17");
            if (x17.Count > 32) throw new ArgumentOutOfRangeException("x17");
            foreach (var n in x17)
            {
                if (n.Value.Length > 32) throw new ArgumentOutOfRangeException("n.Value");
            }
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
            _x15 = x15;
            this.X16 = new ReadOnlyCollection<string>(x16);
            this.X17 = new ReadOnlyDictionary<sbyte, ReadOnlyMemory<byte>>(x17);
            this.X19 = x19;
            this.X20 = x20;
            this.X21 = x21;
            this.X22 = x22;
            this.X23 = x23;
            this.X24 = x24;
            this.X25 = x25;
            this.X26 = x26;

            {
                var hashCode = new HashCode();
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
                if (!this.X14.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.X14.Span));
                if (!this.X15.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.X15.Span));
                foreach (var n in this.X16)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                foreach (var n in this.X17)
                {
                    if (n.Key != default) hashCode.Add(n.Key.GetHashCode());
                    if (!n.Value.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(n.Value.Span));
                }
                if (this.X19 != default) hashCode.Add(this.X19.GetHashCode());
                if (this.X20 != default) hashCode.Add(this.X20.GetHashCode());
                if (this.X21 != default) hashCode.Add(this.X21.GetHashCode());
                if (this.X22 != default) hashCode.Add(this.X22.GetHashCode());
                if (this.X23 != default) hashCode.Add(this.X23.GetHashCode());
                if (this.X24 != default) hashCode.Add(this.X24.GetHashCode());
                if (this.X25 != default) hashCode.Add(this.X25.GetHashCode());
                if (this.X26 != default) hashCode.Add(this.X26.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public bool X1 { get; }
        public sbyte X2 { get; }
        public short X3 { get; }
        public int X4 { get; }
        public long X5 { get; }
        public byte X6 { get; }
        public ushort X7 { get; }
        public uint X8 { get; }
        public ulong X9 { get; }
        public float X10 { get; }
        public double X11 { get; }
        public string X12 { get; }
        public Timestamp X13 { get; }
        public ReadOnlyMemory<byte> X14 { get; }
        private readonly IMemoryOwner<byte> _x15;
        public ReadOnlyMemory<byte> X15 => _x15.Memory;
        public IReadOnlyList<string> X16 { get; }
        public IReadOnlyDictionary<sbyte, ReadOnlyMemory<byte>> X17 { get; }
        public Enum1 X19 { get; }
        public Enum2 X20 { get; }
        public Enum3 X21 { get; }
        public Enum4 X22 { get; }
        public Enum5 X23 { get; }
        public Enum6 X24 { get; }
        public Enum7 X25 { get; }
        public Enum8 X26 { get; }

        public override bool Equals(HelloMessage target)
        {
            if ((object)target == null) return false;
            if (Object.ReferenceEquals(this, target)) return true;
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
            if (this.X12 != target.X12) return false;
            if (this.X13 != target.X13) return false;
            if (!BytesOperations.SequenceEqual(this.X14.Span, target.X14.Span)) return false;
            if (!BytesOperations.SequenceEqual(this.X15.Span, target.X15.Span)) return false;
            if ((this.X16 is null) != (target.X16 is null)) return false;
            if (!(this.X16 is null) && !(target.X16 is null) && !CollectionHelper.Equals(this.X16, target.X16)) return false;
            if ((this.X17 is null) != (target.X17 is null)) return false;
            if (!(this.X17 is null) && !(target.X17 is null) && !CollectionHelper.Equals(this.X17, target.X17)) return false;
            if (this.X19 != target.X19) return false;
            if (this.X20 != target.X20) return false;
            if (this.X21 != target.X21) return false;
            if (this.X22 != target.X22) return false;
            if (this.X23 != target.X23) return false;
            if (this.X24 != target.X24) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        public void Dispose()
        {
            _x15?.Dispose();
        }

        private sealed class CustomFormatter : IRocketPackFormatter<HelloMessage>
        {
            public void Serialize(RocketPackWriter w, HelloMessage value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // X1
                if (value.X1 != default)
                {
                    w.Write((ulong)0);
                    w.Write(value.X1);
                }
                // X2
                if (value.X2 != default)
                {
                    w.Write((ulong)1);
                    w.Write((long)value.X2);
                }
                // X3
                if (value.X3 != default)
                {
                    w.Write((ulong)2);
                    w.Write((long)value.X3);
                }
                // X4
                if (value.X4 != default)
                {
                    w.Write((ulong)3);
                    w.Write((long)value.X4);
                }
                // X5
                if (value.X5 != default)
                {
                    w.Write((ulong)4);
                    w.Write((long)value.X5);
                }
                // X6
                if (value.X6 != default)
                {
                    w.Write((ulong)5);
                    w.Write((ulong)value.X6);
                }
                // X7
                if (value.X7 != default)
                {
                    w.Write((ulong)6);
                    w.Write((ulong)value.X7);
                }
                // X8
                if (value.X8 != default)
                {
                    w.Write((ulong)7);
                    w.Write((ulong)value.X8);
                }
                // X9
                if (value.X9 != default)
                {
                    w.Write((ulong)8);
                    w.Write((ulong)value.X9);
                }
                // X10
                if (value.X10 != default)
                {
                    w.Write((ulong)9);
                    w.Write(value.X10);
                }
                // X11
                if (value.X11 != default)
                {
                    w.Write((ulong)10);
                    w.Write(value.X11);
                }
                // X12
                if (value.X12 != default)
                {
                    w.Write((ulong)11);
                    w.Write(value.X12);
                }
                // X13
                if (value.X13 != default)
                {
                    w.Write((ulong)12);
                    w.Write(value.X13);
                }
                // X14
                if (!value.X14.IsEmpty)
                {
                    w.Write((ulong)13);
                    w.Write(value.X14.Span);
                }
                // X15
                if (!value.X15.IsEmpty)
                {
                    w.Write((ulong)14);
                    w.Write(value.X15.Span);
                }
                // X16
                if (value.X16.Count != 0)
                {
                    w.Write((ulong)15);
                    foreach (var n in value.X16)
                    {
                        w.Write(n);
                    }
                }
                // X17
                if (value.X17.Count != 0)
                {
                    w.Write((ulong)16);
                    foreach (var n in value.X17)
                    {
                        w.Write((long)n.Key);
                        w.Write(n.Value.Span);
                    }
                }
                // X19
                if (value.X19 != default)
                {
                    w.Write((ulong)17);
                    w.Write((long)value.X19);
                }
                // X20
                if (value.X20 != default)
                {
                    w.Write((ulong)18);
                    w.Write((long)value.X20);
                }
                // X21
                if (value.X21 != default)
                {
                    w.Write((ulong)19);
                    w.Write((long)value.X21);
                }
                // X22
                if (value.X22 != default)
                {
                    w.Write((ulong)20);
                    w.Write((long)value.X22);
                }
                // X23
                if (value.X23 != default)
                {
                    w.Write((ulong)21);
                    w.Write((ulong)value.X23);
                }
                // X24
                if (value.X24 != default)
                {
                    w.Write((ulong)22);
                    w.Write((ulong)value.X24);
                }
                // X25
                if (value.X25 != default)
                {
                    w.Write((ulong)23);
                    w.Write((ulong)value.X25);
                }
                // X26
                if (value.X26 != default)
                {
                    w.Write((ulong)24);
                    w.Write((ulong)value.X26);
                }
            }

            public HelloMessage Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                bool p_x1 = default;
                sbyte p_x2 = default;
                short p_x3 = default;
                int p_x4 = default;
                long p_x5 = default;
                byte p_x6 = default;
                ushort p_x7 = default;
                uint p_x8 = default;
                ulong p_x9 = default;
                float p_x10 = default;
                double p_x11 = default;
                string p_x12 = default;
                Timestamp p_x13 = default;
                ReadOnlyMemory<byte> p_x14 = default;
                IMemoryOwner<byte> p_x15 = default;
                IList<string> p_x16 = default;
                IDictionary<sbyte, ReadOnlyMemory<byte>> p_x17 = default;
                Enum1 p_x19 = default;
                Enum2 p_x20 = default;
                Enum3 p_x21 = default;
                Enum4 p_x22 = default;
                Enum5 p_x23 = default;
                Enum6 p_x24 = default;
                Enum7 p_x25 = default;
                Enum8 p_x26 = default;

                while (r.Available > 0)
                {
                    int id = (int)r.GetUInt64();
                    switch (id)
                    {
                        case 0: // X1
                            {
                                p_x1 = r.GetBoolean();
                                break;
                            }
                        case 1: // X2
                            {
                                p_x2 = (sbyte)r.GetInt64();
                                break;
                            }
                        case 2: // X3
                            {
                                p_x3 = (short)r.GetInt64();
                                break;
                            }
                        case 3: // X4
                            {
                                p_x4 = (int)r.GetInt64();
                                break;
                            }
                        case 4: // X5
                            {
                                p_x5 = (long)r.GetInt64();
                                break;
                            }
                        case 5: // X6
                            {
                                p_x6 = (byte)r.GetUInt64();
                                break;
                            }
                        case 6: // X7
                            {
                                p_x7 = (ushort)r.GetUInt64();
                                break;
                            }
                        case 7: // X8
                            {
                                p_x8 = (uint)r.GetUInt64();
                                break;
                            }
                        case 8: // X9
                            {
                                p_x9 = (ulong)r.GetUInt64();
                                break;
                            }
                        case 9: // X10
                            {
                                p_x10 = r.GetFloat32();
                                break;
                            }
                        case 10: // X11
                            {
                                p_x11 = r.GetFloat64();
                                break;
                            }
                        case 11: // X12
                            {
                                p_x12 = r.GetString(128);
                                break;
                            }
                        case 12: // X13
                            {
                                p_x13 = r.GetTimestamp();
                                break;
                            }
                        case 13: // X14
                            {
                                p_x14 = r.GetMemory(256);
                                break;
                            }
                        case 14: // X15
                            {
                                p_x15 = r.GetRecyclableMemory(256);
                                break;
                            }
                        case 15: // X16
                            {
                                var length = (int)r.GetUInt64();
                                var t_array = new string[length];
                                for (int i = 0; i < t_array.Length; i++)
                                {
                                    t_array[i] = r.GetString(128);
                                }
                                p_x16 = new List<string>(t_array);
                                break;
                            }
                        case 16: // X17
                            {
                                var length = (int)r.GetUInt64();
                                p_x17 = new Dictionary<sbyte, ReadOnlyMemory<byte>>();
                                sbyte t_key = default;
                                ReadOnlyMemory<byte> t_value = default;
                                for (int i = 0; i < length; i++)
                                {
                                    t_key = (sbyte)r.GetInt64();
                                    t_value = r.GetMemory(32);
                                    p_x17[t_key] = t_value;
                                }
                                break;
                            }
                        case 17: // X19
                            {
                                p_x19 = (Enum1)r.GetInt64();
                                break;
                            }
                        case 18: // X20
                            {
                                p_x20 = (Enum2)r.GetInt64();
                                break;
                            }
                        case 19: // X21
                            {
                                p_x21 = (Enum3)r.GetInt64();
                                break;
                            }
                        case 20: // X22
                            {
                                p_x22 = (Enum4)r.GetInt64();
                                break;
                            }
                        case 21: // X23
                            {
                                p_x23 = (Enum5)r.GetUInt64();
                                break;
                            }
                        case 22: // X24
                            {
                                p_x24 = (Enum6)r.GetUInt64();
                                break;
                            }
                        case 23: // X25
                            {
                                p_x25 = (Enum7)r.GetUInt64();
                                break;
                            }
                        case 24: // X26
                            {
                                p_x26 = (Enum8)r.GetUInt64();
                                break;
                            }
                    }
                }

                return new HelloMessage(p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

    public readonly struct HelloMessage2
    {
        public static IRocketPackFormatter<HelloMessage2> Formatter { get; }

        static HelloMessage2()
        {
            HelloMessage2.Formatter = new CustomFormatter();
        }

        public static readonly int MaxX12Length = 128;
        public static readonly int MaxX14Length = 256;
        public static readonly int MaxX15Length = 256;
        public static readonly int MaxX16Count = 16;
        public static readonly int MaxX17Count = 32;

        public HelloMessage2(bool x1, sbyte x2, short x3, int x4, long x5, byte x6, ushort x7, uint x8, ulong x9, float x10, double x11, string x12, Timestamp x13, ReadOnlyMemory<byte> x14, ReadOnlyMemory<byte> x15, IList<string> x16, IDictionary<sbyte, ReadOnlyMemory<byte>> x17, Enum1 x19, Enum2 x20, Enum3 x21, Enum4 x22, Enum5 x23, Enum6 x24, Enum7 x25, Enum8 x26)
        {
            if (x12 is null) throw new ArgumentNullException("x12");
            if (x12.Length > 128) throw new ArgumentOutOfRangeException("x12");
            if (x14.Length > 256) throw new ArgumentOutOfRangeException("x14");
            if (x15.Length > 256) throw new ArgumentOutOfRangeException("x15");
            if (x16 is null) throw new ArgumentNullException("x16");
            if (x16.Count > 16) throw new ArgumentOutOfRangeException("x16");
            foreach (var n in x16)
            {
                if (n is null) throw new ArgumentNullException("n");
                if (n.Length > 128) throw new ArgumentOutOfRangeException("n");
            }
            if (x17 is null) throw new ArgumentNullException("x17");
            if (x17.Count > 32) throw new ArgumentOutOfRangeException("x17");
            foreach (var n in x17)
            {
                if (n.Value.Length > 32) throw new ArgumentOutOfRangeException("n.Value");
            }
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
            this.X16 = new ReadOnlyCollection<string>(x16);
            this.X17 = new ReadOnlyDictionary<sbyte, ReadOnlyMemory<byte>>(x17);
            this.X19 = x19;
            this.X20 = x20;
            this.X21 = x21;
            this.X22 = x22;
            this.X23 = x23;
            this.X24 = x24;
            this.X25 = x25;
            this.X26 = x26;

            {
                var hashCode = new HashCode();
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
                if (!this.X14.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.X14.Span));
                if (!this.X15.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(this.X15.Span));
                foreach (var n in this.X16)
                {
                    if (n != default) hashCode.Add(n.GetHashCode());
                }
                foreach (var n in this.X17)
                {
                    if (n.Key != default) hashCode.Add(n.Key.GetHashCode());
                    if (!n.Value.IsEmpty) hashCode.Add(ObjectHelper.GetHashCode(n.Value.Span));
                }
                if (this.X19 != default) hashCode.Add(this.X19.GetHashCode());
                if (this.X20 != default) hashCode.Add(this.X20.GetHashCode());
                if (this.X21 != default) hashCode.Add(this.X21.GetHashCode());
                if (this.X22 != default) hashCode.Add(this.X22.GetHashCode());
                if (this.X23 != default) hashCode.Add(this.X23.GetHashCode());
                if (this.X24 != default) hashCode.Add(this.X24.GetHashCode());
                if (this.X25 != default) hashCode.Add(this.X25.GetHashCode());
                if (this.X26 != default) hashCode.Add(this.X26.GetHashCode());
                _hashCode = hashCode.ToHashCode();
            }
        }

        public static HelloMessage2 Import(ReadOnlySequence<byte> sequence, BufferPool bufferPool)
        {
            return Formatter.Deserialize(new RocketPackReader(sequence, bufferPool), 0);
        }

        public void Export(IBufferWriter<byte> bufferWriter, BufferPool bufferPool)
        {
            Formatter.Serialize(new RocketPackWriter(bufferWriter, bufferPool), (HelloMessage2)this, 0);
        }

        public bool X1 { get; }
        public sbyte X2 { get; }
        public short X3 { get; }
        public int X4 { get; }
        public long X5 { get; }
        public byte X6 { get; }
        public ushort X7 { get; }
        public uint X8 { get; }
        public ulong X9 { get; }
        public float X10 { get; }
        public double X11 { get; }
        public string X12 { get; }
        public Timestamp X13 { get; }
        public ReadOnlyMemory<byte> X14 { get; }
        public ReadOnlyMemory<byte> X15 { get; }
        public IReadOnlyList<string> X16 { get; }
        public IReadOnlyDictionary<sbyte, ReadOnlyMemory<byte>> X17 { get; }
        public Enum1 X19 { get; }
        public Enum2 X20 { get; }
        public Enum3 X21 { get; }
        public Enum4 X22 { get; }
        public Enum5 X23 { get; }
        public Enum6 X24 { get; }
        public Enum7 X25 { get; }
        public Enum8 X26 { get; }

        public static bool operator ==(HelloMessage2 x, HelloMessage2 y) => x.Equals(y);
        public static bool operator !=(HelloMessage2 x, HelloMessage2 y) => !x.Equals(y);

        public override bool Equals(object other)
        {
            if (!(other is HelloMessage2)) return false;
            return this.Equals((HelloMessage2)other);
        }

        public bool Equals(HelloMessage2 target)
        {
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
            if (this.X12 != target.X12) return false;
            if (this.X13 != target.X13) return false;
            if (!BytesOperations.SequenceEqual(this.X14.Span, target.X14.Span)) return false;
            if (!BytesOperations.SequenceEqual(this.X15.Span, target.X15.Span)) return false;
            if ((this.X16 is null) != (target.X16 is null)) return false;
            if (!(this.X16 is null) && !(target.X16 is null) && !CollectionHelper.Equals(this.X16, target.X16)) return false;
            if ((this.X17 is null) != (target.X17 is null)) return false;
            if (!(this.X17 is null) && !(target.X17 is null) && !CollectionHelper.Equals(this.X17, target.X17)) return false;
            if (this.X19 != target.X19) return false;
            if (this.X20 != target.X20) return false;
            if (this.X21 != target.X21) return false;
            if (this.X22 != target.X22) return false;
            if (this.X23 != target.X23) return false;
            if (this.X24 != target.X24) return false;
            if (this.X25 != target.X25) return false;
            if (this.X26 != target.X26) return false;

            return true;
        }

        private readonly int _hashCode;
        public override int GetHashCode() => _hashCode;

        private sealed class CustomFormatter : IRocketPackFormatter<HelloMessage2>
        {
            public void Serialize(RocketPackWriter w, HelloMessage2 value, int rank)
            {
                if (rank > 256) throw new FormatException();

                // X1
                if (value.X1 != default)
                {
                    w.Write(value.X1);
                }
                // X2
                if (value.X2 != default)
                {
                    w.Write((long)value.X2);
                }
                // X3
                if (value.X3 != default)
                {
                    w.Write((long)value.X3);
                }
                // X4
                if (value.X4 != default)
                {
                    w.Write((long)value.X4);
                }
                // X5
                if (value.X5 != default)
                {
                    w.Write((long)value.X5);
                }
                // X6
                if (value.X6 != default)
                {
                    w.Write((ulong)value.X6);
                }
                // X7
                if (value.X7 != default)
                {
                    w.Write((ulong)value.X7);
                }
                // X8
                if (value.X8 != default)
                {
                    w.Write((ulong)value.X8);
                }
                // X9
                if (value.X9 != default)
                {
                    w.Write((ulong)value.X9);
                }
                // X10
                if (value.X10 != default)
                {
                    w.Write(value.X10);
                }
                // X11
                if (value.X11 != default)
                {
                    w.Write(value.X11);
                }
                // X12
                if (value.X12 != default)
                {
                    w.Write(value.X12);
                }
                // X13
                if (value.X13 != default)
                {
                    w.Write(value.X13);
                }
                // X14
                if (!value.X14.IsEmpty)
                {
                    w.Write(value.X14.Span);
                }
                // X15
                if (!value.X15.IsEmpty)
                {
                    w.Write(value.X15.Span);
                }
                // X16
                if (value.X16.Count != 0)
                {
                    foreach (var n in value.X16)
                    {
                        w.Write(n);
                    }
                }
                // X17
                if (value.X17.Count != 0)
                {
                    foreach (var n in value.X17)
                    {
                        w.Write((long)n.Key);
                        w.Write(n.Value.Span);
                    }
                }
                // X19
                if (value.X19 != default)
                {
                    w.Write((long)value.X19);
                }
                // X20
                if (value.X20 != default)
                {
                    w.Write((long)value.X20);
                }
                // X21
                if (value.X21 != default)
                {
                    w.Write((long)value.X21);
                }
                // X22
                if (value.X22 != default)
                {
                    w.Write((long)value.X22);
                }
                // X23
                if (value.X23 != default)
                {
                    w.Write((ulong)value.X23);
                }
                // X24
                if (value.X24 != default)
                {
                    w.Write((ulong)value.X24);
                }
                // X25
                if (value.X25 != default)
                {
                    w.Write((ulong)value.X25);
                }
                // X26
                if (value.X26 != default)
                {
                    w.Write((ulong)value.X26);
                }
            }

            public HelloMessage2 Deserialize(RocketPackReader r, int rank)
            {
                if (rank > 256) throw new FormatException();

                bool p_x1 = default;
                sbyte p_x2 = default;
                short p_x3 = default;
                int p_x4 = default;
                long p_x5 = default;
                byte p_x6 = default;
                ushort p_x7 = default;
                uint p_x8 = default;
                ulong p_x9 = default;
                float p_x10 = default;
                double p_x11 = default;
                string p_x12 = default;
                Timestamp p_x13 = default;
                ReadOnlyMemory<byte> p_x14 = default;
                ReadOnlyMemory<byte> p_x15 = default;
                IList<string> p_x16 = default;
                IDictionary<sbyte, ReadOnlyMemory<byte>> p_x17 = default;
                Enum1 p_x19 = default;
                Enum2 p_x20 = default;
                Enum3 p_x21 = default;
                Enum4 p_x22 = default;
                Enum5 p_x23 = default;
                Enum6 p_x24 = default;
                Enum7 p_x25 = default;
                Enum8 p_x26 = default;

                // X1
                {
                    p_x1 = r.GetBoolean();
                }
                // X2
                {
                    p_x2 = (sbyte)r.GetInt64();
                }
                // X3
                {
                    p_x3 = (short)r.GetInt64();
                }
                // X4
                {
                    p_x4 = (int)r.GetInt64();
                }
                // X5
                {
                    p_x5 = (long)r.GetInt64();
                }
                // X6
                {
                    p_x6 = (byte)r.GetUInt64();
                }
                // X7
                {
                    p_x7 = (ushort)r.GetUInt64();
                }
                // X8
                {
                    p_x8 = (uint)r.GetUInt64();
                }
                // X9
                {
                    p_x9 = (ulong)r.GetUInt64();
                }
                // X10
                {
                    p_x10 = r.GetFloat32();
                }
                // X11
                {
                    p_x11 = r.GetFloat64();
                }
                // X12
                {
                    p_x12 = r.GetString(128);
                }
                // X13
                {
                    p_x13 = r.GetTimestamp();
                }
                // X14
                {
                    p_x14 = r.GetMemory(256);
                }
                // X15
                {
                    p_x15 = r.GetMemory(256);
                }
                // X16
                {
                    var length = (int)r.GetUInt64();
                    var t_array = new string[length];
                    for (int i = 0; i < t_array.Length; i++)
                    {
                        t_array[i] = r.GetString(128);
                    }
                    p_x16 = new List<string>(t_array);
                }
                // X17
                {
                    var length = (int)r.GetUInt64();
                    p_x17 = new Dictionary<sbyte, ReadOnlyMemory<byte>>();
                    sbyte t_key = default;
                    ReadOnlyMemory<byte> t_value = default;
                    for (int i = 0; i < length; i++)
                    {
                        t_key = (sbyte)r.GetInt64();
                        t_value = r.GetMemory(32);
                        p_x17[t_key] = t_value;
                    }
                }
                // X19
                {
                    p_x19 = (Enum1)r.GetInt64();
                }
                // X20
                {
                    p_x20 = (Enum2)r.GetInt64();
                }
                // X21
                {
                    p_x21 = (Enum3)r.GetInt64();
                }
                // X22
                {
                    p_x22 = (Enum4)r.GetInt64();
                }
                // X23
                {
                    p_x23 = (Enum5)r.GetUInt64();
                }
                // X24
                {
                    p_x24 = (Enum6)r.GetUInt64();
                }
                // X25
                {
                    p_x25 = (Enum7)r.GetUInt64();
                }
                // X26
                {
                    p_x26 = (Enum8)r.GetUInt64();
                }

                return new HelloMessage2(p_x1, p_x2, p_x3, p_x4, p_x5, p_x6, p_x7, p_x8, p_x9, p_x10, p_x11, p_x12, p_x13, p_x14, p_x15, p_x16, p_x17, p_x19, p_x20, p_x21, p_x22, p_x23, p_x24, p_x25, p_x26);
            }
        }
    }

}
