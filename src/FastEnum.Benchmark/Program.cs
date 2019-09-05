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
                typeof(ValuesBenchmark),
                typeof(NamesBenchmark),
                typeof(TryParseBenchmark),
                typeof(TryParseIgnoreCaseBenchmark),
                typeof(IsDefinedValueBenchmark),
                typeof(IsDefinedNameBenchmark),
                typeof(ToStringBenchmark),
                typeof(DictionaryEnumKeyBenchmark),
                typeof(DictionaryStringKeyBenchmark),
                typeof(EnumMemberAttributeBenchmark),
            });
            switcher.Run(args, new BenchmarkConfig());
        }
    }
}
