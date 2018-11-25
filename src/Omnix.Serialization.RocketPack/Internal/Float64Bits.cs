using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Omnix.Serialization.RocketPack.Internal
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

        public Float64Bits(double value)
        {
            this = default(Float64Bits);
            this.Value = value;
        }

        public Float64Bits(Span<byte> bigEndianBytes)
        {
            this = default(Float64Bits);

            if (BitConverter.IsLittleEndian)
            {
                this.Byte0 = bigEndianBytes[7];
                this.Byte1 = bigEndianBytes[6];
                this.Byte2 = bigEndianBytes[5];
                this.Byte3 = bigEndianBytes[4];
                this.Byte4 = bigEndianBytes[3];
                this.Byte5 = bigEndianBytes[2];
                this.Byte6 = bigEndianBytes[1];
                this.Byte7 = bigEndianBytes[0];
            }
            else
            {
                this.Byte0 = bigEndianBytes[0];
                this.Byte1 = bigEndianBytes[1];
                this.Byte2 = bigEndianBytes[2];
                this.Byte3 = bigEndianBytes[3];
                this.Byte4 = bigEndianBytes[4];
                this.Byte5 = bigEndianBytes[5];
                this.Byte6 = bigEndianBytes[6];
                this.Byte7 = bigEndianBytes[7];
            }
        }
    }
}
