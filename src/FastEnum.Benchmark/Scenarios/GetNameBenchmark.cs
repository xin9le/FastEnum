using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnum.Benchmark.Models;
using _FastEnum = FastEnum.FastEnum;



namespace FastEnum.Benchmark.Scenarios
{
    public class GetNameBenchmark
    {
        private const Fruits Value = Fruits.Pineapple;


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = Enums.GetNames<Fruits>();
            _ = _FastEnum.GetNames<Fruits>();
        }


        [Benchmark(Baseline = true)]
        public string NetCore()
            => Enum.GetName(typeof(Fruits), Value);


        [Benchmark]
        public string EnumsNet()
            => Enums.GetName(Value);


        [Benchmark]
        public string FastEnum()
            => _FastEnum.GetName(Value);
    }
}
