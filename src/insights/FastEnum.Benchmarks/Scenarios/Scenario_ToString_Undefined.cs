extern alias FastEnumV1;
extern alias FastEnumV2;

using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class Scenario_ToString_Undefined
{
    private const Fruits Value = (Fruits)byte.MaxValue;  // undefined value


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = FastEnumV1::FastEnumUtility.FastEnum.GetMembers<Fruits>();
        _ = FastEnumV2::FastEnumUtility.FastEnum.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public string NetCore()
        => Value.ToString();


    [Benchmark]
    public string FastEnum_v1()
        => FastEnumV1::FastEnumUtility.FastEnum.ToString(Value);


    [Benchmark]
    public string FastEnum_v2()
        => FastEnumV2::FastEnumUtility.FastEnum.ToString(Value);
}
