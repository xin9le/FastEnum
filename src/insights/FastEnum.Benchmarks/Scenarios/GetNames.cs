extern alias FastEnumV1;

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;
using FastEnum1 = FastEnumV1::FastEnumUtility.FastEnum;
using FastEnum2 = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class GetNames
{
    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = FastEnum1.GetNames<Fruits>();
        _ = FastEnum2.GetNames<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public IReadOnlyList<string> NetCore()
        => Enum.GetNames<Fruits>();


    [Benchmark]
    public IReadOnlyList<string> FastEnum_v1()
        => FastEnum1.GetNames<Fruits>();


    [Benchmark]
    public IReadOnlyList<string> FastEnum_v2()
        => FastEnum2.GetNames<Fruits>();
}
