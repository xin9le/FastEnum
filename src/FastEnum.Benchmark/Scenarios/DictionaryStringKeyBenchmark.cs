using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmark.Models;
using FastEnumUtility.Internals;

namespace FastEnumUtility.Benchmark.Scenarios;



public class DictionaryStringKeyBenchmark
{
    private const string LookupKey = nameof(Fruits.Apple);


#pragma warning disable CS8618
    private Dictionary<string, Member<Fruits>> Standard { get; set; }
    private FrozenDictionary<string, Member<Fruits>> GenericsKeyFrozen { get; set; }
    private FrozenStringKeyDictionary<Member<Fruits>> StringKeyFrozen { get; set; }
    private Hashtable Table { get; set; }
#pragma warning restore CS8618


    [GlobalSetup]
    public void Setup()
    {
        var members = FastEnum.GetMembers<Fruits>();
        this.Standard = members.ToDictionary(static x => x.Name);
        this.GenericsKeyFrozen = members.ToFrozenDictionary(static x => x.Name);
        this.StringKeyFrozen = members.ToFrozenStringKeyDictionary(static x => x.Name);
        this.Table = new Hashtable(members.Count);
        foreach (var x in members)
            this.Table[x.Name] = x;
    }


    [Benchmark(Baseline = true)]
    public bool Dictionary()
        => this.Standard.TryGetValue(LookupKey, out _);


    [Benchmark]
    public bool FrozenDictionary()
        => this.GenericsKeyFrozen.TryGetValue(LookupKey, out _);


    [Benchmark]
    public bool FrozenStringKeyDictionary()
        => this.StringKeyFrozen.TryGetValue(LookupKey, out _);


    [Benchmark]
    public Member<Fruits> HashTable()
        => (Member<Fruits>)this.Table[LookupKey]!;
}
