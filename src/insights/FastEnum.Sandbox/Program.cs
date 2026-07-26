using System;
using FastEnumUtility;
using FastEnumUtility.Sandbox;


var success = FastEnum.TryParse<Fruits>("Apple", out var fruit);
Console.WriteLine(success);
Console.WriteLine(fruit);


namespace FastEnumUtility.Sandbox
{
#if NETFRAMEWORK
    public enum Fruits
    {
        Apple = 1,

        [Obsolete]
        Banana,
    }
#else
    using System.Net;


    //[Obsolete(DiagnosticId = "FE0003")]
    public enum Fruits
    {
        Apple = 1,

#pragma warning disable CA1041
        [Obsolete]
        Banana,

        [Obsolete(DiagnosticId = "FE0001")]
        Peach,

        [Obsolete(DiagnosticId = "FE0002")]
        Lemon,
#pragma warning restore CA1041
    }


    [global::FastEnumUtility.FastEnum<Fruits>]
    public partial class FruitsBooster
    { }


    [global::FastEnumUtility.FastEnum<HttpStatusCode>]
    public partial class HttpStatusCodeBooster
    { }


    [global::FastEnumUtility.FastEnum<LoaderOptimization>]
    public partial class LoaderOptimizationBooster
    { }
#endif
}
