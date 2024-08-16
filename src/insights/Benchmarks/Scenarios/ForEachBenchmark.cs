using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Internals;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class ForEachBenchmark
{
#pragma warning disable CS8618
    private ReadOnlyArray<int> _ReadOnlyArray { get; set; }
    private ReadOnlyCollection<int> _ReadOnlyCollection { get; set; }
    private IReadOnlyList<int> _IReadOnlyList { get; set; }
    private List<int> _List { get; set; }
    private int[] _Array { get; set; }
#pragma warning restore CS8618


    [GlobalSetup]
    public void Setup()
    {
        var raw = Enumerable.Range(1, 100);
        this._ReadOnlyArray = new ReadOnlyArray<int>(raw.ToArray());
        this._ReadOnlyCollection = new ReadOnlyCollection<int>(raw.ToList());
        this._IReadOnlyList = raw.ToArray();
        this._List = raw.ToList();
        this._Array = raw.ToArray();
    }


    [Benchmark(Baseline = true)]
    public int Array()
    {
        var sum = 0;
        foreach (var x in this._Array)
            sum += x;
        return sum;
    }


    [Benchmark]
    public int List()
    {
        var sum = 0;
        foreach (var x in this._List)
            sum += x;
        return sum;
    }


    [Benchmark]
    public int ReadOnlyCollection()
    {
        var sum = 0;
        foreach (var x in this._ReadOnlyCollection)
            sum += x;
        return sum;
    }


    [Benchmark]
    public int IReadOnlyList()
    {
        var sum = 0;
        foreach (var x in this._IReadOnlyList)
            sum += x;
        return sum;
    }


    [Benchmark]
    public int ReadOnlyArray()
    {
        var sum = 0;
        foreach (var x in this._ReadOnlyArray)
            sum += x;
        return sum;
    }
}
