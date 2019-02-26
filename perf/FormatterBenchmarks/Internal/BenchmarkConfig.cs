using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace FormatterBenchmarks.Internal
{
    internal class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            this.Add(MarkdownExporter.GitHub);
            this.Add(MemoryDiagnoser.Default);

            this.Add(Job.ShortRun.With(BenchmarkDotNet.Environments.Platform.X64));
        }
    }
}
