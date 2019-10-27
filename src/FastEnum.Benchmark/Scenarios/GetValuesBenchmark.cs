using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmark.Models;
using _FastEnum = FastEnumUtility.FastEnum;



namespace FastEnumUtility.Benchmark.Scenarios
{
    public class GetValuesBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetValues(typeof(Fruits));
            _ = Enums.GetValues<Fruits>();
            _ = _FastEnum.GetValues<Fruits>();
        }


        [Benchmark(Baseline = true)]
        public void NetCore()
            => _ = Enum.GetValues(typeof(Fruits)) as Fruits[];


        [Benchmark]
        public void EnumsNet()
            => _ = Enums.GetValues<Fruits>();


        [Benchmark]
        public void FastEnum()
            => _ = _FastEnum.GetValues<Fruits>();
    }
}
