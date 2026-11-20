using System;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
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
    public ImmutableArray<byte> NameUtf8
    {
        get
        {
            // note:
            //  - It is rarely used, so it is created at the first access instead of the constructor.
            //  - ImmutableArray<T> compares the underlying arrays by reference, so the same array must always be returned.
            //  - Interlocked.CompareExchange() publishes only one array even if multiple threads create it at the same time.

            var bytes = this._nameUtf8;
            if (bytes is null)
            {
                var created = Encoding.UTF8.GetBytes(this.Name);
                bytes = Interlocked.CompareExchange(ref this._nameUtf8, created, null) ?? created;
            }
            return ImmutableCollectionsMarshal.AsImmutableArray(bytes);
        }
    }
    private byte[]? _nameUtf8;


    /// <summary>
    /// Gets the <see cref="System.Reflection.FieldInfo"/> of specified enumration member.
    /// </summary>
    public FieldInfo FieldInfo { get; }


    /// <summary>
    /// Gets the <see cref="System.Runtime.Serialization.EnumMemberAttribute"/> of specified enumration member.
    /// </summary>
    public EnumMemberAttribute? EnumMemberAttribute
    {
        get
        {
            // note:
            //  - It is rarely used, so it is fetched at the first access instead of the constructor.
            //  - null is a valid result (the attribute is not defined), so the field starts with a sentinel that means "not fetched yet".
            //    A sentinel instead of a bool flag keeps the size of Member<T> unchanged.
            //  - GetCustomAttribute() creates a new instance on every call, so Interlocked.CompareExchange() publishes only one result.

            var notFetched = JitOptimization.MemberHelper.s_notFetchedEnumMemberAttribute;
            var attr = this._enumMemberAttribute;
            if (attr == notFetched)
            {
                var created = this.FieldInfo.GetCustomAttribute<EnumMemberAttribute>();
                var original = Interlocked.CompareExchange(ref this._enumMemberAttribute, created, notFetched);
                attr = (original == notFetched) ? created : original;
            }
            return attr;
        }
    }
    private EnumMemberAttribute? _enumMemberAttribute;


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
        this.FieldInfo = field;
        this._enumMemberAttribute = JitOptimization.MemberHelper.NotFetchedEnumMemberAttribute;
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
