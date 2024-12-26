namespace FastEnumUtility.UnitTests.Models;



[FastEnum<SByteEnum>]
public sealed partial class SByteEnumBooster
{ }



[FastEnum<ByteEnum>]
public sealed partial class ByteEnumBooster
{ }



[FastEnum<Int16Enum>]
public sealed partial class Int16EnumBooster
{ }



[FastEnum<UInt16Enum>]
public sealed partial class UInt16EnumBooster
{ }



[FastEnum<Int32Enum>]
public sealed partial class Int32EnumBooster
{ }



[FastEnum<UInt32Enum>]
public sealed partial class UInt32EnumBooster
{ }



[FastEnum<Int64Enum>]
public sealed partial class Int64EnumBooster
{ }



[FastEnum<UInt64Enum>]
public sealed partial class UInt64EnumBooster
{ }



[FastEnumUtility.FastEnum<SameValueContinuousEnum>]  // full qualified name syntax
public sealed partial class SameValueContinuousEnumBooster
{ }



[global::FastEnumUtility.FastEnum<SameValueDiscontinuousEnum>]  // full qualified name syntax
public sealed partial class SameValueDiscontinuousEnumBooster
{ }



[FastEnum<EmptyEnum>]
public sealed partial class EmptyEnumBooster
{ }



[FastEnum<CaseInsensitiveEnum>]
public sealed partial class CaseInsensitiveEnumBooster
{ }
