using BenchmarkDotNet.Running;
using FormatterBenchmarks.Cases;

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
            });

            switcher.Run(args);
        }
    }
}
