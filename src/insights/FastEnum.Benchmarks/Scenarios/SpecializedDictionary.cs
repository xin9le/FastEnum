using System.Collections.Frozen;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Internals;
using TEnum = FastEnumUtility.Benchmarks.Models.Fruits;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class SpecializedDictionary
{
    private static readonly ImmutableArray<TEnum> s_enumValues = FastEnum.GetValues<TEnum>();
    private static readonly FrozenDictionary<TEnum, TEnum> s_enumFronzen = s_enumValues.ToFrozenDictionary(static x => x);
    private static readonly FastReadOnlyDictionary<TEnum, TEnum> s_enumFastRO = s_enumValues.ToFastReadOnlyDictionary(static x => x);


    [Benchmark(Baseline = true)]
    public void EnumKey_FrozenDictionary()
    {
        foreach (var x in s_enumValues.AsSpan())
            _ = s_enumFronzen.TryGetValue(x, out var _);
    }


    [Benchmark]
    public void EnumKey_FastReadOnlyDictionary()
    {
        foreach (var x in s_enumValues.AsSpan())
            _ = s_enumFastRO.TryGetValue(x, out var _);
    }
}
