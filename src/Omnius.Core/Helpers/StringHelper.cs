using System.Collections.Generic;
using System.Text;

namespace Omnius.Core.Helpers
{
    public class StringHelper
    {
        public static string Normalize(string value)
        {
            var sb = new StringBuilder(value.Length);

            foreach (string element in ToTextElements(value))
            {
                for (int i = 0; i < element.Length; i++)
                {
                    if (!char.IsControl(element[i]) && element[i] != '\uFFFD')
                    {
                        sb.Append(element[i]);
                    }
                }
            }

            return sb.ToString();
        }

        private static IEnumerable<string> ToTextElements(string text)
        {
            var iterator = System.Globalization.StringInfo.GetTextElementEnumerator(text);

            while (iterator.MoveNext())
            {
                string item = iterator.GetTextElement();

                if (Encoding.UTF8.GetByteCount(item) > 16)
                {
                    yield return " ";
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}
