namespace FastEnumUtility.UnitTests.Models;



[FastEnum<SByteEnum>]
public sealed partial class SByteEnumBooster
{ }



[FastEnum<ByteEnum>]
public sealed partial record class ByteEnumBooster
{ }



[FastEnum<SameValueEnum>]
public readonly partial struct SameValueEnumBooster
{ }



[FastEnum<EmptyEnum>]
public readonly partial record struct EmptyEnumBooster
{ }
