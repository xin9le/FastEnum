using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;

namespace FastEnumUtility.Benchmarks.Internals;



internal static class BenchmarkConfig
{
    public static IConfig ForDefault()
        => CreateSharedConfig(Job.Default);


    public static IConfig ForShortRun()
        => CreateSharedConfig(Job.ShortRun);


    /// <summary>
    /// Compares this source tree with another one (e.g. a worktree of the previous release) in one run.
    /// </summary>
    /// <param name="baselineLibs">The 'src/libs' directory of the baseline tree.</param>
    /// <remarks>
    /// Numbers of two separate runs drift by 10-15% on their own, so a comparison between them is not trustworthy.
    /// Here every benchmark is measured with both versions back to back, each in its own process built against its own FastEnum.Core.
    /// Two launches per job average out the placement of code and data, which is fixed for a process.
    /// The iterations are shorter than Job.Default to keep a comparison of a dozen benchmarks at about 5 minutes.
    /// </remarks>
    public static IConfig ForComparison(string baselineLibs)
    {
        var job = Job.Default
            .WithRuntime(CoreRuntime.Core11_0)
            .WithLaunchCount(2)
            .WithWarmupCount(5)
            .WithIterationCount(10)
            .WithIterationTime(Perfolizer.Horology.TimeInterval.FromMilliseconds(200));
        var baseline = job
            .WithArguments([new MsBuildArgument($"/p:FastEnumLibs=\"{baselineLibs.TrimEnd('\\', '/')}\"")])  // quoted: the path may contain spaces
            .WithId("baseline")
            .AsBaseline();
        var current = job.WithId("current");

        return ManualConfig
            .CreateMinimumViable()
            .AddExporter(MarkdownExporter.GitHub)
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddJob(baseline)
            .AddJob(current);
    }


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
            .AddJob(job.WithRuntime(CoreRuntime.Core11_0));
    }
    #endregion
}
