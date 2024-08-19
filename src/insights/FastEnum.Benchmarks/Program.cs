using BenchmarkDotNet.Running;
using FastEnumUtility.Benchmarks.Internals;
using FastEnumUtility.Benchmarks.Scenarios;



//var config = BenchmarkConfig.ForDefault();
var config = BenchmarkConfig.ForShortRun();
var switcher = new BenchmarkSwitcher(
[
    typeof(Scenario_ToString_Defined),
    typeof(Scenario_ToString_Undefined),
]);
switcher.Run(args, config);
