using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using _FastEnum = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class ToString_Defined
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
    public string NetCore()
        => Value.ToString();


    [Benchmark]
    public string EnumsNet()
        => Enums.AsString(Value);


    [Benchmark]
    public string FastEnum()
        => _FastEnum.ToString<Fruits, FruitsBooster>(Value);
}
