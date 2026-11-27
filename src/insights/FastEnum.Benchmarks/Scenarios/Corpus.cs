using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using BenchmarkDotNet.Attributes;

namespace FastEnumUtility.Benchmarks.Scenarios;



/// <summary>
/// Every scenario over a corpus of 512 public enums of the BCL, as one benchmark each.
/// </summary>
/// <remarks>
/// <para>
/// The other scenarios measure a few hand-picked enums, so a change of the lookup tables can look better or worse than it is.
/// Here each benchmark goes through all the enums of the corpus with 64 inputs each, so that every enum weighs the same,
/// and the result is the mean time of one operation over the whole corpus.
/// </para>
/// <para>
/// The enums are generic arguments known only at run time, so each one is called through a delegate to a generic method,
/// created once in the setup. The call costs the same for every version and is spread over the 64 inputs.
/// </para>
/// <para>
/// Inputs per enum: its names (Hit), the same in lower case (IgnoreCase), its names with one character in the middle replaced
/// by another letter (Typo; names of other enums if none of them is free), and names of other enums (Foreign).
/// GetName / ToString go through its defined values.
/// </para>
/// </remarks>
public class Corpus
{
    #region Constants
    private const int EnumCount = 512;
    private const int InputsPerEnum = 64;
    private const int Operations = EnumCount * InputsPerEnum;
    #endregion


    #region Fields
    private Func<object, long>[] _tryParse = null!;
    private Func<object, long>[] _tryParseIgnoreCase = null!;
    private Func<object, long>[] _getName = null!;
    private Func<object, long>[] _toString = null!;
    private object[] _hits = null!;
    private object[] _lower = null!;
    private object[] _typos = null!;
    private object[] _foreign = null!;
    private object[] _values = null!;
    #endregion


    [GlobalSetup]
    public void Setup()
    {
        var all = CollectEnums();
        var pool = all.SelectMany(static t => Enum.GetNames(t)).Distinct().ToArray();
        var random = new Random(42);

        var chosen = new List<Type>();
        var tryParse = new List<Func<object, long>>();
        var tryParseIgnoreCase = new List<Func<object, long>>();
        var getName = new List<Func<object, long>>();
        var toString = new List<Func<object, long>>();
        var hits = new List<object>();
        var lower = new List<object>();
        var typos = new List<object>();
        var foreign = new List<object>();
        var values = new List<object>();
        foreach (var type in all)
        {
            if (tryParse.Count == EnumCount)
                break;

            try
            {
                var names = Enum.GetNames(type);
                var defined = new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
                var shuffled = names.OrderBy(_ => random.Next()).ToArray();
                var foreignNames = pool.Where(x => !defined.Contains(x)).OrderBy(_ => random.Next()).Take(InputsPerEnum).ToArray();
                var typoNames = shuffled
                    .Select(static x =>
                    {
                        var chars = x.ToCharArray();
                        var i = chars.Length / 2;
                        var c = chars[i];
                        chars[i] = char.IsAsciiLetterLower(c) ? (char)('a' + ((c - 'a' + 1) % 26))
                            : char.IsAsciiLetterUpper(c) ? (char)('A' + ((c - 'A' + 1) % 26))
                            : 'x';
                        return new string(chars);
                    })
                    .Where(x => !defined.Contains(x) && !char.IsAsciiDigit(x[0]) && x[0] is not '+' and not '-')
                    .ToArray();
                var definedValues = Enum.GetValues(type);
                var cycledValues = Array.CreateInstance(type, InputsPerEnum);
                for (var i = 0; i < InputsPerEnum; i++)
                    cycledValues.SetValue(definedValues.GetValue(i % definedValues.Length), i);

                var p = create(nameof(TryParseOf), type);
                var pi = create(nameof(TryParseIgnoreCaseOf), type);
                var g = create(nameof(GetNameOf), type);
                var s = create(nameof(ToStringOf), type);
                var hit = cycle(shuffled);

                // An enum FastEnum cannot handle is left out of the corpus.
                _ = p(hit);
                _ = g(cycledValues);

                chosen.Add(type);
                tryParse.Add(p);
                tryParseIgnoreCase.Add(pi);
                getName.Add(g);
                toString.Add(s);
                hits.Add(hit);
                lower.Add(hit.Select(static x => x.ToLowerInvariant()).ToArray());
                typos.Add(cycle(typoNames.Length > 0 ? typoNames : foreignNames));
                foreign.Add(cycle(foreignNames));
                values.Add(cycledValues);
            }
            catch
            {
                // not supported by FastEnum; skip
            }
        }

        if (tryParse.Count != EnumCount)
            throw new InvalidOperationException($"The corpus has only {tryParse.Count} enums; {EnumCount} are needed.");

        // note:
        //  - Printed into the log, so that a comparison can check every job measured the same corpus.
        //  - FNV-1a over the type names and their member names; string.GetHashCode() is randomized per process.
        var checksum = 2166136261u;
        foreach (var c in chosen.SelectMany(static t => t.FullName + ":" + string.Join(",", Enum.GetNames(t)) + ";"))
            checksum = (checksum ^ c) * 16777619u;
        Console.WriteLine($"// Corpus: {chosen.Count} of {all.Count} enums, {chosen[0].FullName} .. {chosen[^1].FullName}, checksum {checksum:X8}");

        this._tryParse = [.. tryParse];
        this._tryParseIgnoreCase = [.. tryParseIgnoreCase];
        this._getName = [.. getName];
        this._toString = [.. toString];
        this._hits = [.. hits];
        this._lower = [.. lower];
        this._typos = [.. typos];
        this._foreign = [.. foreign];
        this._values = [.. values];


        #region Local Functions
        static Func<object, long> create(string name, Type type)
            => typeof(Corpus).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(type).CreateDelegate<Func<object, long>>();


        static string[] cycle(string[] source)
            => Enumerable.Range(0, InputsPerEnum).Select(i => source[i % source.Length]).ToArray();
        #endregion
    }


