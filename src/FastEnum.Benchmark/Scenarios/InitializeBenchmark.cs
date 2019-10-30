using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmark.Models;
using FastEnumUtility.Internals;



namespace FastEnumUtility.Benchmark.Scenarios
{
    public class InitializeBenchmark
    {
        [Benchmark(Baseline = true)]
        public void FastEnum_Init()
        {
            Cache<Fruits>();

            static void Cache<T>()
                where T : struct, Enum
            {
                var type = typeof(T);
                var underlyingType = Enum.GetUnderlyingType(type);
                var values = (Enum.GetValues(type) as T[]).AsReadOnly();
                var names = Enum.GetNames(type).Select(string.Intern).ToReadOnlyArray();
                var members = names.Select(x => new Member<T>(x)).ToReadOnlyArray();
                var minValue = values.DefaultIfEmpty().Min();
                var maxValue = values.DefaultIfEmpty().Max();
                var isEmpty = values.Count == 0;
                var isFlags = Attribute.IsDefined(type, typeof(FlagsAttribute));
                var memberByValue = members.Distinct(new Member<T>.ValueComparer()).ToFrozenDictionary(x => x.Value);
                var memberByName = members.ToFrozenStringKeyDictionary(x => x.Name);
                var underlyingOperation
                    = Type.GetTypeCode(type) switch
                    {
                        TypeCode.SByte => new SByteOperation<T>(minValue, maxValue) as IUnderlyingOperation<T>,
                        TypeCode.Byte => new ByteOperation<T>(minValue, maxValue),
                        TypeCode.Int16 => new Int16Operation<T>(minValue, maxValue),
                        TypeCode.UInt16 => new UInt16Operation<T>(minValue, maxValue),
                        TypeCode.Int32 => new Int32Operation<T>(minValue, maxValue),
                        TypeCode.UInt32 => new UInt32Operation<T>(minValue, maxValue),
                        TypeCode.Int64 => new Int64Operation<T>(minValue, maxValue),
                        TypeCode.UInt64 => new UInt64Operation<T>(minValue, maxValue),
                        _ => throw new InvalidOperationException(),
                    };
                var isContinuous = IsContinuousInternal();

                bool IsContinuousInternal()
                {
                    if (isEmpty)
                        return false;

                    var subtract = underlyingOperation.Subtract(maxValue, minValue);
                    var count = memberByValue.Count - 1;
                    return underlyingOperation.Equals(subtract, count);
                }
            }
        }


        [Benchmark]
        public void Enum_GetValues()
            => _ = Enum.GetValues(typeof(Fruits)) as Fruits[];


        [Benchmark]
        public void Enum_GetNames()
            => _ = Enum.GetNames(typeof(Fruits));


        [Benchmark]
        public void Enum_GetName()
            => _ = Enum.GetName(typeof(Fruits), Fruits.Lemon);


        [Benchmark]
        public void Enum_IsDefined()
            => _ = Enum.IsDefined(typeof(Fruits), Fruits.Melon);


        [Benchmark]
        public void Enum_TryParse()
            => _ = Enum.Parse(typeof(Fruits), "Apple");


        [Benchmark]
        public void Enum_ToString()
            => _ = Fruits.Grape.ToString();
    }
}
