using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace FastEnumUtility.Benchmarks.Internals;



internal static class BenchmarkConfig
{
    public static IConfig ForDefault()
    {
        return ManualConfig.CreateEmpty()
            .AddExporter(MarkdownExporter.GitHub)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddJob(Job.Default)
            .AddColumn(StatisticColumn.AllStatistics);
    }


    public static IConfig ForShortRun()
    {
        return ManualConfig.CreateEmpty()
            .AddExporter(MarkdownExporter.GitHub)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddJob(Job.ShortRun.WithWarmupCount(0).WithIterationCount(1));
    }
}
