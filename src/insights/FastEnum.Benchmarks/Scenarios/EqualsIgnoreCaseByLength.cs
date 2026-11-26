using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Internals;

namespace FastEnumUtility.Benchmarks.Scenarios;



/// <summary>
/// Case-insensitive comparison of an ASCII name, used by the case-insensitive name lookup.
/// </summary>
public class EqualsIgnoreCaseByLength
{
    private string _ascii = null!;
    private string _other = null!;


    [Params(1, 2, 3, 6, 12, 17, 24, 32)]
    public int Length { get; set; }


    [GlobalSetup]
    public void Setup()
    {
        var chars = new char[this.Length];
        for (var i = 0; i < chars.Length; i++)
            chars[i] = (char)('A' + (i % 26));
        this._ascii = new string(chars);
        this._other = this._ascii.ToLowerInvariant();
    }


    [Benchmark(Baseline = true)]
    public bool OrdinalIgnoreCase()
        => MemoryExtensions.Equals(this._ascii, this._other, StringComparison.OrdinalIgnoreCase);


    [Benchmark]
    public bool EqualsAscii()
        => StringHelper.CaseInsensitive.EqualsAscii(this._ascii, this._other);
}
