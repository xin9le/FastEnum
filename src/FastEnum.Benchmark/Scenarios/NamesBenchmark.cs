using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class NamesBenchmark
    {
        private const int LoopCount = 10000;


        [GlobalSetup]
        public void Setup()
        {
            var a = Enum.GetNames(typeof(Fruits));
            var b = FastEnum<Fruits>.Names;
            _ = a.Length;
            _ = b.Length;
        }


        [Benchmark(Baseline = true)]
        public void NetCore()
        {
            for (var i = 0; i < LoopCount; i++)
            {
                _ = Enum.GetNames(typeof(Fruits));
            }
        }


        [Benchmark]
        public void FastEnum()
        {
            for (var i = 0; i < LoopCount; i++)
            {
                _ = FastEnum<Fruits>.Names;
            }
        }
    }
}
