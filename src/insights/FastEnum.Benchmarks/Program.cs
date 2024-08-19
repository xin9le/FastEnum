using BenchmarkDotNet.Running;
using FastEnumUtility.Benchmarks.Internals;
using FastEnumUtility.Benchmarks.Scenarios;



//var config = BenchmarkConfig.ForDefault();
var config = BenchmarkConfig.ForShortRun();
var switcher = new BenchmarkSwitcher(
[
    typeof(Scenario_ToString_Defined),
]);
switcher.Run(args, config);
