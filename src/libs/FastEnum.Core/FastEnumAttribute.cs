using System;

namespace FastEnumUtility;



/// <summary>
/// A marker attribute utilized to automatically generate high-performance enum utility implementations using Source Generator.
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class FastEnumAttribute : Attribute
{ }



/// <summary>
/// A marker attribute utilized to automatically generate high-performance enum utility implementations using Source Generator.
/// </summary>
/// <typeparam name="T"><see cref="Enum"/> type</typeparam>
[AttributeUsage(AttributeTargets.Class)]
public sealed class FastEnumAttribute<T> : Attribute
    where T : struct, Enum
{ }
