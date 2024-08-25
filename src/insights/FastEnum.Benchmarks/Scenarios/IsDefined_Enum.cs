extern alias FastEnumV1;

using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using FastEnum1 = FastEnumV1::FastEnumUtility.FastEnum;
using FastEnum2 = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class IsDefined_Enum
{
    private const Fruits Value = Fruits.Pineapple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetMembers<Fruits>();
        _ = FastEnum1.GetMembers<Fruits>();
        _ = FastEnum2.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.IsDefined(Value);


    [Benchmark]
    public bool EnumsNet()
        => Enums.IsDefined(Value);


    [Benchmark]
    public bool FastEnum_v1()
        => FastEnum1.IsDefined(Value);


    [Benchmark]
    public bool FastEnum_v2()
        => FastEnum2.IsDefined(Value);
}
