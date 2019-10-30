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
                var distinctedMembers = members.Distinct(new Member<T>.ValueComparer()).ToArray();
                var memberByValue = distinctedMembers.ToFrozenDictionary(x => x.Value);
                var memberByName = members.ToFrozenStringKeyDictionary(x => x.Name);
                var underlyingOperation
                    = Type.GetTypeCode(type) switch
                    {
                        TypeCode.SByte => SByteOperation<T>.Create(minValue, maxValue, distinctedMembers),
                        TypeCode.Byte => ByteOperation<T>.Create(minValue, maxValue, distinctedMembers),
                        TypeCode.Int16 => Int16Operation<T>.Create(minValue, maxValue, distinctedMembers),
                        TypeCode.UInt16 => UInt16Operation<T>.Create(minValue, maxValue, distinctedMembers),
                        TypeCode.Int32 => Int32Operation<T>.Create(minValue, maxValue, distinctedMembers),
                        TypeCode.UInt32 => UInt32Operation<T>.Create(minValue, maxValue, distinctedMembers),
                        TypeCode.Int64 => Int64Operation<T>.Create(minValue, maxValue, distinctedMembers),
                        TypeCode.UInt64 => UInt64Operation<T>.Create(minValue, maxValue, distinctedMembers),
                        _ => throw new InvalidOperationException(),
                    };
                var isContinuous = underlyingOperation.IsContinuous;
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
