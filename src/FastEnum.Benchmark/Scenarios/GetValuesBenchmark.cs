using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class GetValuesBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetValues(typeof(Fruits));
            _ = FastEnum<Fruits>.Values;
        }


        [Benchmark(Baseline = true)]
        public Fruits[] NetCore()
            => Enum.GetValues(typeof(Fruits)) as Fruits[];


        [Benchmark]
        public Fruits[] FastEnum()
            => FastEnum<Fruits>.Values;
    }
}
