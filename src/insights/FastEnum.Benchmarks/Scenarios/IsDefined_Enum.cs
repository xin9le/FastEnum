using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using _FastEnum = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class IsDefined_Enum
{
    private const Fruits Value = Fruits.Pineapple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetMembers<Fruits>();
        _ = _FastEnum.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.IsDefined(Value);


    [Benchmark]
    public bool EnumsNet()
        => Enums.IsDefined(Value);


    [Benchmark]
    public bool FastEnum()
        => _FastEnum.IsDefined<Fruits, FruitsBooster>(Value);
}
