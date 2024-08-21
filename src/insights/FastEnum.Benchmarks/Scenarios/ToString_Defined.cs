extern alias FastEnumV1;

using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;
using FastEnum1 = FastEnumV1::FastEnumUtility.FastEnum;
using FastEnum2 = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class ToString_Defined
{
    private const Fruits Value = Fruits.Pineapple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = FastEnum1.GetMembers<Fruits>();
        _ = FastEnum2.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public string NetCore()
        => Value.ToString();


    [Benchmark]
    public string FastEnum_v1()
        => FastEnum1.ToString(Value);


    [Benchmark]
    public string FastEnum_v2()
        => FastEnum2.ToString(Value);
}
