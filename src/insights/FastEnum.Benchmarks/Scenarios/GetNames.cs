extern alias FastEnumV1;
extern alias FastEnumV2;

using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class GetNames
{
    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = FastEnumV1::FastEnumUtility.FastEnum.GetNames<Fruits>();
        _ = FastEnumV2::FastEnumUtility.FastEnum.GetNames<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public IReadOnlyList<string> NetCore()
        => Enum.GetNames<Fruits>();


    [Benchmark]
    public IReadOnlyList<string> FastEnum_v1()
        => FastEnumV1::FastEnumUtility.FastEnum.GetNames<Fruits>();


    [Benchmark]
    public IReadOnlyList<string> FastEnum_v2()
        => FastEnumV2::FastEnumUtility.FastEnum.GetNames<Fruits>();
}
