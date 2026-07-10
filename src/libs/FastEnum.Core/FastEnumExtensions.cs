using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using FastEnumUtility.Internals;

namespace FastEnumUtility;



/// <summary>
/// Provides extension methods for FastEnum.
/// </summary>
public static class FastEnumExtensions
{
    /// <summary>
    /// Provides <see cref="Enum"/> extension methods.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="this"></param>
    extension<T>(T @this)
        where T : struct, Enum
    {
        #region ToMember
        /// <summary>
        /// Converts to the member information of the constant in the specified enumeration value.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Member<T>? ToMember()
            => FastEnum.GetMember(@this);


        /// <summary>
        /// Converts to the name of the constant in the specified enumeration value.
        /// </summary>
        /// <returns>A string containing the name of the enumerated constant in enum type whose value is value; or null if no such constant is found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? ToName()
            => FastEnum.GetName(@this);
        #endregion


        #region ToString
        /// <summary>
        /// Converts the specified value to its equivalent string representation.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string FastToString()
            => FastEnum.ToString(@this);
        #endregion


        #region ToNumber
        /// <summary>
        /// Converts to the 8-bit signed integer.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ToSByte()
        {
            ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, sbyte>(nameof(@this));
            return Unsafe.BitCast<T, sbyte>(@this);
        }


        /// <summary>
        /// Converts to the 8-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ToByte()
        {
            ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, byte>(nameof(@this));
            return Unsafe.BitCast<T, byte>(@this);
        }


        /// <summary>
        /// Converts to the 16-bit signed integer.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ToInt16()
        {
            ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, short>(nameof(@this));
            return Unsafe.BitCast<T, short>(@this);
        }


        /// <summary>
        /// Converts to the 16-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ToUInt16()
        {
            ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, ushort>(nameof(@this));
            return Unsafe.BitCast<T, ushort>(@this);
        }


        /// <summary>
        /// Converts to the 32-bit signed integer.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ToInt32()
        {
            ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, int>(nameof(@this));
            return Unsafe.BitCast<T, int>(@this);
        }


        /// <summary>
        /// Converts to the 32-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToUInt32()
        {
            ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, uint>(nameof(@this));
            return Unsafe.BitCast<T, uint>(@this);
        }


        /// <summary>
        /// Converts to the 64-bit signed integer.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ToInt64()
        {
            ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, long>(nameof(@this));
            return Unsafe.BitCast<T, long>(@this);
        }


        /// <summary>
        /// Converts to the 64-bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ToUInt64()
        {
            ThrowHelper.ThrowIfUnderlyingTypeMismatch<T, ulong>(nameof(@this));
            return Unsafe.BitCast<T, ulong>(@this);
        }
        #endregion


        #region Condition
        /// <summary>
        /// Returns an indication whether a constant with a specified value exists in a specified enumeration.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDefined()
            => FastEnum.IsDefined(@this);
        #endregion


        #region Attribute
        /// <summary>
        /// Gets the <see cref="EnumMemberAttribute.Value"/> of specified enumration member.
        /// </summary>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        /// <exception cref="NotFoundException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? GetEnumMemberValue(bool throwIfNotFound = true)
        {
            var member = @this.ToMember();
            if (throwIfNotFound)
            {
                if (member is null)
                    ThrowHelper.ThrowValueNotDefined(@this);

                var attr = member.EnumMemberAttribute;
                if (attr is null)
                    ThrowHelper.ThrowAttributeNotDefined<T, EnumMemberAttribute>(@this);

                return attr.Value;
            }
            else
            {
                return member?.EnumMemberAttribute?.Value;
            }
        }


        /// <summary>
        /// Gets the <see cref="LabelAttribute.Value"/> of specified enumration member.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? GetLabel(int index = 0, bool throwIfNotFound = true)
        {
            var member = @this.ToMember();
            if (throwIfNotFound && member is null)
                ThrowHelper.ThrowValueNotDefined(@this);
            return member?.GetLabel(index, throwIfNotFound);
        }
        #endregion
    }



    /// <summary>
    /// Provides <see cref="Member{T}"/> extension methods.
    /// </summary>
    /// <typeparam name="T"><see cref="Enum"/> type</typeparam>
    /// <param name="this"></param>
    extension<T>(Member<T> @this)
        where T : struct, Enum
    {
        #region Attribute
        /// <summary>
        /// Gets the <see cref="LabelAttribute.Value"/> of specified enumration member.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="throwIfNotFound"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotFoundException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string? GetLabel(int index = 0, bool throwIfNotFound = true)
        {
            ArgumentNullException.ThrowIfNull(@this);

            if (@this.Labels.TryGetValue(index, out var label))
                return label;

            if (throwIfNotFound)
                ThrowHelper.ThrowLabelNotFound(index);

            return null;
        }
        #endregion
    }
}
