﻿using System;
using System.Reflection;
using System.Runtime.Serialization;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using _FastEnum = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class EnumMemberAttributeBenchmark
{
    private const Fruits Value = Fruits.Apple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetValues<Fruits>();
        _ = _FastEnum.GetNames<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public string? NetCore()
    {
        var type = typeof(Fruits);
        var name = Enum.GetName(Value)!;
        var info = type.GetField(name);
        var attr = info!.GetCustomAttribute<EnumMemberAttribute>();
        return attr?.Value;
    }


    [Benchmark]
    public string? EnumsNet()
    {
        var attr = Enums.GetAttributes(Value)!.Get<EnumMemberAttribute>();
        return attr?.Value;
    }


    [Benchmark]
    public string? FastEnum()
        => Value.GetEnumMemberValue();
}
