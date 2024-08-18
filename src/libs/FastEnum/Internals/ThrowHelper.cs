using System;
using System.Numerics;

namespace FastEnumUtility.Internals;



internal static class ThrowHelper
{
    public static void ThrowIfUnderlyingTypeMismatch<TEnum, TUnderlying>(string? paramName)
        where TEnum : struct, Enum
        where TUnderlying : INumberBase<TUnderlying>
    {
        if (FastEnum.GetUnderlyingType<TEnum>() == typeof(TUnderlying))
            return;

        const string message = $"The underlying type of the enum and the value must be the same type.";
        throw new ArgumentException(message, paramName);
    }
}
