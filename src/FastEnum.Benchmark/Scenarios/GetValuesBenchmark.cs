using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class GetValuesBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetValues(typeof(Fruits));
            _ = Enums.GetValues<Fruits>();
            _ = FastEnum<Fruits>.Values;
        }


        [Benchmark(Baseline = true)]
        public Fruits[] NetCore()
            => Enum.GetValues(typeof(Fruits)) as Fruits[];


        [Benchmark]
        public Fruits[] EnumsNet()
            => Enums.GetValues<Fruits>().ToArray();


        [Benchmark]
        public Fruits[] FastEnum()
            => FastEnum<Fruits>.Values;
    }
}
