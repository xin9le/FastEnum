extern alias FastEnumV1;
extern alias FastEnumV2;

using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class TryParse_CaseSensitive
{
    private const string Value = nameof(Fruits.WaterMelon);


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = FastEnumV1::FastEnumUtility.FastEnum.GetMembers<Fruits>();
        _ = FastEnumV2::FastEnumUtility.FastEnum.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.TryParse<Fruits>(Value, out _);


    [Benchmark]
    public bool FastEnum_v1()
        => FastEnumV1::FastEnumUtility.FastEnum.TryParse<Fruits>(Value, out _);


    [Benchmark]
    public bool FastEnum_v2()
        => FastEnumV2::FastEnumUtility.FastEnum.TryParse<Fruits>(Value, out _);
}
