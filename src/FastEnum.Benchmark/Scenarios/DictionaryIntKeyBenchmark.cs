using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;
using FastEnum.Internals;



namespace FastEnum.Benchmark.Scenarios
{
    public class DictionaryIntKeyBenchmark
    {
        private const int LookupKey = (int)Fruits.Pear;


        private Dictionary<int, Member<Fruits>> Standard { get; set; }
        private FrozenDictionary<int, Member<Fruits>> GenericsKeyFrozen { get; set; }
        private IntKeyFrozenDictionary<Member<Fruits>> IntKeyFrozen { get; set; }



        [GlobalSetup]
        public void Setup()
        {
            var members = FastEnum<Fruits>.Members;
            this.Standard = members.ToDictionary(x => (int)x.Value);
            this.GenericsKeyFrozen = members.ToFrozenDictionary(x => (int)x.Value);
            this.IntKeyFrozen = members.ToIntKeyFrozenDictionary(x => (int)x.Value);
        }


        [Benchmark(Baseline = true)]
        public bool Dictionary()
            => this.Standard.TryGetValue(LookupKey, out _);


        [Benchmark]
        public bool FrozenDictionary()
            => this.GenericsKeyFrozen.TryGetValue(LookupKey, out _);


        [Benchmark]
        public bool IntKeyFrozenDictionary()
            => this.IntKeyFrozen.TryGetValue(LookupKey, out _);
    }
}
