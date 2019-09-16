using System;
using System.Collections.Generic;
using System.Linq;
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
        public IReadOnlyList<Fruits> NetCore()
            => Enum.GetValues(typeof(Fruits)) as Fruits[];


        [Benchmark]
        public IReadOnlyList<Fruits> EnumsNet()
            => Enums.GetValues<Fruits>().ToArray();


        [Benchmark]
        public IReadOnlyList<Fruits> FastEnum()
            => _FastEnum.GetValues<Fruits>();
    }
}
