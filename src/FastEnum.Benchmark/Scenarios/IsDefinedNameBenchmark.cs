using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class IsDefinedNameBenchmark
    {
        private const string Value = nameof(Fruits.Pineapple);


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = FastEnum<Fruits>.Values;
        }


        [Benchmark(Baseline = true)]
        public bool NetCore()
            => Enum.IsDefined(typeof(Fruits), Value);


        [Benchmark]
        public bool FastEnum()
            => FastEnum<Fruits>.IsDefined(Value);
    }
}
