using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;
using FastEnum.Benchmark.Models;
using _FastEnum = FastEnum.FastEnum;

namespace FastEnum.Benchmark.Scenarios
{
    public class EnumAttributesBenchmark
    {
        private const Fruits Value = Fruits.Apple;


        [GlobalSetup]
        public void Setup()
        {
            _ = Enum.GetNames(typeof(Fruits));
            _ = EnumsNET.Enums.GetNames<Fruits>();
            _ = _FastEnum.GetNames<Fruits>();
        }

        [Benchmark(Baseline = true)]
        public string[] NetCore()
        {
            var type = typeof(Fruits);
            var name = Enum.GetName(type, Value);
            var info = type.GetField(name);
            var attr = info.GetCustomAttributes<ColorAttribute>();
            return attr.Select(x => x.Color).ToArray();
        }

        [Benchmark]
        public string[] EnumsNet()
            => EnumsNET.Enums.GetAttributes(Value).GetAll<ColorAttribute>().Select(x => x.Color).ToArray();

        [Benchmark]
        public string[] FastEnumFromMember()
            => Value.ToMember().GetAttributes<ColorAttribute>().Select(x => x.Color).ToArray();
        [Benchmark]
        public string[] FastEnumDirect()
            => Value.GetAttributes<Fruits, ColorAttribute>().Select(x => x.Color).ToArray();
    }

}
