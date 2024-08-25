using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
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
            .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80));
    }


    public static IConfig ForShortRun()
    {
        return ManualConfig.CreateMinimumViable()
            .AddExporter(MarkdownExporter.GitHub)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddJob(Job.ShortRun.WithRuntime(CoreRuntime.Core80));
    }
}
