using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;



namespace FastEnumUtility.Benchmark.Configurations
{
    internal sealed class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            this.Add(MarkdownExporter.GitHub);
            this.Add(DefaultConfig.Instance);
            this.Add(MemoryDiagnoser.Default);
            //this.Add(StatisticColumn.AllStatistics);
            this.Add(Job.ShortRun);
        }
    }
}
