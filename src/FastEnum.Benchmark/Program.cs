using BenchmarkDotNet.Running;
using FastEnum.Benchmark.Configurations;
using FastEnum.Benchmark.Scenarios;



namespace FastEnum.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[]
                {
                typeof(GetValuesBenchmark),
                typeof(GetNamesBenchmark),
                typeof(GetNameBenchmark),
                typeof(TryParseBenchmark),
                typeof(TryParseIgnoreCaseBenchmark),
                typeof(IsDefinedByteBenchmark),
                typeof(IsDefinedEnumBenchmark),
                typeof(IsDefinedStringBenchmark),
                typeof(ToStringBenchmark),
                typeof(DictionaryEnumKeyBenchmark),
                typeof(DictionaryIntKeyBenchmark),
                typeof(DictionaryStringKeyBenchmark),
                typeof(EnumMemberAttributeBenchmark),
                typeof(EnumAttributeBenchmark),
                typeof(EnumAttributesBenchmark),
            });
            switcher.Run(args, new BenchmarkConfig());
        }
    }
}
