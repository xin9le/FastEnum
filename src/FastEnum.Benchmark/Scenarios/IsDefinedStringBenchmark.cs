using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmark.Models;
using _FastEnum = FastEnumUtility.FastEnum;



namespace FastEnumUtility.Benchmark.Scenarios
{
    public class IsDefinedStringBenchmark
    {
        private const string Value = nameof(Fruits.Pineapple);


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames<Fruits>();
            _ = _FastEnum.GetValues<Fruits>();
        }


        [Benchmark(Baseline = true)]
        public bool NetCore()
            => Enum.IsDefined(typeof(Fruits), Value);


        [Benchmark]
        public bool FastEnum()
            => _FastEnum.IsDefined<Fruits>(Value);
    }
}
