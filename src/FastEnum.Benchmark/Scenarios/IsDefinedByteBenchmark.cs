using System;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmark.Models;
using _FastEnum = FastEnumUtility.FastEnum;



namespace FastEnumUtility.Benchmark.Scenarios
{
    public class IsDefinedByteBenchmark
    {
        private const byte Value = (byte)Fruits.Cherry;


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames<Fruits>();
            _ = _FastEnum.GetValues<Fruits>();
        }


        [Benchmark(Baseline = true)]
        public bool NetCore()
            => Enum.IsDefined(typeof(Fruits), Value);


        [Benchmark]
        public void FastEnum()
            => _FastEnum.IsDefined<Fruits>(Value);
    }
}
