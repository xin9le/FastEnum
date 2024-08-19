extern alias FastEnumV1;
extern alias FastEnumV2;

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class GetValues
{
    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetValues<Fruits>();
        _ = FastEnumV1::FastEnumUtility.FastEnum.GetValues<Fruits>();
        _ = FastEnumV2::FastEnumUtility.FastEnum.GetValues<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public IReadOnlyList<Fruits> NetCore()
        => Enum.GetValues<Fruits>();


    [Benchmark]
    public IReadOnlyList<Fruits> FastEnum_v1()
        => FastEnumV1::FastEnumUtility.FastEnum.GetValues<Fruits>();


    [Benchmark]
    public IReadOnlyList<Fruits> FastEnum_v2()
        => FastEnumV2::FastEnumUtility.FastEnum.GetValues<Fruits>();
}
