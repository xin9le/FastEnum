using System;
using FastEnumUtility;


var a = FastEnum.TryParse<Fruits>("Apple", out var fruit);
Console.WriteLine(a);
Console.WriteLine(fruit);


[FastEnum]
public enum Fruits
{
    Apple = 1,
    Banana,
}


[FastEnum<Fruits>]
public partial class FruitsOperation
{

}
