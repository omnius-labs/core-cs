using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Running;
using FormatterBenchmarks.Cases;
using FormatterBenchmarks.Internal;

namespace FormatterBenchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[]
            {
                typeof(IntSerializeBenchmark),
                typeof(IntDeserializeBenchmark),
                typeof(StringSerializeBenchmark),
                typeof(StringDeserializeBenchmark),
                typeof(BytesSerializeBenchmark),
                typeof(BytesDeserializeBenchmark),
            });

            switcher.Run(args);
        }
    }
}
