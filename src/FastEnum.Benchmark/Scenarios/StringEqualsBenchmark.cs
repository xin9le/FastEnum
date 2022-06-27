using System;
using BenchmarkDotNet.Attributes;

namespace FastEnumUtility.Benchmark.Scenarios;



public class StringEqualsBenchmark
{
#pragma warning disable CS8618
    private string Lower { get; set; }
    private string Upper { get; set; }
#pragma warning restore CS8618


    [GlobalSetup]
    public void Setup()
    {
        var text = Guid.NewGuid().ToString();
        Lower = text.ToLower();
        Upper = text.ToUpper();
    }


    [Benchmark(Baseline = true)]
    public bool String_OrdinalIgnoreCase()
        => Lower.Equals(Upper, StringComparison.OrdinalIgnoreCase);


    [Benchmark]
    public bool Span_OrdinalIgnoreCase()
    {
        var lower = Lower.AsSpan();
        var upper = Upper.AsSpan();
        return lower.Equals(upper, StringComparison.OrdinalIgnoreCase);
    }


    [Benchmark]
    public bool String_InvariantCultureIgnoreCase()
        => Lower.Equals(Upper, StringComparison.InvariantCultureIgnoreCase);


    [Benchmark]
    public bool Span_InvariantCultureIgnoreCase()
    {
        var lower = Lower.AsSpan();
        var upper = Upper.AsSpan();
        return lower.Equals(upper, StringComparison.InvariantCultureIgnoreCase);
    }
}
