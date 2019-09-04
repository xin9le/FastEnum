using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class ValuesBenchmark
    {
        private const int LoopCount = 10000;


        [GlobalSetup]
        public void Setup()
        {
            var a = Enum.GetValues(typeof(Fruits));
            var b = FastEnum<Fruits>.Values;
            _ = a.Length;
            _ = b.Length;
        }


        [Benchmark(Baseline = true)]
        public void NetCore()
        {
            for (var i = 0; i < LoopCount; i++)
            {
                _ = Enum.GetValues(typeof(Fruits));
            }
        }


        [Benchmark]
        public void FastEnum()
        {
            for (var i = 0; i < LoopCount; i++)
            {
                _ = FastEnum<Fruits>.Values;
            }
        }
    }
}
