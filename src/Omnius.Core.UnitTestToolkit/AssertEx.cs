using System;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;

namespace Omnius.Core.UnitTestToolkit
{
    public class BytesReadOnlyMemoryConverter : JsonConverter<ReadOnlyMemory<byte>>
    {
        public static BytesReadOnlyMemoryConverter Default { get; } = new BytesReadOnlyMemoryConverter();

        public override ReadOnlyMemory<byte> Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) => new ReadOnlyMemory<byte>(reader.GetBytesFromBase64());

        public override void Write(
            Utf8JsonWriter writer,
            ReadOnlyMemory<byte> value,
            JsonSerializerOptions options) =>
                writer.WriteBase64StringValue(value.Span);
    }

    public static class AssertEx
    {
        public static void EqualJson<T>(T expected, T actual, string? description = null)
        {
            var options = new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
            };
            options.Converters.Add(BytesReadOnlyMemoryConverter.Default);

            var expectedJsonString = JsonSerializer.Serialize(expected, options);
            var actualJsonString = JsonSerializer.Serialize(actual, options);
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

            throw new AssertException($"EqualJson Error: {description}");
        }
    }
}
