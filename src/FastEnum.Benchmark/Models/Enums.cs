using System.Runtime.Serialization;



namespace FastEnumUtility.Benchmark.Models
{
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
}
