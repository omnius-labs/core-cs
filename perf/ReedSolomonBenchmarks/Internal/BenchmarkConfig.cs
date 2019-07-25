using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;

namespace ReedSolomonBenchmarks.Internal
{
    internal class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            this.Add(MarkdownExporter.GitHub);
            this.Add(MemoryDiagnoser.Default);
            this.Add(Job.ShortRun.With(BenchmarkDotNet.Environments.Platform.X64).With(CsProjCoreToolchain.NetCoreApp30));
            //this.Add(new EtwProfiler());
        }
    }
}
