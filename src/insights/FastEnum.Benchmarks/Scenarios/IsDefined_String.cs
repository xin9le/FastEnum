using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;
using _FastEnum = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class IsDefined_String
{
    private const string Value = nameof(Fruits.Pineapple);


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = _FastEnum.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.IsDefined(typeof(Fruits), Value);


    [Benchmark]
    public bool NetEscapades()
        => FruitsExtensions.IsDefined(Value);


    [Benchmark]
    public bool FastEnum()
        => _FastEnum.IsDefined<Fruits, FruitsBooster>(Value);
}
