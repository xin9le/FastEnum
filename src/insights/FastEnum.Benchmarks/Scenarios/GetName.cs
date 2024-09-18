using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class GetName
{
    private const Fruits Value = Fruits.Pineapple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetNames<Fruits>();
        _ = FastEnum.GetNames<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public string? NetCore()
        => Enum.GetName(Value);


    [Benchmark]
    public string? EnumsNet()
        => Enums.GetName(Value);


    [Benchmark]
    public string? FastEnum_Reflection()
        => FastEnum.GetName(Value);


    [Benchmark]
    public string? FastEnum_SourceGen()
        => FastEnum.GetName<Fruits, FruitsBooster>(Value);
}
