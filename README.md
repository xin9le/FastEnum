# FastEnum
FastEnum is **extremely fast** enum utilities for C#/.NET. It's much faster than .NET. Provided methods are all achieved **zero allocation** and are designed easy to use like `System.Enum`. This library is quite useful to significantly improve your performance because enum is really popular feature.

[![Releases](https://img.shields.io/github/release/xin9le/FastEnum.svg)](https://github.com/xin9le/FastEnum/releases)



# Performance

![Benchmark](https://github.com/user-attachments/assets/134afde8-93d0-4c10-8a80-a999ff31b7da)


``` ini
BenchmarkDotNet v0.14.0, Windows 11 (10.0.22621.4037/22H2/2022Update/SunValley2)
13th Gen Intel Core i7-1360P, 1 CPU, 16 logical and 12 physical cores
.NET SDK 8.0.400
  [Host]     : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
  Job-CYQAVK : .NET 8.0.8 (8.0.824.36612), X64 RyuJIT AVX2
```

> [!NOTE]
> At present, FastEnum doesn't generate source code at compile time. We understand that leveraging Roslyn's Source Generator functionality could further accelerate performance, and thus our current implementation is not theoretically the "fastest" possible. We are positively considering the introduction of Source Generator in the future. However, as we aim to maintain a library that is both user-friendly and easily comprehensible, we may need to wait for future enhancements in C#'s expressiveness. We appreciate your understanding in this matter.



# Support Platform
- .NET 8.0+

> [!Important]
> For those who wish to use this on older platforms, please use v1.8.0. It supports follows.
> - .NET Framework 4.6.1+
> - .NET Standard 2.0+
> - .NET 5+



# How to use

This library super easy to use like `System.Enum` that is standard of .NET. Look below:

```cs
//--- FastEnum
var values = FastEnum.GetValues<Fruits>();
var names = FastEnum.GetNames<Fruits>();
var name = FastEnum.GetName<Fruits>(Fruits.Apple);
var toString = Fruits.Apple.FastToString();
var defined = FastEnum.IsDefined<Fruits>(123);
var parse = FastEnum.Parse<Fruits>("Apple");
var tryParse = FastEnum.TryParse<Fruits>("Apple", out var value);
```

```cs
//--- .NET
var values = Enum.GetValues(typeof(Fruits)) as Fruits[];
var names = Enum.GetNames(typeof(Fruits));
var name = Enum.GetName(typeof(Fruits), Fruits.Apple);
var toString = Fruits.Apple.ToString();
var defined = Enum.IsDefined(typeof(Fruits), 123);
var parse = Enum.Parse<Fruits>("Apple");
var tryParse = Enum.TryParse<Fruits>("Apple", out var value);
```

As you can see, the replacement from `System.Enum` is very easy. You never confuse.



# More features

There are some functions that are often used for enum, and you can be used more conveniently by including them together.


## 1. Gets pairwised member information

Sometimes you want name / value pair of enum. `Member<TEnum>` can be used under such cases. Of course supports [deconstruction](https://docs.microsoft.com/en-us/dotnet/csharp/deconstruct) feature. `FieldInfo` is also included, so please use it for reflection code.


```cs
class Member<TEnum>
{
    public TEnum Value { get; }
    public string Name { get; }
    public FieldInfo FieldInfo { get; }
    // etc...
}

var member = Fruits.Apple.ToMember()!;
var (name, value) = member;  // Supports deconstruction
```


## 2. Gets `EnumMemberAttribute.Value`

I often see the developer using `EnumMemberAttribute` as an alias for field name. So FastEnum provides an API that the value can be quickly obtained from the `EnumMemberAttribute.Value` property.


```cs
enum Company
{
    [EnumMember(Value = "Apple, Inc.")]
    Apple = 0,
}

var value = Company.Apple.GetEnumMemberValue();  // Apple, Inc.
```



## 3. Adds multiple label annotations to a field

Multiple attributes can’t be attached to the same field, since `EnumMemberAttribute` is specified `AllowMultiple = false`. It’s inconvenient and I don’t like it personally, so I often use my own `LabelAttribute` as an alternative. You can use it conveniently as follows, because FastEnum provides this feature.


```cs
enum Company
{
    [Label("Apple, Inc.")]
    [Label("AAPL", 1)]
    Apple = 0,
}

var x1 = Company.Apple.GetLabel();   // Apple, Inc.
var x2 = Company.Apple.GetLabel(1);  // AAPL
```


# Limitation

## 1. Provides only generics API
FastEnum provides only generics version method because of performance reason. `System.Enum` provides `System.Type` argument overload, but that’s too slow because of boxing occuration. If you need to use the method that passes `System.Type` type, please use `System.Enum` version.



## 2. Can’t parse comma-separated string
`System.Enum.Parse` can parse like following string. I think that it isn’t well known because it is a specification that exists quietly.


```cs
//--- Assuming there is an enum type like following...
[Flags]
enum Fruits
{
    Apple = 1,
    Lemon = 2,
    Melon = 4,
    Banana = 8,
}

//--- Passes comma-separated string
var value = Enum.Parse<Fruits>("Apple, Melon");
Console.WriteLine((int)value);  // 5
```

It seems to be a useful function when performing flag processing, but if tries to add such a comma-separated analysis, the overhead will come out, so cutting this feature off makes speed up. I think that in most cases there is no problem, because this feature is rarely used (at least I have NEVER used for 12 years).



# Why fast ?

As you might expect, it’s because cached internally. It takes the approach of **Static Type Caching**, so the reading cost is **almost zero**. Based on this, I use techniques for avoiding allocation, and create specialized dictionary for specific key internally.




# Installation

Getting started from downloading [NuGet](https://www.nuget.org/packages/FastEnum) package.

```
dotnet add package FastEnum
```



# License

This library is provided under [MIT License](http://opensource.org/licenses/MIT).


# Author

Takaaki Suzuki (a.k.a [@xin9le](https://twitter.com/xin9le)) is software developer in Japan who awarded Microsoft MVP for Developer Technologies (C#) since July 2012.
