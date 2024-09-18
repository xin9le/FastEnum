using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class GetNames
{
    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetNames<Fruits>();
        _ = FastEnum.GetNames<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public IReadOnlyList<string> NetCore()
        => Enum.GetNames<Fruits>();


    [Benchmark]
    public IReadOnlyList<string> EnumsNet()
        => Enums.GetNames<Fruits>();


    [Benchmark]
    public IReadOnlyList<string> FastEnum_Reflection()
        => FastEnum.GetNames<Fruits>();
}
