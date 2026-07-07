using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace FastEnumUtility.Benchmarks.Internals;



internal static class BenchmarkConfig
{
    public static IConfig ForDefault()
        => CreateSharedConfig(Job.Default);


    public static IConfig ForShortRun()
        => CreateSharedConfig(Job.ShortRun);


    #region Helpers
    private static ManualConfig CreateSharedConfig(Job job)
    {
        return ManualConfig
            .CreateMinimumViable()
            .AddExporter(MarkdownExporter.GitHub)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddDiagnoser(new DisassemblyDiagnoser(new
            (
                maxDepth: 3,
                printSource: true,
                exportGithubMarkdown: true,
                exportCombinedDisassemblyReport: true
            )))
            .AddJob(job);
    }
    #endregion
}
