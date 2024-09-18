using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using _FastEnum = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class TryParse_CaseInsensitive
{
    private const string Value = nameof(Fruits.WaterMelon);


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetMembers<Fruits>();
        _ = _FastEnum.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.TryParse<Fruits>(Value, true, out _);


    [Benchmark]
    public bool EnumsNet()
        => Enums.TryParse<Fruits>(Value, true, out _);


    [Benchmark]
    public bool FastEnum()
        => _FastEnum.TryParse<Fruits, FruitsBooster>(Value, true, out _);
}
