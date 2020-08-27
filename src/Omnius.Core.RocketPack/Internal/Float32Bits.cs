using System;
using System.Runtime.InteropServices;

namespace Omnius.Core.RocketPack.Internal
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

        public Float32Bits(in float value)
        {
            this = default(Float32Bits);
            Value = value;
        }

        public Float32Bits(in Span<byte> bigEndianBytes)
        {
            this = default(Float32Bits);

            if (BitConverter.IsLittleEndian)
            {
                Byte0 = bigEndianBytes[3];
                Byte1 = bigEndianBytes[2];
                Byte2 = bigEndianBytes[1];
                Byte3 = bigEndianBytes[0];
            }
            else
            {
                Byte0 = bigEndianBytes[0];
                Byte1 = bigEndianBytes[1];
                Byte2 = bigEndianBytes[2];
                Byte3 = bigEndianBytes[3];
            }
        }

        public void CopyTo(ref Span<byte> span)
        {
            if (BitConverter.IsLittleEndian)
            {
                span[3] = Byte0;
                span[2] = Byte1;
                span[1] = Byte2;
                span[0] = Byte3;
            }
            else
            {
                span[0] = Byte0;
                span[1] = Byte1;
                span[2] = Byte2;
                span[3] = Byte3;
            }
        }
    }
}
