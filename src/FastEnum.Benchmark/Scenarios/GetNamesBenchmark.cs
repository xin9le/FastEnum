using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class GetNamesBenchmark
    {
        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = Enums.GetNames<Fruits>();
            _ = FastEnum<Fruits>.Names;
        }


        [Benchmark(Baseline = true)]
        public string[] NetCore()
            => Enum.GetNames(typeof(Fruits));


        [Benchmark]
        public string[] EnumsNet()
            => Enums.GetNames<Fruits>().ToArray();


        [Benchmark]
        public string[] FastEnum()
            => FastEnum<Fruits>.Names;
    }
}
