using BenchmarkDotNet.Running;
using FastEnumUtility.Benchmarks.Internals;
using FastEnumUtility.Benchmarks.Scenarios;



// note:
//  - FASTENUM_BENCH_BASELINE = the 'src/libs' directory of another tree compares it with this one in one run; see BenchmarkConfig.ForComparison().
var baselineLibs = System.Environment.GetEnvironmentVariable("FASTENUM_BENCH_BASELINE");
var config = string.IsNullOrEmpty(baselineLibs)
    ? BenchmarkConfig.ForDefault()
    : BenchmarkConfig.ForComparison(baselineLibs);
//var config = BenchmarkConfig.ForShortRun();
var switcher = new BenchmarkSwitcher(
[
    typeof(EnumMemberAttribute),
    typeof(EqualsIgnoreCaseByLength),
    typeof(GetName),
    typeof(GetNames),
    typeof(GetValues),
    typeof(IsDefined_Enum),
    typeof(IsDefined_String),
    typeof(NonConstantInput),
    typeof(ToString_Defined),
    typeof(ToString_Undefined),
    typeof(TryParse_CaseInsensitive),
    typeof(TryParse_CaseSensitive),
]);
switcher.Run(args, config);
