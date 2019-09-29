using System;
using System.Collections.Generic;
using System.Linq;
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
        public IReadOnlyList<string> NetCore()
            => Enum.GetNames(typeof(Fruits));


        [Benchmark]
        public IReadOnlyList<string> EnumsNet()
            => Enums.GetNames<Fruits>().ToArray();


        [Benchmark]
        public IReadOnlyList<string> FastEnum()
            => _FastEnum.GetNames<Fruits>();
    }
}
