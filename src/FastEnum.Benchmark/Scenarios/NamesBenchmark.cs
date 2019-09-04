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
            int _;
            for (var i = 0; i < LoopCount; i++)
            {
                var a = Enum.GetNames(typeof(Fruits));
                _ = a.Length;
            }
        }


        [Benchmark]
        public void FastEnum()
        {
            int _;
            for (var i = 0; i < LoopCount; i++)
            {
                var a = FastEnum<Fruits>.Names;
                _ = a.Length;
            }
        }
    }
}
