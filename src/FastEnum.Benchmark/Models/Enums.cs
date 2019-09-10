using System;
using System.ComponentModel;
using System.Runtime.Serialization;



namespace FastEnum.Benchmark.Models
{
    public enum Fruits : byte
    {
        Unknown = 0,

        [EnumMember(Value = "🍎"), Description("It is a rose family"), Color("Red"), Color("green")]
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

    /// <summary>
    /// Sample AllowMultiple Attribute 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class ColorAttribute : Attribute
    {
        public string Color { get; }

        public ColorAttribute(string Color)
        {
            this.Color = Color;
        }
    }
}
