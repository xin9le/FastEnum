using BenchmarkDotNet.Running;
using FastEnumUtility.Benchmarks.Internals;
using FastEnumUtility.Benchmarks.Scenarios;



//var config = BenchmarkConfig.ForDefault();
var config = BenchmarkConfig.ForShortRun();
var switcher = new BenchmarkSwitcher(
[
    typeof(EnumMemberAttribute),
    typeof(GetName),
    typeof(GetNames),
    typeof(GetValues),
    typeof(IsDefined_Enum),
    typeof(IsDefined_String),
    typeof(ToString_Defined),
    typeof(ToString_Undefined),
    typeof(TryParse_Byte_CaseSensitive),
    typeof(TryParse_Char_CaseInsensitive),
    typeof(TryParse_Char_CaseSensitive),
]);
switcher.Run(args, config);
