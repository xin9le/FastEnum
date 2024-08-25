extern alias FastEnumV1;

using System;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmarks.Models;
using EnumMemberAttr = System.Runtime.Serialization.EnumMemberAttribute;
using FastEnum1 = FastEnumV1::FastEnumUtility.FastEnum;
using FastEnum1Ex = FastEnumV1::FastEnumUtility.FastEnumExtensions;
using FastEnum2 = FastEnumUtility.FastEnum;
using FastEnum2Ex = FastEnumUtility.FastEnumExtensions;


namespace FastEnumUtility.Benchmarks.Scenarios;



public class EnumMemberAttribute
{
    private const Fruits Value = Fruits.Apple;


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = Enums.GetMembers<Fruits>();
        _ = FastEnum1.GetMembers<Fruits>();
        _ = FastEnum2.GetMembers<Fruits>();
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
    public string? FastEnum_v1()
        => FastEnum1Ex.GetEnumMemberValue(Value);


    [Benchmark]
    public string? FastEnum_v2()
        => FastEnum2Ex.GetEnumMemberValue(Value);
}
