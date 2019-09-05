using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class NamesBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = FastEnum<Fruits>.Names;
        }


        [Benchmark(Baseline = true)]
        public string[] NetCore()
            => Enum.GetNames(typeof(Fruits));


        [Benchmark]
        public string[] FastEnum()
            => FastEnum<Fruits>.Names;
    }
}