    #region Benchmarks
    [Benchmark(OperationsPerInvoke = Operations)]
    public long TryParse_Hit()
        => Run(this._tryParse, this._hits);


    [Benchmark(OperationsPerInvoke = Operations)]
    public long TryParse_IgnoreCase()
        => Run(this._tryParseIgnoreCase, this._lower);


    [Benchmark(OperationsPerInvoke = Operations)]
    public long TryParse_Typo()
        => Run(this._tryParse, this._typos);


    [Benchmark(OperationsPerInvoke = Operations)]
    public long TryParse_Foreign()
        => Run(this._tryParse, this._foreign);


    [Benchmark(OperationsPerInvoke = Operations)]
    public long GetName()
        => Run(this._getName, this._values);


    [Benchmark(OperationsPerInvoke = Operations)]
    public long ToString_()
        => Run(this._toString, this._values);
    #endregion


    #region Helpers
    private static long Run(Func<object, long>[] operations, object[] inputs)
    {
        long sum = 0;
        for (var i = 0; i < operations.Length; i++)
            sum += operations[i](inputs[i]);
        return sum;
    }


    private static long TryParseOf<T>(object inputs)
        where T : struct, Enum
    {
        long sum = 0;
        foreach (var x in (string[])inputs)
        {
            if (FastEnum.TryParse<T>(x, out var result))
                sum += Unsafe.As<T, byte>(ref result) + 1;
        }
        return sum;
    }


    private static long TryParseIgnoreCaseOf<T>(object inputs)
        where T : struct, Enum
    {
        long sum = 0;
        foreach (var x in (string[])inputs)
        {
            if (FastEnum.TryParse<T>(x, ignoreCase: true, out var result))
                sum += Unsafe.As<T, byte>(ref result) + 1;
        }
        return sum;
    }


    private static long GetNameOf<T>(object values)
        where T : struct, Enum
    {
        long sum = 0;
        foreach (var x in (T[])values)
            sum += FastEnum.GetName(x)?.Length ?? 0;
        return sum;
    }


    private static long ToStringOf<T>(object values)
        where T : struct, Enum
    {
        long sum = 0;
        foreach (var x in (T[])values)
            sum += FastEnum.ToString(x).Length;
        return sum;
    }


    private static List<Type> CollectEnums()
    {
        // note:
        //  - Every public enum of the System.* / Microsoft.Win32.* assemblies of the shared framework, ordered by name, so the corpus is the same on every run.
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
                    if (type.IsEnum && !type.ContainsGenericParameters && Enum.GetNames(type).Length > 0)
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
}
