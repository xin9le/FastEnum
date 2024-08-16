using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FastEnumUtility.Benchmarks.Models;
using FastEnumUtility.Internals;

namespace FastEnumUtility.Benchmarks.Scenarios;



public class InitializeBenchmark
{
    #region FastEnum
    [Benchmark(Baseline = true)]
    public void FastEnum_Init_All()
    {
        Cache<Fruits>();

        static void Cache<T>()
            where T : struct, Enum
        {
            var type = typeof(T);
            var underlyingType = Enum.GetUnderlyingType(type);
            var values = ((T[])Enum.GetValues(type)).AsReadOnly();
            var names = Enum.GetNames(type).ToReadOnlyArray();
            var members = names.Select(static x => new Member<T>(x)).ToReadOnlyArray();
            var minValue = values.DefaultIfEmpty().Min();
            var maxValue = values.DefaultIfEmpty().Max();
            var isEmpty = values.Count == 0;
            var isFlags = Attribute.IsDefined(type, typeof(FlagsAttribute));
            var distinctedMembers = members.Distinct(new Member<T>.ValueComparer()).ToArray();
            var memberByValue = distinctedMembers.ToFrozenDictionary(static x => x.Value);
            var memberByName = members.ToFrozenStringKeyDictionary(static x => x.Name);
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
        }
    }


    [Benchmark]
    public void FastEnum_Init_Type()
    {
        Cache<Fruits>();

        static void Cache<T>()
            where T : struct, Enum
        {
            var type = typeof(T);
            var underlyingType = Enum.GetUnderlyingType(type);
        }
    }


    [Benchmark]
    public void FastEnum_Init_Values()
    {
        Cache<Fruits>();

        static void Cache<T>()
            where T : struct, Enum
        {
            var type = typeof(T);
            var values = (T[])Enum.GetValues(type);
            var values2 = values.AsReadOnly();
            var isEmpty = values.Length == 0;
        }
    }


    [Benchmark]
    public void FastEnum_Init_Names()
    {
        Cache<Fruits>();

        static void Cache<T>()
            where T : struct, Enum
        {
            var type = typeof(T);
            var names = Enum.GetNames(type).ToReadOnlyArray();
        }
    }


    [Benchmark]
    public void FastEnum_Init_Members()
    {
        Cache<Fruits>();

        static void Cache<T>()
            where T : struct, Enum
        {
            var type = typeof(T);
            var names = Enum.GetNames(type).ToReadOnlyArray();
            var members
                = names
                .Select(static x => new Member<T>(x))
                .ToReadOnlyArray();
        }
    }


    [Benchmark]
    public void FastEnum_Init_MinMaxValues()
    {
        Cache<Fruits>();

        static void Cache<T>()
            where T : struct, Enum
        {
            var type = typeof(T);
            var values = (T[])Enum.GetValues(type);
            var values2 = values.AsReadOnly();
            var isEmpty = values.Length == 0;

            var minValue = values2.DefaultIfEmpty().Min();
            var maxValue = values2.DefaultIfEmpty().Max();
        }
    }


    [Benchmark]
    public void FastEnum_Init_IsFlags()
    {
        Cache<Fruits>();

        static void Cache<T>()
            where T : struct, Enum
        {
            var type = typeof(T);
            var isFlags = Attribute.IsDefined(type, typeof(FlagsAttribute));
        }
    }


    [Benchmark]
    public void FastEnum_Init_MembersByName()
    {
        Cache<Fruits>();

        static void Cache<T>()
            where T : struct, Enum
        {
            var type = typeof(T);
            var names = Enum.GetNames(type).ToReadOnlyArray();
            var members
                = names
                .Select(static x => new Member<T>(x))
                .ToReadOnlyArray();

            var memberByName = members.ToFrozenStringKeyDictionary(static x => x.Name);
        }
    }


    [Benchmark]
    public void FastEnum_Init_UnderlyingOperation()
    {
        Cache<Fruits>();

        static void Cache<T>()
            where T : struct, Enum
        {
            var type = typeof(T);
            var values = (T[])Enum.GetValues(type);
            var values2 = values.AsReadOnly();
            var isEmpty = values.Length == 0;
            var min = values2.DefaultIfEmpty().Min();
            var max = values2.DefaultIfEmpty().Max();
            var names = Enum.GetNames(type).ToReadOnlyArray();
            var members
                = names
                .Select(static x => new Member<T>(x))
                .ToReadOnlyArray();
            var distincted = members.OrderBy(static x => x.Value).Distinct(new Member<T>.ValueComparer()).ToArray();
            var underlyingOperation
                = Type.GetTypeCode(type) switch
                {
                    TypeCode.SByte => SByteOperation<T>.Create(min, max, distincted),
                    TypeCode.Byte => ByteOperation<T>.Create(min, max, distincted),
                    TypeCode.Int16 => Int16Operation<T>.Create(min, max, distincted),
                    TypeCode.UInt16 => UInt16Operation<T>.Create(min, max, distincted),
                    TypeCode.Int32 => Int32Operation<T>.Create(min, max, distincted),
                    TypeCode.UInt32 => UInt32Operation<T>.Create(min, max, distincted),
                    TypeCode.Int64 => Int64Operation<T>.Create(min, max, distincted),
                    TypeCode.UInt64 => UInt64Operation<T>.Create(min, max, distincted),
                    _ => throw new InvalidOperationException(),
                };
        }
    }
    #endregion


    #region Enum
    [Benchmark]
    public void Enum_GetValues()
        => _ = Enum.GetValues<Fruits>();


    [Benchmark]
    public void Enum_GetNames()
        => _ = Enum.GetNames<Fruits>();


    [Benchmark]
    public void Enum_GetName()
        => _ = Enum.GetName(Fruits.Lemon);


    [Benchmark]
    public void Enum_IsDefined()
        => _ = Enum.IsDefined(Fruits.Melon);


    [Benchmark]
    public void Enum_Parse()
        => _ = Enum.Parse<Fruits>("Apple");


    [Benchmark]
    public void Enum_ToString()
        => _ = Fruits.Grape.ToString();
    #endregion
}
