using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Omnix.Base.Internal
{
    internal static unsafe class PureUnsafeMethods
    {
        public static void Zero(byte* source, int length)
        {
            byte* t_s = source;

            for (int i = (length / 8) - 1; i >= 0; i--, t_s += 8)
            {
                *((long*)t_s) = 0;
            }

            if ((length & 4) != 0)
            {
                *((int*)t_s) = 0;
                t_s += 4;
            }

            if ((length & 2) != 0)
            {
                *((short*)t_s) = 0;
                t_s += 2;
            }

            if ((length & 1) != 0)
            {
                *((byte*)t_s) = 0;
            }
        }

        public static void Copy(byte* source, byte* destination, int length)
        {
            Buffer.MemoryCopy(source, destination, 1024 * 1024, length);
        }

        public static bool Equals(byte* source1, byte* source2, int length)
        {
            byte* t_x = source1, t_y = source2;

            for (int i = (length / 8) - 1; i >= 0; i--, t_x += 8, t_y += 8)
            {
                if (*((long*)t_x) != *((long*)t_y))
                {
                    return false;
                }
            }

            if ((length & 4) != 0)
            {
                if (*((int*)t_x) != *((int*)t_y))
                {
                    return false;
                }

                t_x += 4;
                t_y += 4;
            }

            if ((length & 2) != 0)
            {
                if (*((short*)t_x) != *((short*)t_y))
                {
                    return false;
                }

                t_x += 2;
                t_y += 2;
            }

            if ((length & 1) != 0)
            {
                if (*((byte*)t_x) != *((byte*)t_y))
                {
                    return false;
                }
            }

            return true;
        }

        public static int Compare(byte* source1, byte* source2, int length)
        {
            byte* t_x = source1, t_y = source2;

            int len = length;
            int c = 0;

            for (; len > 0; len--)
            {
                c = (int)*t_x++ - (int)*t_y++;
                if (c != 0)
                {
                    return c;
                }
            }

            return 0;
        }

        public static void And(byte* source1, byte* source2, byte* result, int length)
        {
            byte* t_x = source1, t_y = source2;
            var t_buffer = result;

            for (int i = (length / 8) - 1; i >= 0; i--, t_x += 8, t_y += 8, t_buffer += 8)
            {
                *((long*)t_buffer) = *((long*)t_x) & *((long*)t_y);
            }

            if ((length & 4) != 0)
            {
                *((int*)t_buffer) = *((int*)t_x) & *((int*)t_y);
                t_x += 4;
                t_y += 4;
                t_buffer += 4;
            }

            if ((length & 2) != 0)
            {
                *((short*)t_buffer) = (short)(*((short*)t_x) & *((short*)t_y));
                t_x += 2;
                t_y += 2;
                t_buffer += 2;
            }

            if ((length & 1) != 0)
            {
                *((byte*)t_buffer) = (byte)(*((byte*)t_x) & *((byte*)t_y));
            }
        }

        public static void Or(byte* source1, byte* source2, byte* result, int length)
        {
            byte* t_x = source1, t_y = source2;
            var t_buffer = result;

            for (int i = (length / 8) - 1; i >= 0; i--, t_x += 8, t_y += 8, t_buffer += 8)
            {
                *((long*)t_buffer) = *((long*)t_x) | *((long*)t_y);
            }

            if ((length & 4) != 0)
            {
                *((int*)t_buffer) = *((int*)t_x) | *((int*)t_y);
                t_x += 4;
                t_y += 4;
                t_buffer += 4;
            }

            if ((length & 2) != 0)
            {
                *((short*)t_buffer) = (short)(*((short*)t_x) | *((short*)t_y));
                t_x += 2;
                t_y += 2;
                t_buffer += 2;
            }

            if ((length & 1) != 0)
            {
                *((byte*)t_buffer) = (byte)(*((byte*)t_x) | *((byte*)t_y));
            }
        }

        public static void Xor(byte* source1, byte* source2, byte* result, int length)
        {
            byte* t_x = source1, t_y = source2;
            var t_buffer = result;

            for (int i = (length / 8) - 1; i >= 0; i--, t_x += 8, t_y += 8, t_buffer += 8)
            {
                *((long*)t_buffer) = *((long*)t_x) ^ *((long*)t_y);
            }

            if ((length & 4) != 0)
            {
                *((int*)t_buffer) = *((int*)t_x) ^ *((int*)t_y);
                t_x += 4;
                t_y += 4;
                t_buffer += 4;
            }

            if ((length & 2) != 0)
            {
                *((short*)t_buffer) = (short)(*((short*)t_x) ^ *((short*)t_y));
                t_x += 2;
                t_y += 2;
                t_buffer += 2;
            }

            if ((length & 1) != 0)
            {
                *((byte*)t_buffer) = (byte)(*((byte*)t_x) ^ *((byte*)t_y));
            }
        }
    }
}
