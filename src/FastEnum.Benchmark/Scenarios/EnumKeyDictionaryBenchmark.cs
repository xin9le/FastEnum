using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;
using FastEnum.Internals;



namespace FastEnum.Benchmark.Scenarios
{
    public class EnumKeyDictionaryBenchmark
    {
        private const Fruits LookupKey = Fruits.Pear;


        private Dictionary<Fruits, Member<Fruits>> Standard { get; set; }
        private FrozenDictionary<Fruits, Member<Fruits>> Frozen { get; set; }


        [GlobalSetup]
        public void Setup()
        {
            var members = FastEnum<Fruits>.Members;
            this.Standard = members.ToDictionary(x => x.Value);
            this.Frozen = members.ToFrozenDictionary(x => x.Value);
        }


        [Benchmark(Baseline = true)]
        public bool Dictionary()
            => this.Standard.TryGetValue(LookupKey, out _);


        [Benchmark]
        public bool FrozenDictionary()
            => this.Frozen.TryGetValue(LookupKey, out _);
    }
}
