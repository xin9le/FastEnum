extern alias FastEnumV1;

using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;
using FastEnum1 = FastEnumV1::FastEnumUtility.FastEnum;
using FastEnum2 = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class GetName
{
    private const Fruits Value = Fruits.Pineapple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = FastEnum1.GetNames<Fruits>();
        _ = FastEnum2.GetNames<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public string? NetCore()
        => Enum.GetName(Value);


    [Benchmark]
    public string? FastEnum_v1()
        => FastEnum1.GetName(Value);


    [Benchmark]
    public string? FastEnum_v2()
        => FastEnum2.GetName(Value);
}
