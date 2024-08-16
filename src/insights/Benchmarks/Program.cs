using BenchmarkDotNet.Running;
using FastEnumUtility.Benchmarks;
using FastEnumUtility.Benchmarks.Scenarios;



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
    typeof(ToStringDefinedBenchmark),
    typeof(ToStringUndefinedBenchmark),
    typeof(DictionaryEnumKeyBenchmark),
    typeof(DictionaryInt32KeyBenchmark),
    typeof(DictionaryStringKeyBenchmark),
    typeof(EnumMemberAttributeBenchmark),
    typeof(ForEachBenchmark),
    typeof(StringEqualsBenchmark),
    typeof(InitializeBenchmark),
});
switcher.Run(args, new BenchmarkConfig());
