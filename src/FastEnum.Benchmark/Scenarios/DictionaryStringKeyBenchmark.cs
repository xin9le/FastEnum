using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;
using FastEnum.Internals;



namespace FastEnum.Benchmark.Scenarios
{
    public class DictionaryStringKeyBenchmark
    {
        private const string LookupKey = nameof(Fruits.Apple);


        private Dictionary<string, Member<Fruits>> Standard { get; set; }
        private FrozenDictionary<string, Member<Fruits>> StandardFrozen { get; set; }
        private StringKeyFrozenDictionary<Member<Fruits>> StringKeyFrozen { get; set; }
        private Hashtable Table { get; set; }


        [GlobalSetup]
        public void Setup()
        {
            var members = FastEnum<Fruits>.Members;
            this.Standard = members.ToDictionary(x => x.Name);
            this.StandardFrozen = members.ToFrozenDictionary(x => x.Name);
            this.StringKeyFrozen = members.ToStringKeyFrozenDictionary(x => x.Name);
            this.Table = new Hashtable(members.Length);
            foreach (var x in members)
                this.Table[x.Name] = x;
        }


        [Benchmark(Baseline = true)]
        public bool Dictionary()
            => this.Standard.TryGetValue(LookupKey, out _);


        [Benchmark]
        public bool FrozenDictionary()
            => this.StandardFrozen.TryGetValue(LookupKey, out _);


        [Benchmark]
        public bool StringKeyFrozenDictionary()
            => this.StringKeyFrozen.TryGetValue(LookupKey, out _);


        [Benchmark]
        public Member<Fruits> HashTable()
            => (Member<Fruits>)this.Table[LookupKey];
    }
}
