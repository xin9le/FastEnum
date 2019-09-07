using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;
using _FastEnum = FastEnum.FastEnum;



namespace FastEnum.Benchmark.Scenarios
{
    public class IsDefinedStringBenchmark
    {
        private const string Value = nameof(Fruits.Pineapple);


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
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
