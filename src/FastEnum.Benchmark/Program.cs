using BenchmarkDotNet.Running;
using FastEnumUtility.Benchmark.Configurations;
using FastEnumUtility.Benchmark.Scenarios;



namespace FastEnumUtility.Benchmark
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
            });
            switcher.Run(args, new BenchmarkConfig());
        }
    }
}
