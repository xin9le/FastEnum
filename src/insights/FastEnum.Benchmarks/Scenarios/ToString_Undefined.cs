using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class ToString_Undefined
{
    private const Fruits Value = (Fruits)byte.MaxValue;  // undefined value


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetMembers<Fruits>();
        _ = FastEnum.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public string NetCore()
        => Value.ToString();


    [Benchmark]
    public string EnumsNet()
        => Enums.AsString(Value);


    [Benchmark]
    public string FastEnum_Reflection()
        => FastEnum.ToString(Value);
}
