using System.ComponentModel;
using System.Runtime.Serialization;



namespace FastEnum.Benchmark.Models
{
    public enum Fruits : byte
    {
        Unknown = 0,

        [EnumMember(Value = "🍎"), Description("It is a rose family")]
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
