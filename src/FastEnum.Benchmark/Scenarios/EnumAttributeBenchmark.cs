using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;
using EnumsNET;
using FastEnum.Benchmark.Models;
using _FastEnum = FastEnum.FastEnum;

namespace FastEnum.Benchmark.Scenarios
{
    public class EnumAttributeBenchmark
    {
        private const Fruits Value = Fruits.Apple;


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = _FastEnum.GetNames<Fruits>();
        }

        [Benchmark(Baseline = true)]
        public string NetCore()
        {
            var type = typeof(Fruits);
            var name = Enum.GetName(type, Value);
            var info = type.GetField(name);
            var attr = info.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description;
        }

        [Benchmark]
        public string EnumsNet()
            => Value.GetAttributes().Get<DescriptionAttribute>()?.Description;

        [Benchmark]
        public string FastEnumFromMember()
            => Value.ToMember().GetAttribute<DescriptionAttribute>()?.Description;
        [Benchmark]
        public string FastEnumDirect()
            => Value.GetAttribute<Fruits, DescriptionAttribute>()?.Description;
    }
}
