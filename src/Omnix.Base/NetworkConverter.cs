using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Omnix.Base
{
    public unsafe static class NetworkConverter
    {
        public static string ToSizeString(decimal b)
        {
            string f = (b < 0) ? "-" : "";
            b = Math.Abs(b);

            var u = new List<string> { "Byte", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int i = 0;

            while (b >= 1024)
            {
                b /= (decimal)1024;
                i++;
            }

            string value = Math.Round(b, 2).ToString().Trim();

            if (value.Contains("."))
            {
                value = value.TrimEnd('0').TrimEnd('.');
            }

            return f + value + " " + u[i];
        }

        public static string ToSizeString(decimal b, string unit)
        {
            string f = (b < 0) ? "-" : "";
            b = Math.Abs(b);

            var u = new List<string> { "Byte", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            int i = 0;

            while (u[i] != unit)
            {
                if (b != 0) b /= (decimal)1024;
                i++;
            }

            string value = Math.Round(b, 2).ToString().Trim();

            if (value.Contains("."))
            {
                value = value.TrimEnd('0').TrimEnd('.');
            }

            return f + value + " " + u[i];
        }

        public static decimal FromSizeString(string value)
        {
            decimal f = value.StartsWith("-") ? -1 : 1;
            value = value.TrimStart('-');

            var builder = new StringBuilder("0");

            foreach (char item in value)
            {
                if (Regex.IsMatch(item.ToString(), @"([0-9])|(\.)"))
                {
                    builder.Append(item.ToString());
                }
            }

            try
            {
                if (value.ToLower().Contains("yb"))
                {
                    return f * decimal.Parse(builder.ToString()) * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024;
                }
                else if (value.ToLower().Contains("zb"))
                {
                    return f * decimal.Parse(builder.ToString()) * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024;
                }
                else if (value.ToLower().Contains("eb"))
                {
                    return f * decimal.Parse(builder.ToString()) * 1024 * 1024 * 1024 * 1024 * 1024 * 1024;
                }
                else if (value.ToLower().Contains("pb"))
                {
                    return f * decimal.Parse(builder.ToString()) * 1024 * 1024 * 1024 * 1024 * 1024;
                }
                else if (value.ToLower().Contains("tb"))
                {
                    return f * decimal.Parse(builder.ToString()) * 1024 * 1024 * 1024 * 1024;
                }
                else if (value.ToLower().Contains("gb"))
                {
                    return f * decimal.Parse(builder.ToString()) * 1024 * 1024 * 1024;
                }
                else if (value.ToLower().Contains("mb"))
                {
                    return f * decimal.Parse(builder.ToString()) * 1024 * 1024;
                }
                else if (value.ToLower().Contains("kb"))
                {
                    return f * decimal.Parse(builder.ToString()) * 1024;
                }
                else
                {
                    return f * decimal.Parse(builder.ToString());
                }
            }
            catch (Exception)
            {
                if (f == -1) return decimal.MinValue;
                else return decimal.MaxValue;
            }
        }

        public static bool ToBoolean(ReadOnlySpan<byte> value)
        {
            if (value.Length < 1) throw new ArgumentOutOfRangeException(nameof(value));

            return (value[0] != 0);
        }

        public static char ToChar(ReadOnlySpan<byte> value)
        {
            if (value.Length < 2) throw new ArgumentOutOfRangeException(nameof(value));

            if (BitConverter.IsLittleEndian)
            {
                char result;
                {
                    var result_p = (byte*)&result;

                    fixed (byte* value_p = value)
                    {
                        var t_p = value_p;

                        *result_p++ = t_p[1];
                        *result_p++ = t_p[0];
                    }
                }

                return result;
            }
            else
            {
                return BitConverter.ToChar(value);
            }
        }

        public static float ToSingle(ReadOnlySpan<byte> value)
        {
            if (value.Length < 4) throw new ArgumentOutOfRangeException(nameof(value));

            if (BitConverter.IsLittleEndian)
            {
                float result;
                {
                    var result_p = (byte*)&result;

                    fixed (byte* value_p = value)
                    {
                        var t_p = value_p;

                        *result_p++ = t_p[3];
                        *result_p++ = t_p[2];
                        *result_p++ = t_p[1];
                        *result_p++ = t_p[0];
                    }
                }

                return result;
            }
            else
            {
                return BitConverter.ToSingle(value);
            }
        }

        public static double ToDouble(byte[] value, int offset)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length < 8) throw new ArgumentOutOfRangeException(nameof(value));
            if ((value.Length - offset) < 8) throw new ArgumentOutOfRangeException(nameof(offset));

            if (BitConverter.IsLittleEndian)
            {
                double result;
                {
                    var result_p = (byte*)&result;

                    fixed (byte* value_p = value)
                    {
                        var t_p = value_p + offset;

                        *result_p++ = t_p[7];
                        *result_p++ = t_p[6];
                        *result_p++ = t_p[5];
                        *result_p++ = t_p[4];
                        *result_p++ = t_p[3];
                        *result_p++ = t_p[2];
                        *result_p++ = t_p[1];
                        *result_p++ = t_p[0];
                    }
                }

                return result;
            }
            else
            {
                return BitConverter.ToDouble(value, offset);
            }
        }

        public static short ToInt16(byte[] value)
        {
            return NetworkConverter.ToInt16(value, 0);
        }

        public static short ToInt16(byte[] value, int offset)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length < 2) throw new ArgumentOutOfRangeException(nameof(value));
            if ((value.Length - offset) < 2) throw new ArgumentOutOfRangeException(nameof(offset));

            if (BitConverter.IsLittleEndian)
            {
                short result;
                {
                    var result_p = (byte*)&result;

                    fixed (byte* value_p = value)
                    {
                        var t_p = value_p + offset;

                        *result_p++ = t_p[1];
                        *result_p++ = t_p[0];
                    }
                }

                return result;
            }
            else
            {
                return BitConverter.ToInt16(value, offset);
            }
        }

        public static int ToInt32(byte[] value)
        {
            return NetworkConverter.ToInt32(value, 0);
        }

        public static int ToInt32(byte[] value, int offset)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length < 4) throw new ArgumentOutOfRangeException(nameof(value));
            if ((value.Length - offset) < 4) throw new ArgumentOutOfRangeException(nameof(offset));

            if (BitConverter.IsLittleEndian)
            {
                int result;
                {
                    var result_p = (byte*)&result;

                    fixed (byte* value_p = value)
                    {
                        var t_p = value_p + offset;

                        *result_p++ = t_p[3];
                        *result_p++ = t_p[2];
                        *result_p++ = t_p[1];
                        *result_p++ = t_p[0];
                    }
                }

                return result;
            }
            else
            {
                return BitConverter.ToInt32(value, offset);
            }
        }

        public static long ToInt64(byte[] value)
        {
            return NetworkConverter.ToInt64(value, 0);
        }

        public static long ToInt64(byte[] value, int offset)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length < 8) throw new ArgumentOutOfRangeException(nameof(value));
            if ((value.Length - offset) < 8) throw new ArgumentOutOfRangeException(nameof(offset));

            if (BitConverter.IsLittleEndian)
            {
                long result;
                {
                    var result_p = (byte*)&result;

                    fixed (byte* value_p = value)
                    {
                        var t_p = value_p + offset;

                        *result_p++ = t_p[7];
                        *result_p++ = t_p[6];
                        *result_p++ = t_p[5];
                        *result_p++ = t_p[4];
                        *result_p++ = t_p[3];
                        *result_p++ = t_p[2];
                        *result_p++ = t_p[1];
                        *result_p++ = t_p[0];
                    }
                }

                return result;
            }
            else
            {
                return BitConverter.ToInt64(value, offset);
            }
        }

        public static ushort ToUInt16(byte[] value)
        {
            return NetworkConverter.ToUInt16(value, 0);
        }

        public static ushort ToUInt16(byte[] value, int offset)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length < 2) throw new ArgumentOutOfRangeException(nameof(value));
            if ((value.Length - offset) < 2) throw new ArgumentOutOfRangeException(nameof(offset));

            if (BitConverter.IsLittleEndian)
            {
                ushort result;
                {
                    var result_p = (byte*)&result;

                    fixed (byte* value_p = value)
                    {
                        var t_p = value_p + offset;

                        *result_p++ = t_p[1];
                        *result_p++ = t_p[0];
                    }
                }

                return result;
            }
            else
            {
                return BitConverter.ToUInt16(value, offset);
            }
        }

        public static uint ToUInt32(byte[] value)
        {
            return NetworkConverter.ToUInt32(value, 0);
        }

        public static uint ToUInt32(byte[] value, int offset)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length < 4) throw new ArgumentOutOfRangeException(nameof(value));
            if ((value.Length - offset) < 4) throw new ArgumentOutOfRangeException(nameof(offset));

            if (BitConverter.IsLittleEndian)
            {
                uint result;
                {
                    var result_p = (byte*)&result;

                    fixed (byte* value_p = value)
                    {
                        var t_p = value_p + offset;

                        *result_p++ = t_p[3];
                        *result_p++ = t_p[2];
                        *result_p++ = t_p[1];
                        *result_p++ = t_p[0];
                    }
                }

                return result;
            }
            else
            {
                return BitConverter.ToUInt32(value, offset);
            }
        }

        public static ulong ToUInt64(byte[] value)
        {
            return NetworkConverter.ToUInt64(value, 0);
        }

        public static ulong ToUInt64(byte[] value, int offset)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (value.Length < 8) throw new ArgumentOutOfRangeException(nameof(value));
            if ((value.Length - offset) < 8) throw new ArgumentOutOfRangeException(nameof(offset));

            if (BitConverter.IsLittleEndian)
            {
                ulong result;
                {
                    var result_p = (byte*)&result;

                    fixed (byte* value_p = value)
                    {
                        var t_p = value_p + offset;

                        *result_p++ = t_p[7];
                        *result_p++ = t_p[6];
                        *result_p++ = t_p[5];
                        *result_p++ = t_p[4];
                        *result_p++ = t_p[3];
                        *result_p++ = t_p[2];
                        *result_p++ = t_p[1];
                        *result_p++ = t_p[0];
                    }
                }

                return result;
            }
            else
            {
                return BitConverter.ToUInt64(value, offset);
            }
        }

        public static byte[] GetBytes(bool value)
        {
            return BitConverter.GetBytes(value);
        }

        public static byte[] GetBytes(char value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var result = new byte[2];
                {
                    var value_p = (byte*)&value;

                    result[1] = *value_p++;
                    result[0] = *value_p++;
                }

                return result;
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(float value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var result = new byte[4];
                {
                    var value_p = (byte*)&value;

                    result[3] = *value_p++;
                    result[2] = *value_p++;
                    result[1] = *value_p++;
                    result[0] = *value_p++;
                }

                return result;
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(double value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var result = new byte[8];
                {
                    var value_p = (byte*)&value;

                    result[7] = *value_p++;
                    result[6] = *value_p++;
                    result[5] = *value_p++;
                    result[4] = *value_p++;
                    result[3] = *value_p++;
                    result[2] = *value_p++;
                    result[1] = *value_p++;
                    result[0] = *value_p++;
                }

                return result;
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(short value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var result = new byte[2];
                {
                    var value_p = (byte*)&value;

                    result[1] = *value_p++;
                    result[0] = *value_p++;
                }

                return result;
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(int value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var result = new byte[4];
                {
                    var value_p = (byte*)&value;

                    result[3] = *value_p++;
                    result[2] = *value_p++;
                    result[1] = *value_p++;
                    result[0] = *value_p++;
                }

                return result;
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(long value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var result = new byte[8];
                {
                    var value_p = (byte*)&value;

                    result[7] = *value_p++;
                    result[6] = *value_p++;
                    result[5] = *value_p++;
                    result[4] = *value_p++;
                    result[3] = *value_p++;
                    result[2] = *value_p++;
                    result[1] = *value_p++;
                    result[0] = *value_p++;
                }

                return result;
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(ushort value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var result = new byte[2];
                {
                    var value_p = (byte*)&value;

                    result[1] = *value_p++;
                    result[0] = *value_p++;
                }

                return result;
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(uint value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var result = new byte[4];
                {
                    var value_p = (byte*)&value;

                    result[3] = *value_p++;
                    result[2] = *value_p++;
                    result[1] = *value_p++;
                    result[0] = *value_p++;
                }

                return result;
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }

        public static byte[] GetBytes(ulong value)
        {
            if (BitConverter.IsLittleEndian)
            {
                var result = new byte[8];
                {
                    var value_p = (byte*)&value;

                    result[7] = *value_p++;
                    result[6] = *value_p++;
                    result[5] = *value_p++;
                    result[4] = *value_p++;
                    result[3] = *value_p++;
                    result[2] = *value_p++;
                    result[1] = *value_p++;
                    result[0] = *value_p++;
                }

                return result;
            }
            else
            {
                return BitConverter.GetBytes(value);
            }
        }
    }
}
