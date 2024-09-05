using System;

namespace FastEnumUtility;



/// <summary>
/// A marker attribute utilized to automatically generate high-performance enum utility implementations using Source Generator.
/// </summary>
/// <typeparam name="T"><see cref="Enum"/> type</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class FastEnumAttribute<T> : Attribute
    where T : struct, Enum
{ }
