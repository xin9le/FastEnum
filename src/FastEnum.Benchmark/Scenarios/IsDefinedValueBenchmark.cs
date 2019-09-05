using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class IsDefinedValueBenchmark
    {
        private const Fruits Value = Fruits.Pineapple;


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
        public void FastEnum()
            => FastEnum<Fruits>.IsDefined(Value);
    }
}
