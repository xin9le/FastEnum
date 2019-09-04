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
            });

            args = new[] { "0" };
            switcher.Run(args, new BenchmarkConfig());
        }
    }
}
