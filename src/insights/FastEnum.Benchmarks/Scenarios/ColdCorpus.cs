using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;

namespace FastEnumUtility.Benchmarks.Scenarios;



/// <summary>
/// The first use of FastEnum for each enum of the corpus, in a fresh process every time.
/// </summary>
/// <remarks>
/// <para>
/// The same 512 BCL enums as <see cref="Corpus"/>. Every launch is a new process with one iteration of one invocation and no warmup
/// (see <see cref="ColdStartAttribute"/>), so a measurement covers everything the first use costs:
/// reflection over the fields, building the tables, and JIT of the generic code for every enum type.
/// </para>
/// <para>
/// The setup only finds the types and creates the delegates; it does not call them, nor touch the enums through Enum.GetNames() and the like,
/// so that it does not warm up anything FastEnum would do itself.
/// </para>
/// </remarks>
[ColdStart]
public class ColdCorpus
{
    #region Constants
    private const int EnumCount = 512;
    #endregion


    #region Fields
    private Action[] _getName = null!;
    private Action[] _tryParse = null!;
    private Action[] _tryParseIgnoreCase = null!;
    #endregion


    [GlobalSetup]
    public void Setup()
    {
        var types = CollectEnums().Take(EnumCount).ToArray();
        if (types.Length != EnumCount)
            throw new InvalidOperationException($"The corpus has only {types.Length} enums; {EnumCount} are needed.");

        this._getName = create(nameof(GetNameOf));
        this._tryParse = create(nameof(TryParseOf));
        this._tryParseIgnoreCase = create(nameof(TryParseIgnoreCaseOf));

        // note:
        //  - Printed into the log, so that a comparison can check every job measured the same corpus.
        var checksum = 2166136261u;
        foreach (var c in types.SelectMany(static t => t.FullName + ";"))
            checksum = (checksum ^ c) * 16777619u;
        Console.WriteLine($"// ColdCorpus: {types.Length} enums, {types[0].FullName} .. {types[^1].FullName}, checksum {checksum:X8}");


        #region Local Functions
        Action[] create(string name)
        {
            var method = typeof(ColdCorpus).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!;
            return types.Select(t => method.MakeGenericMethod(t).CreateDelegate<Action>()).ToArray();
        }
        #endregion
    }


    #region Benchmarks
    [Benchmark]
    public void Cold_GetName()
        => Run(this._getName);


    [Benchmark]
    public void Cold_TryParse()
        => Run(this._tryParse);


    [Benchmark]
    public void Cold_TryParseIgnoreCase()
        => Run(this._tryParseIgnoreCase);


    [Benchmark]
    public void Cold_All()
    {
        Run(this._getName);
        Run(this._tryParse);
        Run(this._tryParseIgnoreCase);
    }
    #endregion


    #region Helpers
    private static object? s_sink;


    private static void Run(Action[] actions)
    {
        foreach (var x in actions)
            x();
    }


    private static void GetNameOf<T>()
        where T : struct, Enum
        => s_sink = FastEnum.GetName(default(T));


    private static void TryParseOf<T>()
        where T : struct, Enum
    {
        _ = FastEnum.TryParse<T>("Zzz", out var result);
        s_sink = result;
    }


    private static void TryParseIgnoreCaseOf<T>()
        where T : struct, Enum
    {
        _ = FastEnum.TryParse<T>("zzz", ignoreCase: true, out var result);
        s_sink = result;
    }


    private static List<Type> CollectEnums()
    {
        // note:
        //  - The same selection as Corpus, without Enum.GetNames() (it would warm up the runtime's own enum cache that FastEnum uses).
        var paths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(System.IO.Path.PathSeparator);
        var result = new HashSet<Type>();
        foreach (var path in paths)
        {
            var file = System.IO.Path.GetFileName(path);
            if (!file.StartsWith("System.", StringComparison.Ordinal) && !file.StartsWith("Microsoft.Win32.", StringComparison.Ordinal))
                continue;
            try
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(AssemblyName.GetAssemblyName(path));
                foreach (var type in assembly.GetExportedTypes())
                {
                    if (type.IsEnum && !type.ContainsGenericParameters)
                        result.Add(type);
                }
            }
            catch
            {
                // not loadable on this platform; skip
            }
        }
        return [.. result.OrderBy(static t => t.FullName, StringComparer.Ordinal)];
    }
    #endregion



    /// <summary>
    /// Turns every job into a cold start: a new process per measurement, one iteration of one invocation, no warmup.
    /// </summary>
    /// <remarks>It mutates the jobs of the config, so the baseline / current jobs of a comparison keep their builds.</remarks>
    private sealed class ColdStartAttribute()
        : JobMutatorConfigBaseAttribute(Job.Default
            .WithStrategy(RunStrategy.ColdStart)
            .WithLaunchCount(15)
            .WithWarmupCount(0)
            .WithIterationCount(1)
            .WithInvocationCount(1)
            .WithUnrollFactor(1));
}
