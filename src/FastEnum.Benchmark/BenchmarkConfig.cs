using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace FastEnumUtility.Benchmark;



internal sealed class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        this.AddExporter(MarkdownExporter.GitHub);
        this.Add(DefaultConfig.Instance);
        this.AddDiagnoser(MemoryDiagnoser.Default);
        //this.AddJob(Job.ShortRun);
        this.AddJob(Job.ShortRun.WithWarmupCount(1).WithIterationCount(1));
        //this.AddColumn(StatisticColumn.AllStatistics);
    }
}
