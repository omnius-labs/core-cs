using System.Linq;
using System.Dynamic;
using System.Collections.Generic;
using System.Text;
using System;
using System.Text.Json;

namespace Omnius.Core.Test
{
    public static class AssertEx
    {
        public static void EqualJson<T>(T expected, T actual)
        {
            var option = new JsonSerializerOptions() { WriteIndented = true };
            var expectedJsonString = JsonSerializer.Serialize(expected, option);
            var actualJsonString = JsonSerializer.Serialize(actual, option);
            if (expectedJsonString == actualJsonString) return;

            var diff = InlineDiffBuilder.Diff(expectedJsonString, actualJsonString);

            var savedColor = Console.ForegroundColor;

            try
            {
                foreach (var line in diff.Lines)
                {
                    switch (line.Type)
                    {
                        case ChangeType.Inserted:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("+ ");
                            break;
                        case ChangeType.Deleted:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("- ");
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Gray; // compromise for dark or light background
                            Console.Write("  ");
                            break;
                    }

                    Console.WriteLine(line.Text);
                }
            }
            finally
            {
                Console.ForegroundColor = savedColor;
            }

            throw new AssertException();
        }
    }
}
