using System.Security.Cryptography;

namespace Core.Net.I2p.Internal
{
    internal static class I2pConverter
    {
        public static class Base32
        {
            private static readonly char[] _table = "abcdefghijklmnopqrstuvwxyz234567".ToCharArray();

            private const int InBitsPerByte = 8;
            private const int OutBitsPerByte = 5;
            private const int OutBitMask = 0x1f;

            public static int CalculateLength(int length)
            {
                return ((length * InBitsPerByte) + (OutBitsPerByte - 1)) / OutBitsPerByte;
            }

            public static int ToCharArray(byte[] inArray, int offsetIn, int lengthIn, char[] outArray, int offsetOut)
            {
                if (inArray == null) throw new ArgumentNullException(nameof(inArray));
                if (offsetIn < 0 || inArray.Length < offsetIn) throw new ArgumentOutOfRangeException(nameof(offsetIn));
                if (lengthIn < 0 || inArray.Length < lengthIn) throw new ArgumentOutOfRangeException(nameof(lengthIn));
                if (inArray.Length - offsetIn < lengthIn) throw new ArgumentOutOfRangeException();
                if (outArray == null) throw new ArgumentNullException(nameof(outArray));
                if (offsetOut < 0 || outArray.Length < offsetOut) throw new ArgumentOutOfRangeException(nameof(offsetOut));

                int lengthOut = Base32.CalculateLength(lengthIn);

                if (lengthOut < 0 || outArray.Length < lengthOut) throw new ArgumentOutOfRangeException(nameof(offsetOut));
                if (outArray.Length - offsetOut < lengthOut) throw new ArgumentOutOfRangeException();

                int positionIn = offsetIn;
                int positionOut = offsetOut;

                int queue = 0;
                int bitsInQueue = 0;

                for (int i = 0; i != lengthIn; ++i)
                {
                    queue <<= InBitsPerByte;
                    queue |= inArray[positionIn];
                    ++positionIn;
                    bitsInQueue += InBitsPerByte;

                    for (; bitsInQueue >= OutBitsPerByte; bitsInQueue -= OutBitsPerByte)
                    {
                        int outIndex = (queue >> (bitsInQueue - OutBitsPerByte)) & OutBitMask;
                        outArray[positionOut] = _table[outIndex];
                        ++positionOut;
                    }
                }

                if (bitsInQueue != 0)
                {
                    int outIndex = (queue << (OutBitsPerByte - bitsInQueue)) & OutBitMask;
                    outArray[positionOut] = _table[outIndex];
                    ++positionOut;
                    bitsInQueue = 0;
                }

                return lengthOut;
            }

            public static string ToString(byte[] inArray)
            {
                return ToString(inArray, 0, inArray.Length);
            }

            public static string ToString(byte[] inArray, int offset, int length)
            {
                var outArray = new char[CalculateLength(length)];
                ToCharArray(inArray, offset, length, outArray, 0);
                return new string(outArray);
            }
        }

        public static class Base64
        {
            public static string ToString(byte[] v)
            {
                return Convert.ToBase64String(v).Replace('+', '-').Replace('/', '~');
            }

            public static byte[] FromString(string s)
            {
                return Convert.FromBase64String(s.Replace('-', '+').Replace('~', '/'));
            }
        }

        public static class Base32Address
        {
            public static string FromDestination(byte[] destination)
            {
                using (var sha256 = SHA256.Create())
                {
                    var hashResult = sha256.ComputeHash(destination);
                    return Base32.ToString(hashResult) + ".b32.i2p";
                }
            }
        }
    }
}
