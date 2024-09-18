using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class IsDefined_Enum
{
    private const Fruits Value = Fruits.Pineapple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetMembers<Fruits>();
        _ = FastEnum.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.IsDefined(Value);


    [Benchmark]
    public bool EnumsNet()
        => Enums.IsDefined(Value);


    [Benchmark]
    public bool FastEnum_Reflection()
        => FastEnum.IsDefined(Value);
}
