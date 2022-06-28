using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using _FastEnum = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class ToStringUndefinedBenchmark
{
    private const Fruits Value = (Fruits)byte.MaxValue;  // undefined value


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetNames<Fruits>();
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
        => Value.FastToString();
}
