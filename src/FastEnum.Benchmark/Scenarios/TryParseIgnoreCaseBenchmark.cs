using System;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;



namespace FastEnum.Benchmark.Scenarios
{
    public class TryParseIgnoreCaseBenchmark
    {
        private const string Value = nameof(Fruits.WaterMelon);
        //private const string Value = "10";


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = FastEnum<Fruits>.Values;
        }


        [Benchmark(Baseline = true)]
        public bool NetCore()
            => Enum.TryParse<Fruits>(Value, true, out _);


        [Benchmark]
        public bool FastEnum()
            => FastEnum<Fruits>.TryParse(Value, true, out _);
    }
}
