using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class GetValues
{
    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetValues<Fruits>();
        _ = FastEnum.GetValues<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public IReadOnlyList<Fruits> NetCore()
        => Enum.GetValues<Fruits>();


    [Benchmark]
    public IReadOnlyList<Fruits> EnumsNet()
        => Enums.GetValues<Fruits>();


    [Benchmark]
    public IReadOnlyList<Fruits> FastEnum_Reflection()
        => FastEnum.GetValues<Fruits>();
}
