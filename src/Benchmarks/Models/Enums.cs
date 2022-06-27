using System.Runtime.Serialization;

namespace FastEnumUtility.Benchmarks.Models;



public enum Fruits : byte
{
    Unknown = 0,

    [EnumMember(Value = "🍎")]
    Apple,
    Banana,
    Peach,
    Orange,
    Grape,
    Lemon,
    Melon,
    Strawberry,
    Cherry,
    WaterMelon,
    Pear,
    Pineapple,
}
