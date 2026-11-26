using System;
using System.Linq;
using System.Net;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class NonConstantInput
{
    #region Constants
    private const int Count = 64;
    #endregion


    #region Fields
    private Fruits[] _continuous = null!;
    private Fruits[] _continuousUndefined = null!;
    private HttpStatusCode[] _discontinuous = null!;
    private Permissions[] _flags = null!;

    private string[] _continuousNames = null!;
    private string[] _continuousNamesLower = null!;
    private string[] _continuousNamesUndefined = null!;
    private string[] _discontinuousNames = null!;
    private string[] _discontinuousNamesLower = null!;
    private string[] _discontinuousNamesUndefined = null!;
    private string[] _dayOfWeekNames = null!;
    private string[] _dayOfWeekNamesLower = null!;
    private string[] _consoleKeyNamesLower = null!;
    private Colors[] _colors = null!;
    #endregion


    [GlobalSetup]
    public void Setup()
    {
        // note:
        //  - Seed is fixed so that every run measures the same inputs.

        var random = new Random(42);

        this._continuous = pick(random, Enum.GetValues<Fruits>());
        this._discontinuous = pick(random, Enum.GetValues<HttpStatusCode>());
        this._flags = pick(random, Enum.GetValues<Permissions>());

        // note:
        //  - 100-149 aren't defined in Fruits.
        //  - The runtime caches the strings of small numbers (< 300), so ToString() of these values doesn't allocate.
        this._continuousUndefined = pick(random, Enumerable.Range(100, 50).Select(static x => (Fruits)x).ToArray());

        this._continuousNames = pick(random, Enum.GetNames<Fruits>());
        this._continuousNamesLower = this._continuousNames.Select(static x => x.ToLowerInvariant()).ToArray();
        this._continuousNamesUndefined = this._continuousNames.Select(static x => $"{x}_").ToArray();
        this._discontinuousNames = pick(random, Enum.GetNames<HttpStatusCode>());
        this._discontinuousNamesLower = this._discontinuousNames.Select(static x => x.ToLowerInvariant()).ToArray();
        this._discontinuousNamesUndefined = this._discontinuousNames.Select(static x => $"{x}_").ToArray();
        this._dayOfWeekNames = pick(random, Enum.GetNames<DayOfWeek>());
        this._dayOfWeekNamesLower = this._dayOfWeekNames.Select(static x => x.ToLowerInvariant()).ToArray();
        this._consoleKeyNamesLower = pick(random, Enum.GetNames<ConsoleKey>()).Select(static x => x.ToLowerInvariant()).ToArray();  // 1-18 chars
        this._colors = pick(random, Enum.GetValues<Colors>());

        _ = FastEnum.GetMembers<ConsoleKey>();
        _ = FastEnum.GetMembers<Colors>();
        _ = FastEnum.GetMembers<DayOfWeek>();
        _ = FastEnum.GetMembers<Fruits>();
        _ = FastEnum.GetMembers<HttpStatusCode>();
        _ = FastEnum.GetMembers<Permissions>();


        #region Local Functions
        static T[] pick<T>(Random random, T[] source)
        {
            return Enumerable
                .Range(0, Count)
                .Select(_ => source[random.Next(source.Length)])
                .ToArray();
        }
        #endregion
    }


    #region GetName
    [Benchmark(OperationsPerInvoke = Count)]
    public int GetName_Continuous()
    {
        var sum = 0;
        foreach (var x in this._continuous)
            sum += FastEnum.GetName(x)!.Length;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int GetName_Discontinuous()
    {
        var sum = 0;
        foreach (var x in this._discontinuous)
            sum += FastEnum.GetName(x)!.Length;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int GetName_Flags()
    {
        var sum = 0;
        foreach (var x in this._flags)
            sum += FastEnum.GetName(x)!.Length;
        return sum;
    }
    #endregion


    #region IsDefined (value)
    [Benchmark(OperationsPerInvoke = Count)]
    public int IsDefined_Continuous()
    {
        var sum = 0;
        foreach (var x in this._continuous)
            sum += FastEnum.IsDefined(x) ? 1 : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int IsDefined_Discontinuous()
    {
        var sum = 0;
        foreach (var x in this._discontinuous)
            sum += FastEnum.IsDefined(x) ? 1 : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int IsDefined_Flags()
    {
        var sum = 0;
        foreach (var x in this._flags)
            sum += FastEnum.IsDefined(x) ? 1 : 0;
        return sum;
    }
    #endregion


    #region IsDefined (name)
    [Benchmark(OperationsPerInvoke = Count)]
    public int IsDefinedName_Continuous()
    {
        var sum = 0;
        foreach (var x in this._continuousNames)
            sum += FastEnum.IsDefined<Fruits>(x) ? 1 : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int IsDefinedName_Discontinuous()
    {
        var sum = 0;
        foreach (var x in this._discontinuousNames)
            sum += FastEnum.IsDefined<HttpStatusCode>(x) ? 1 : 0;
        return sum;
    }
    #endregion


    #region ToString
    [Benchmark(OperationsPerInvoke = Count)]
    public int ToString_Continuous()
    {
        var sum = 0;
        foreach (var x in this._continuous)
            sum += FastEnum.ToString(x).Length;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int ToString_Continuous_Undefined()
    {
        var sum = 0;
        foreach (var x in this._continuousUndefined)
            sum += FastEnum.ToString(x).Length;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int ToString_Discontinuous()
    {
        var sum = 0;
        foreach (var x in this._discontinuous)
            sum += FastEnum.ToString(x).Length;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int ToString_Flags()
    {
        var sum = 0;
        foreach (var x in this._flags)
            sum += FastEnum.ToString(x).Length;
        return sum;
    }
    #endregion


    #region TryParse (case-sensitive)
    [Benchmark(OperationsPerInvoke = Count)]
    public int TryParse_Continuous()
    {
        var sum = 0;
        foreach (var x in this._continuousNames)
            sum += FastEnum.TryParse<Fruits>(x, out var result) ? (int)result : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int TryParse_Continuous_Undefined()
    {
        var sum = 0;
        foreach (var x in this._continuousNamesUndefined)
            sum += FastEnum.TryParse<Fruits>(x, out var result) ? (int)result : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int TryParse_Discontinuous()
    {
        var sum = 0;
        foreach (var x in this._discontinuousNames)
            sum += FastEnum.TryParse<HttpStatusCode>(x, out var result) ? (int)result : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int TryParse_Discontinuous_Undefined()
    {
        var sum = 0;
        foreach (var x in this._discontinuousNamesUndefined)
            sum += FastEnum.TryParse<HttpStatusCode>(x, out var result) ? (int)result : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int TryParse_DayOfWeek()
    {
        var sum = 0;
        foreach (var x in this._dayOfWeekNames)
            sum += FastEnum.TryParse<DayOfWeek>(x, out var result) ? (int)result : 0;
        return sum;
    }
    #endregion


    #region TryParse (case-insensitive)
    [Benchmark(OperationsPerInvoke = Count)]
    public int TryParseIgnoreCase_Continuous()
    {
        var sum = 0;
        foreach (var x in this._continuousNamesLower)
            sum += FastEnum.TryParse<Fruits>(x, ignoreCase: true, out var result) ? (int)result : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int TryParseIgnoreCase_Discontinuous()
    {
        var sum = 0;
        foreach (var x in this._discontinuousNamesLower)
            sum += FastEnum.TryParse<HttpStatusCode>(x, ignoreCase: true, out var result) ? (int)result : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int TryParseIgnoreCase_DayOfWeek()
    {
        var sum = 0;
        foreach (var x in this._dayOfWeekNamesLower)
            sum += FastEnum.TryParse<DayOfWeek>(x, ignoreCase: true, out var result) ? (int)result : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int TryParseIgnoreCase_ConsoleKey()
    {
        var sum = 0;
        foreach (var x in this._consoleKeyNamesLower)
            sum += FastEnum.TryParse<ConsoleKey>(x, ignoreCase: true, out var result) ? (int)result : 0;
        return sum;
    }
    #endregion


    #region TryParse (source generator)
    [Benchmark(OperationsPerInvoke = Count)]
    public int Booster_TryParse_Discontinuous()
    {
        var sum = 0;
        foreach (var x in this._discontinuousNames)
            sum += FastEnum.TryParse<HttpStatusCode, HttpStatusCodeBooster>(x, out var result) ? (int)result : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int Booster_TryParseIgnoreCase_Continuous()
    {
        var sum = 0;
        foreach (var x in this._continuousNamesLower)
            sum += FastEnum.TryParse<Fruits, FruitsBooster>(x, ignoreCase: true, out var result) ? (int)result : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int Booster_TryParseIgnoreCase_Discontinuous()
    {
        var sum = 0;
        foreach (var x in this._discontinuousNamesLower)
            sum += FastEnum.TryParse<HttpStatusCode, HttpStatusCodeBooster>(x, ignoreCase: true, out var result) ? (int)result : 0;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int Booster_TryParseIgnoreCase_ConsoleKey()
    {
        var sum = 0;
        foreach (var x in this._consoleKeyNamesLower)
            sum += FastEnum.TryParse<ConsoleKey, ConsoleKeyBooster>(x, ignoreCase: true, out var result) ? (int)result : 0;
        return sum;
    }
    #endregion


    #region Attributes
    [Benchmark(OperationsPerInvoke = Count)]
    public int GetLabel()
    {
        var sum = 0;
        foreach (var x in this._colors)
            sum += x.GetLabel(0)!.Length + x.GetLabel(1)!.Length;
        return sum;
    }


    [Benchmark(OperationsPerInvoke = Count)]
    public int GetEnumMemberValue()
    {
        var sum = 0;
        foreach (var x in this._continuous)
            sum += x.GetEnumMemberValue(throwIfNotFound: false)?.Length ?? 0;
        return sum;
    }
    #endregion
}
