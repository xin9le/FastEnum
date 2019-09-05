using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class ToStringBenchmark
    {
        private const Fruits Value = Fruits.Pineapple;


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = FastEnum<Fruits>.Values;
        }


        [Benchmark(Baseline = true)]
        public string NetCore()
            => Value.ToString();


        [Benchmark]
        public void FastEnum()
            => Value.ToName();
    }
}
