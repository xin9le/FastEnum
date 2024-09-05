using System;
using FastEnumUtility;
using FastEnumUtility.Sandbox;


var success = FastEnum.TryParse<Fruits>("Apple", out var fruit);
Console.WriteLine(success);
Console.WriteLine(fruit);


namespace FastEnumUtility.Sandbox
{
    public enum Fruits
    {
        Apple = 1,
        Banana,
    }


    [FastEnum<Fruits>]
    public partial class FruitsBooster
    { }
}

