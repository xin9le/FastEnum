using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using _FastEnum = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class GetValues
{
    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetValues<Fruits>();
        _ = _FastEnum.GetValues<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public IReadOnlyList<Fruits> NetCore()
        => Enum.GetValues<Fruits>();


    [Benchmark]
    public IReadOnlyList<Fruits> EnumsNet()
        => Enums.GetValues<Fruits>();


    [Benchmark]
    public Fruits[] NetEscapades()
        => FruitsExtensions.GetValues();


    [Benchmark]
    public ImmutableArray<Fruits> FastEnum()
        => _FastEnum.GetValues<Fruits>();
}
