using System;
using System.Runtime.InteropServices;

namespace Omnius.Core.RocketPack.Internal
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Float64Bits
    {
        [FieldOffset(0)]
        public readonly double Value;

        [FieldOffset(0)]
        public readonly byte Byte0;

        [FieldOffset(1)]
        public readonly byte Byte1;

        [FieldOffset(2)]
        public readonly byte Byte2;

        [FieldOffset(3)]
        public readonly byte Byte3;

        [FieldOffset(4)]
        public readonly byte Byte4;

        [FieldOffset(5)]
        public readonly byte Byte5;

        [FieldOffset(6)]
        public readonly byte Byte6;

        [FieldOffset(7)]
        public readonly byte Byte7;

        public Float64Bits(in double value)
        {
            this = default(Float64Bits);
            Value = value;
        }

        public Float64Bits(in Span<byte> bigEndianBytes)
        {
            this = default(Float64Bits);

            if (BitConverter.IsLittleEndian)
            {
                Byte0 = bigEndianBytes[7];
                Byte1 = bigEndianBytes[6];
                Byte2 = bigEndianBytes[5];
                Byte3 = bigEndianBytes[4];
                Byte4 = bigEndianBytes[3];
                Byte5 = bigEndianBytes[2];
                Byte6 = bigEndianBytes[1];
                Byte7 = bigEndianBytes[0];
            }
            else
            {
                Byte0 = bigEndianBytes[0];
                Byte1 = bigEndianBytes[1];
                Byte2 = bigEndianBytes[2];
                Byte3 = bigEndianBytes[3];
                Byte4 = bigEndianBytes[4];
                Byte5 = bigEndianBytes[5];
                Byte6 = bigEndianBytes[6];
                Byte7 = bigEndianBytes[7];
            }
        }

        public void CopyTo(ref Span<byte> span)
        {
            if (BitConverter.IsLittleEndian)
            {
                span[7] = Byte0;
                span[6] = Byte1;
                span[5] = Byte2;
                span[4] = Byte3;
                span[3] = Byte4;
                span[2] = Byte5;
                span[1] = Byte6;
                span[0] = Byte7;
            }
            else
            {
                span[0] = Byte0;
                span[1] = Byte1;
                span[2] = Byte2;
                span[3] = Byte3;
                span[4] = Byte4;
                span[5] = Byte5;
                span[6] = Byte6;
                span[7] = Byte7;
            }
        }
    }
}
