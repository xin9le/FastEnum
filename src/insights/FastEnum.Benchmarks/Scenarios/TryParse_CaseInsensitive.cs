using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class TryParse_CaseInsensitive
{
    private const string Value = nameof(Fruits.WaterMelon);


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetMembers<Fruits>();
        _ = FastEnum.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.TryParse<Fruits>(Value, true, out _);


    [Benchmark]
    public bool EnumsNet()
        => Enums.TryParse<Fruits>(Value, true, out _);


    [Benchmark]
    public bool FastEnum_Reflection()
        => FastEnum.TryParse<Fruits>(Value, true, out _);


    [Benchmark]
    public bool FastEnum_SourceGen()
        => FastEnum.TryParse<Fruits, FruitsBooster>(Value, true, out _);
}
