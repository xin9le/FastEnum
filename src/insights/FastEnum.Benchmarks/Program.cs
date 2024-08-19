using System;
using BenchmarkDotNet.Running;
using FastEnumUtility.Benchmarks.Internals;



//var config = BenchmarkConfig.ForDefault();
var config = BenchmarkConfig.ForShortRun();
var switcher = new BenchmarkSwitcher(Type.EmptyTypes);
switcher.Run(args, config);
