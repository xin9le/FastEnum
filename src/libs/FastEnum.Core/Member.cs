using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using FastEnumUtility.Internals;

namespace FastEnumUtility;




/// <summary>
/// Represents the member information of the constant in the specified enumeration.
/// </summary>
/// <typeparam name="T">Enum type</typeparam>
public sealed class Member<T>
    where T : struct, Enum
{
    #region Properties
    /// <summary>
    /// Gets the value of specified enumration member.
    /// </summary>
    public T Value { get; }


    /// <summary>
    /// Gets the name of specified enumration member.
    /// </summary>
    public string Name { get; }


    /// <summary>
    /// Gets the name of the specified enumeration member as a UTF-8 byte array.
    /// </summary>
    public ImmutableArray<byte> NameUtf8 { get; }


    /// <summary>
    /// Gets the <see cref="System.Reflection.FieldInfo"/> of specified enumration member.
    /// </summary>
    public FieldInfo FieldInfo { get; }


    /// <summary>
    /// Gets the <see cref="System.Runtime.Serialization.EnumMemberAttribute"/> of specified enumration member.
    /// </summary>
    public EnumMemberAttribute? EnumMemberAttribute { get; }


    /// <summary>
    /// Gets the labels of specified enumration member.
    /// </summary>
    internal FastDictionary<int, string?> Labels { get; }
    #endregion


    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
    /// <param name="name"></param>
    internal Member(string name)
    {
        this.Value = Enum.Parse<T>(name);
        this.Name = name;
        this.NameUtf8 = ImmutableCollectionsMarshal.AsImmutableArray(Encoding.UTF8.GetBytes(name));
        this.FieldInfo = typeof(T).GetField(name)!;
        this.EnumMemberAttribute = this.FieldInfo.GetCustomAttribute<EnumMemberAttribute>();
        this.Labels
            = this.FieldInfo
            .GetCustomAttributes<LabelAttribute>()
            .ToFastDictionary(static x => x.Index, static x => x.Value);
    }


    /// <summary>
    /// Deconstruct into name and value.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public void Deconstruct(out string name, out T value)
    {
        name = this.Name;
        value = this.Value;
    }
    #endregion
}
