using System;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnumUtility.Benchmark.Models;
using _FastEnum = FastEnumUtility.FastEnum;



namespace FastEnumUtility.Benchmark.Scenarios
{
    public class TryParseIgnoreCaseBenchmark
    {
        private const string Value = nameof(Fruits.WaterMelon);
        //private const string Value = "10";


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames<Fruits>();
            _ = Enums.GetValues<Fruits>();
            _ = _FastEnum.GetValues<Fruits>();
        }


        [Benchmark(Baseline = true)]
        public bool NetCore()
            => Enum.TryParse<Fruits>(Value, true, out _);


        [Benchmark]
        public bool EnumsNet()
            => Enums.TryParse<Fruits>(Value, true, out _);


        [Benchmark]
        public bool FastEnum()
            => _FastEnum.TryParse<Fruits>(Value, true, out _);
    }
}
