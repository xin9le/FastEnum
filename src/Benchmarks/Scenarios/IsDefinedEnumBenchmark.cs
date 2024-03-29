﻿using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using _FastEnum = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class IsDefinedEnumBenchmark
{
    private const Fruits Value = Fruits.Pineapple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetValues<Fruits>();
        _ = _FastEnum.GetValues<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.IsDefined(Value);


    [Benchmark]
    public bool EnumsNet()
        => Enums.IsDefined(Value);


    [Benchmark]
    public void FastEnum()
        => _FastEnum.IsDefined(Value);
}
