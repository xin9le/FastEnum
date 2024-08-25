extern alias FastEnumV1;

using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;
using FastEnum1 = FastEnumV1::FastEnumUtility.FastEnum;
using FastEnum2 = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class IsDefined_String
{
    private const string Value = nameof(Fruits.Pineapple);


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = FastEnum1.GetMembers<Fruits>();
        _ = FastEnum2.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.IsDefined(typeof(Fruits), Value);


    [Benchmark]
    public bool FastEnum_v1()
        => FastEnum1.IsDefined<Fruits>(Value);


    [Benchmark]
    public bool FastEnum_v2()
        => FastEnum2.IsDefined<Fruits>(Value);
}
