using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class GetNameBenchmark
    {
        private const Fruits Value = Fruits.Pineapple;


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = FastEnum<Fruits>.Names;
        }


        [Benchmark(Baseline = true)]
        public string NetCore()
            => Enum.GetName(typeof(Fruits), Value);


        [Benchmark]
        public string FastEnum()
            => Value.ToName();
    }
}
