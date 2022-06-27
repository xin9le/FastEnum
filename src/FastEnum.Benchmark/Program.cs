using BenchmarkDotNet.Running;
using FastEnumUtility.Benchmark.Scenarios;

namespace FastEnumUtility.Benchmark;



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
            typeof(DictionaryInt32KeyBenchmark),
            typeof(DictionaryStringKeyBenchmark),
            typeof(EnumMemberAttributeBenchmark),
            typeof(ForEachBenchmark),
            typeof(StringEqualsBenchmark),
            typeof(InitializeBenchmark),
        });
        switcher.Run(args, new BenchmarkConfig());
    }
}
