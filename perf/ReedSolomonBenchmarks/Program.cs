using BenchmarkDotNet.Running;
using ReedSolomonBenchmarks.Cases;

namespace ReedSolomonBenchmarks
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[]
            {
                typeof(EncodeBenchmark),
            });

            switcher.Run(args);
        }
    }
}
