using System;
using System.Collections.Generic;
using System.Text;

namespace Omnix.Cryptography.HashcashComputer
{
    public class BytesConvert
    {
        public static string ToHexString(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
            {
                if (b < 16) sb.Append('0');
                sb.Append(Convert.ToString(b, 16));
            }
            return sb.ToString();
        }

        public static byte[] FromHexString(string text)
        {
            int length = text.Length / 2;
            byte[] bytes = new byte[length];

            int j = 0;
            for (int i = 0; i < length; i++)
            {
                bytes[i] = Convert.ToByte(text.Substring(j, 2), 16);
                j += 2;
            }

            return bytes;
        }
    }
}
