using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmark.Models;
using _FastEnum = FastEnumUtility.FastEnum;



namespace FastEnumUtility.Benchmark.Scenarios
{
    public class GetNamesBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = Enums.GetNames<Fruits>();
            _ = _FastEnum.GetNames<Fruits>();
        }


        [Benchmark(Baseline = true)]
        public void NetCore()
            => _ = Enum.GetNames(typeof(Fruits));


        [Benchmark]
        public void EnumsNet()
            => _ = Enums.GetNames<Fruits>();


        [Benchmark]
        public void FastEnum()
            => _ = _FastEnum.GetNames<Fruits>();
    }
}
