#if NETFRAMEWORK
#pragma warning disable IDE0130
#pragma warning disable IDE0290



namespace System
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.CompilerServices;


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
