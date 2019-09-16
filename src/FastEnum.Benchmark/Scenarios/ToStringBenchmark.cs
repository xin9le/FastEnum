using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmark.Models;
using _FastEnum = FastEnumUtility.FastEnum;



namespace FastEnumUtility.Benchmark.Scenarios
{
    public class ToStringBenchmark
    {
        private const Fruits Value = Fruits.Pineapple;


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = _FastEnum.GetValues<Fruits>();
        }


        [Benchmark(Baseline = true)]
        public string NetCore()
            => Value.ToString();


        [Benchmark]
        public void FastEnum()
            => Value.ToName();
    }
}
