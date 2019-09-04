using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class TryParseBenchmark
    {
        private const int LoopCount = 10000;
        private const string Value = nameof(Fruits.WaterMelon);
        //private const string Value = "10";


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
                _ = Enum.TryParse<Fruits>(Value, out var _);
            }
        }


        [Benchmark]
        public void FastEnum()
        {
            for (var i = 0; i < LoopCount; i++)
            {
                _ = FastEnum<Fruits>.TryParse(Value, out var _);
            }
        }
    }

}
