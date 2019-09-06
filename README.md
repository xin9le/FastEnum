# FastEnum
FastEnum is **the fastest** enum utilities for C#/.NET. It's designed easier to use than `System.Enum`. And provided methods are all achieved zero allocation and faster than `System.Enum`.



# Performance

![image](https://user-images.githubusercontent.com/4776688/64415012-6acae400-d0cf-11e9-9fbb-c0af92ae503e.png)



``` ini
BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview9-014004
  [Host]     : .NET Core 3.0.0-preview9-19423-09 (CoreCLR 4.700.19.42102, CoreFX 4.700.19.42104), 64bit RyuJIT
  DefaultJob : .NET Core 3.0.0-preview9-19423-09 (CoreCLR 4.700.19.42102, CoreFX 4.700.19.42104), 64bit RyuJIT
```



# How to use

Simple to use like following, you don't confuse. The biggest limitation of this library is that it only provides a generics version, however almost cases covered.


### Get values

```cs
//--- FastEnum
var values = FastEnum<Fruits>.Values;

//--- .NET
var values = Enum.GetValues(typeof(Fruits)) as Fruits[];
```


### Get names

```cs
//--- FastEnum
var names = FastEnum<Fruits>.Names;

//--- .NET
var names = Enum.GetNames(typeof(Fruits));
```



### Convert to name

```cs
//--- FastEnum
var name = Fruits.Apple.ToName();

//--- .NET
var name = Fruits.Apple.ToString();
var name = Enum.GetName(typeof(Fruits), Fruits.Apple);
```


### Check whether value is defined

```cs
//--- FastEnum
var defined = FastEnum<Fruits>.IsDefined(Fruits.Apple);
var defined = FastEnum<Fruits>.IsDefined(123);
var defined = FastEnum<Fruits>.IsDefined("Apple");
var defined = Fruits.Apple.IsDefined();

//--- .NET
var defined = Enum.IsDefined(typeof(Fruits), Fruits.Apple);
var defined = Enum.IsDefined(typeof(Fruits), 123);
var defined = Enum.IsDefined(typeof(Fruits), "Apple");
```


### Parse / TryParse

```cs
//--- FastEnum
var value = FastEnum<Fruits>.Parse("Apple");
var value = FastEnum<Fruits>.Parse("123");

//--- .NET
var value = Enum.Parse<Fruits>("Apple");
var value = Enum.Parse<Fruits>("123");
var value = Enum.Parse(typeof(Fruits), "Apple");
var value = Enum.Parse(typeof(Fruits), "123");
```

```cs
//--- FastEnum
var ok = FastEnum<Fruits>.TryParse("Apple", out var value);
var ok = FastEnum<Fruits>.TryParse("123", out var value);

//--- .NET
var ok = Enum.TryParse<Fruits>("Apple", out var value);
var ok = Enum.TryParse<Fruits>("123", out var value);
var ok = Enum.TryParse(typeof(Fruits), "Apple", out var value);
var ok = Enum.TryParse(typeof(Fruits), "123", out var value);
```


### Get field member information

```cs
//--- FastEnum
var member = Fruits.Apple.ToMember();

//--- .NET
Not supported.
```



### Get `EnumMemberAttribute.Value`

```cs
enum Fruits
{
    [EnumMember(Value = "_apple_")]
    Apple = 0,
}

//--- FastEnum
var value = Fruits.Apple.GetEnumMemberValue();  // _apple_

//--- .NET
var type = typeof(Fruits);
var name = Enum.GetName(type, Fruits.Apple);
var info = type.GetField(name);
var attr = info.GetCustomAttribute<EnumMemberAttribute>();
var value = attr.Value;  // _apple_
```



### Get label

FastEnum provides the `LabelAttribute` that allows multiple text annotation for each enumeration member.

```cs
enum Fruits
{
    [Label("Apple, Inc.")]
    [Label("AAPL", 1)]
    Apple = 0,
}

//--- FastEnum
var label_0 = Fruits.Apple.GetLabel();   // Apple, Inc.
var label_1 = Fruits.Apple.GetLabel(1);  // AAPL

//--- .NET
Not supported.
```



# Installation

Getting started from downloading [NuGet](https://www.nuget.org/packages/FastEnum) package.

```
PM> Install-Package FastEnum
```



# License

This library is provided under [MIT License](http://opensource.org/licenses/MIT).


# Author

Takaaki Suzuki (a.k.a [@xin9le](https://twitter.com/xin9le)) is software developer in Japan who awarded Microsoft MVP for Developer Technologies (C#) since July 2012.
