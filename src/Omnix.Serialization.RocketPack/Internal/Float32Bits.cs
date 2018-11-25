using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Omnix.Serialization.RocketPack.Internal
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct Float32Bits
    {
        [FieldOffset(0)]
        public readonly float Value;

        [FieldOffset(0)]
        public readonly byte Byte0;

        [FieldOffset(1)]
        public readonly byte Byte1;

        [FieldOffset(2)]
        public readonly byte Byte2;

        [FieldOffset(3)]
        public readonly byte Byte3;

        public Float32Bits(float value)
        {
            this = default(Float32Bits);
            this.Value = value;
        }

        public Float32Bits(Span<byte> bigEndianBytes)
        {
            this = default(Float32Bits);

            if (BitConverter.IsLittleEndian)
            {
                this.Byte0 = bigEndianBytes[3];
                this.Byte1 = bigEndianBytes[2];
                this.Byte2 = bigEndianBytes[1];
                this.Byte3 = bigEndianBytes[0];
            }
            else
            {
                this.Byte0 = bigEndianBytes[0];
                this.Byte1 = bigEndianBytes[1];
                this.Byte2 = bigEndianBytes[2];
                this.Byte3 = bigEndianBytes[3];
            }
        }
    }
}
