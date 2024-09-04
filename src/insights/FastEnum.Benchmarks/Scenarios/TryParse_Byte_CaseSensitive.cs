extern alias FastEnumV1;

using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;
using FastEnum2 = FastEnumUtility.FastEnum;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class TryParse_Byte_CaseSensitive
{
    private const string Text = nameof(Fruits.Apple);
    private static readonly byte[] Bytes = [(byte)'A', (byte)'p', (byte)'p', (byte)'l', (byte)'e'];


    [GlobalSetup]
    public void Setup()
    {
        _ = Enum.GetNames<Fruits>();
        _ = FastEnum2.GetMembers<Fruits>();
    }


    [Benchmark(Baseline = true)]
    public bool NetCore()
        => Enum.TryParse<Fruits>(Text, out _);


    [Benchmark]
    public bool FastEnum_v2_String()
        => FastEnum2.TryParse<Fruits>(Text, out _);


    [Benchmark]
    public bool FastEnum_v2_Byte()
        => FastEnum2.TryParse<Fruits>(Bytes, out _);
}
