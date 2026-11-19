using System;
using System.Collections.Frozen;
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
public sealed class Member<T> : IMemberValue<T>, IMemberName
    where T : struct, Enum
{
    #region IMemberValue<T>
    /// <summary>
    /// Gets the value of specified enumration member.
    /// </summary>
    public T Value { get; }
    #endregion


    #region IMemberName
    /// <summary>
    /// Gets the name of specified enumration member.
    /// </summary>
    public string Name { get; }
    #endregion


    #region Properties
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
    internal FrozenDictionary<int, string?> Labels { get; }
    #endregion


    #region Constructors
    /// <summary>
    /// Creates instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <param name="field"></param>
    internal Member(string name, T value, FieldInfo field)
    {
        this.Value = value;
        this.Name = name;
        this.NameUtf8 = ImmutableCollectionsMarshal.AsImmutableArray(Encoding.UTF8.GetBytes(name));
        this.FieldInfo = field;
        this.EnumMemberAttribute = this.FieldInfo.GetCustomAttribute<EnumMemberAttribute>();
        this.Labels = JitOptimization.MemberHelper.CreateLabels(this.FieldInfo);
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



internal interface IMemberValue<T>
{
    public T Value { get; }
}



internal interface IMemberName
{
    public string Name { get; }
}
