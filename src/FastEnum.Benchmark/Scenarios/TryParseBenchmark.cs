using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnum.Benchmark.Models;
using _FastEnum = FastEnum.FastEnum;



namespace FastEnum.Benchmark.Scenarios
{
    public class TryParseBenchmark
    {
        private const string Value = nameof(Fruits.WaterMelon);
        //private const string Value = "10";


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = Enums.GetValues<Fruits>();
            _ = _FastEnum.GetValues<Fruits>();
        }


        [Benchmark(Baseline = true)]
        public bool NetCore()
            => Enum.TryParse<Fruits>(Value, out _);


        [Benchmark]
        public bool EnumsNet()
            => Enums.TryParse<Fruits>(Value, out _);


        [Benchmark]
        public bool FastEnum()
            => _FastEnum.TryParse<Fruits>(Value, out _);
    }
}
