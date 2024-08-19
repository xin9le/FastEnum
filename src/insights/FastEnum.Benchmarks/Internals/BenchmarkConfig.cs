using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace FastEnumUtility.Benchmarks.Internals;



internal static class BenchmarkConfig
{
    public static IConfig ForDefault()
    {
        return ManualConfig.CreateMinimumViable()
            .AddExporter(MarkdownExporter.GitHub)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddJob(Job.Default);
    }


    public static IConfig ForShortRun()
    {
        return ManualConfig.CreateMinimumViable()
            .AddExporter(MarkdownExporter.GitHub)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddJob(Job.ShortRun);
    }
}
