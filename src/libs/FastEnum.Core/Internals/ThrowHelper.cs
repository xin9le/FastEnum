using System;
using System.Diagnostics.CodeAnalysis;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace FastEnumUtility.Internals;



internal static class ThrowHelper
{
    public static void ThrowIfUnderlyingTypeMismatch<TEnum, TUnderlying>(string? paramName)
        where TEnum : struct, Enum
#if NET7_0_OR_GREATER
        where TUnderlying : INumberBase<TUnderlying>
#endif
    {
        if (FastEnum.GetUnderlyingType<TEnum>() == typeof(TUnderlying))
            return;

        const string message = $"The underlying type of the enum and the value must be the same type.";
        throw new ArgumentException(message, paramName);
    }


    [DoesNotReturn]
    public static void ThrowValueNotDefined(ReadOnlySpan<char> value, string? paramName)
    {
        var message = $"Specified value '{value}' is not defined.";
        throw new ArgumentException(message, paramName);
    }


    [DoesNotReturn]
    public static void ThrowLabelNotFound(int index)
    {
        var message = $"{nameof(LabelAttribute)} that is specified index {index} is not found.";
        throw new NotFoundException(message);
    }


    [DoesNotReturn]
    public static void ThrowValueNotDefined<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        var message = $"Specified value '{value}' is not defined.";
        throw new NotFoundException(message);
    }


    [DoesNotReturn]
    public static void ThrowAttributeNotDefined<TEnum, TAttribute>(TEnum value)
        where TEnum : struct, Enum
        where TAttribute : Attribute
    {
        var message = $"The {typeof(TAttribute).FullName} is not defined for the specified value '{value}'.";
        throw new NotFoundException(message);
    }


    [DoesNotReturn]
    public static void ThrowUnexpectedCodeReached()
    {
        const string message = "Unexpected code execution detected.";
        throw new InvalidOperationException(message);
    }


    [DoesNotReturn]
    public static void ThrowDuplicatedKeyExists<T>(T key)
    {
        var message = $"Key '{key}' was already exists.";
        throw new ArgumentException(message);
    }
}
