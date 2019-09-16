using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmark.Models;
using FastEnumUtility.Internals;



namespace FastEnumUtility.Benchmark.Scenarios
{
    public class DictionaryIntKeyBenchmark
    {
        private const int LookupKey = (int)Fruits.Pear;


        private Dictionary<int, Member<Fruits>> Standard { get; set; }
        private FrozenDictionary<int, Member<Fruits>> GenericsKeyFrozen { get; set; }
        private FrozenIntKeyDictionary<Member<Fruits>> IntKeyFrozen { get; set; }



        [GlobalSetup]
        public void Setup()
        {
            var members = FastEnum.GetMembers<Fruits>();
            this.Standard = members.ToDictionary(x => (int)x.Value);
            this.GenericsKeyFrozen = members.ToFrozenDictionary(x => (int)x.Value);
            this.IntKeyFrozen = members.ToFrozenIntKeyDictionary(x => (int)x.Value);
        }


        [Benchmark(Baseline = true)]
        public bool Dictionary()
            => this.Standard.TryGetValue(LookupKey, out _);


        [Benchmark]
        public bool FrozenDictionary()
            => this.GenericsKeyFrozen.TryGetValue(LookupKey, out _);


        [Benchmark]
        public bool FrozenIntKeyDictionary()
            => this.IntKeyFrozen.TryGetValue(LookupKey, out _);
    }
}
