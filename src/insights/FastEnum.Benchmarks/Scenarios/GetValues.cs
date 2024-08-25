extern alias FastEnumV1;

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using FastEnum1 = FastEnumV1::FastEnumUtility.FastEnum;
using FastEnum2 = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class GetValues
{
    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetValues<Fruits>();
        _ = FastEnum1.GetValues<Fruits>();
        _ = FastEnum2.GetValues<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public IReadOnlyList<Fruits> NetCore()
        => Enum.GetValues<Fruits>();


    [Benchmark]
    public IReadOnlyList<Fruits> EnumsNet()
        => Enums.GetValues<Fruits>();


    [Benchmark]
    public IReadOnlyList<Fruits> FastEnum_v1()
        => FastEnum1.GetValues<Fruits>();


    [Benchmark]
    public IReadOnlyList<Fruits> FastEnum_v2()
        => FastEnum2.GetValues<Fruits>();
}
