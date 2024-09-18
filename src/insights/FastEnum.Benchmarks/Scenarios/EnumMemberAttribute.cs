using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using _FastEnum = FastEnumUtility.FastEnum;
using EnumMemberAttr = System.Runtime.Serialization.EnumMemberAttribute;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class EnumMemberAttribute
{
    private const Fruits Value = Fruits.Apple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetMembers<Fruits>();
        _ = _FastEnum.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public string? NetCore()
    {
        var type = typeof(Fruits);
        var name = Enum.GetName(Value)!;
        var info = type.GetField(name);
        var attr = info!.GetCustomAttribute<EnumMemberAttr>();
        return attr?.Value;
    }


    [Benchmark]
    public string? EnumsNet()
    {
        var attr = Enums.GetAttributes(Value)!.Get<EnumMemberAttr>();
        return attr?.Value;
    }


    [Benchmark]
    public string? FastEnum()
        => FastEnumExtensions.GetEnumMemberValue(Value);
}
