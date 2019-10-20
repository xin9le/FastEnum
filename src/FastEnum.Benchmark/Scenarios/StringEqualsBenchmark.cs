using System;
using BenchmarkDotNet.Attributes;



namespace FastEnumUtility.Benchmark.Scenarios
{
    public class StringEqualsBenchmark
    {
        private const string Lower = "abcdefghijklmnopqrstuvwxyz";
        private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


        [GlobalSetup]
        public void Setup()
        {}


        [Benchmark(Baseline = true)]
        public bool String_OrdinalIgnoreCase()
        {
            var lower = Lower.AsSpan();
            var upper = Upper.AsSpan();
            return Lower.Equals(Upper, StringComparison.OrdinalIgnoreCase);
        }


        [Benchmark]
        public bool Span_OrdinalIgnoreCase()
        {
            var lower = Lower.AsSpan();
            var upper = Upper.AsSpan();
            return lower.Equals(upper, StringComparison.OrdinalIgnoreCase);
        }


        [Benchmark]
        public bool String_InvariantCultureIgnoreCase()
        {
            var lower = Lower.AsSpan();
            var upper = Upper.AsSpan();
            return Lower.Equals(Upper, StringComparison.InvariantCultureIgnoreCase);
        }


        [Benchmark]
        public bool Span_InvariantCultureIgnoreCase()
        {
            var lower = Lower.AsSpan();
            var upper = Upper.AsSpan();
            return lower.Equals(upper, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
