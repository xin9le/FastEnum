extern alias FastEnumV1;
extern alias FastEnumV2;

using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class GetName
{
    private const Fruits Value = Fruits.Pineapple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = FastEnumV1::FastEnumUtility.FastEnum.GetNames<Fruits>();
        _ = FastEnumV2::FastEnumUtility.FastEnum.GetNames<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public string? NetCore()
        => Enum.GetName(Value);


    [Benchmark]
    public string? FastEnum_v1()
        => FastEnumV1::FastEnumUtility.FastEnum.GetName(Value);


    [Benchmark]
    public string? FastEnum_v2()
        => FastEnumV2::FastEnumUtility.FastEnum.GetName(Value);
}
