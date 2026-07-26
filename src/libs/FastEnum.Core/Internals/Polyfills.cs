#if NETFRAMEWORK
#pragma warning disable IDE0130
#pragma warning disable IDE0290



namespace System
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;
    using FastEnumUtility.Internals;



    internal static class ArgumentNullExceptionExtensions
    {
        extension(ArgumentNullException)
        {
            public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
            {
                if (argument is null)
                    throw new ArgumentNullException(paramName);
            }
        }
    }



    internal static class EnumExtensions
    {
        extension(Enum)
        {
            public static T Parse<T>(string name)
                => (T)Enum.Parse(typeof(T), name);
        }
    }



    internal static class StringExtensions
    {
        extension(string? @this)
        {
            public bool IsEmpty
                => @this.AsSpan().Length is 0;


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ref char At(int index)
            {
                var span = @this.AsSpan();
                return ref CollectionExtensions.At(span, index);
            }
        }
    }
}



namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
    internal sealed class NotNullAttribute : Attribute
    { }



    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class NotNullWhenAttribute : Attribute
    {
        public NotNullWhenAttribute(bool returnValue)
            => this.ReturnValue = returnValue;


        public bool ReturnValue { get; }
    }



    [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
    internal sealed class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool returnValue)
            => this.ReturnValue = returnValue;


        public bool ReturnValue { get; }
    }



    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    internal sealed class DoesNotReturnAttribute : Attribute
    { }
}



namespace System.Linq
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;



    internal static class EnumerableExtensions
    {
        extension<TSource>(IEnumerable<TSource> @this)
        {
            public IEnumerable<TSource> DistinctBy<TKey>(Func<TSource, TKey> keySelector)
                => DistinctBy(@this, keySelector, null);


            public IEnumerable<TSource> DistinctBy<TKey>(Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
            {
                return isEmptyArray(@this)
                    ? []
                    : distinctByIterator(@this, keySelector, comparer);


                #region Local Functions
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                static bool isEmptyArray(IEnumerable<TSource> self)
                    => self is TSource[] { Length: 0 };


                static IEnumerable<TSource> distinctByIterator(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? comparer)
                {
                    using (var enumerator = source.GetEnumerator())
                    {
                        if (enumerator.MoveNext())
                        {
                            var set = new HashSet<TKey>(capacity: 7, comparer);
                            do
                            {
                                var element = enumerator.Current;
                                if (set.Add(keySelector(element)))
                                    yield return element;
                            }
                            while (enumerator.MoveNext());
                        }
                    }
                }
                #endregion
            }


            public bool TryGetNonEnumeratedCount(out int count)
            {
                if (@this is ICollection<TSource> x1)
                {
                    count = x1.Count;
                    return true;
                }
                if (@this is ICollection x2)
                {
                    count = x2.Count;
                    return true;
                }
                if (@this is IReadOnlyCollection<TSource> x3)
                {
                    count = x3.Count;
                    return true;
                }
                count = 0;
                return false;
            }
        }
    }
}



namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    internal sealed class CallerArgumentExpressionAttribute : Attribute
    {
        public CallerArgumentExpressionAttribute(string parameterName)
            => this.ParameterName = parameterName;


        public string ParameterName { get; }
    }



    internal static class UnsafeExtensions
    {
        extension(Unsafe)
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static TTo BitCast<TFrom, TTo>(TFrom source)
            {
                ref var @byte = ref Unsafe.As<TFrom, byte>(ref source);
                return Unsafe.ReadUnaligned<TTo>(ref @byte);
            }
        }
    }
}



namespace System.Runtime.InteropServices
{
    internal static class MemoryMarshalExtensions
    {
        extension(MemoryMarshal)
        {
            public static ref T GetArrayDataReference<T>(T[] array)
                => ref MemoryMarshal.GetReference(array);
        }
    }
}
#endif
