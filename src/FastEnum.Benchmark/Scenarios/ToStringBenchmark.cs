using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class ToStringBenchmark
    {
        private const int LoopCount = 10000;
        private const Fruits Value = Fruits.Pineapple;


        [GlobalSetup]
        public void Setup()
        {
            var a = Enum.GetNames(typeof(Fruits));
            var b = FastEnum<Fruits>.Values;
            _ = a.Length;
            _ = b.Length;
        }


        [Benchmark(Baseline = true)]
        public void NetCore()
        {
            for (var i = 0; i < LoopCount; i++)
            {
                _ = Value.ToString();
            }
        }


        [Benchmark]
        public void FastEnum()
        {
            for (var i = 0; i < LoopCount; i++)
            {
                _ = Value.ToName();
            }
        }
    }

}
