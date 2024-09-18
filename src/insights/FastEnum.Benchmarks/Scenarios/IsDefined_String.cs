using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class IsDefined_String
{
    private const string Value = nameof(Fruits.Pineapple);


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = FastEnum.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.IsDefined(typeof(Fruits), Value);


    [Benchmark]
    public bool FastEnum_Reflection()
        => FastEnum.IsDefined<Fruits>(Value);
}
