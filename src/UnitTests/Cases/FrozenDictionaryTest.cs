using System;
using System.Linq;
using FastEnumUtility.Internals;
using FluentAssertions;
using Xunit;

namespace FastEnumUtility.UnitTests.Cases;



public class FrozenDictionaryTest
{
    private const int Count = 100;


    [Fact]
    public void GenericsKey()
    {
        var values = Enumerable.Range(0, Count);
        var dic = values.ToFrozenDictionary(static x => x);

        dic.Count.Should().Be(Count);
        dic.TryGetValue(-1, out _).Should().BeFalse();
        foreach (var x in values)
        {
            dic.TryGetValue(x, out var result).Should().BeTrue();
            result.Should().Be(x);
        }
    }


    [Fact]
    public void IntKey()
    {
        var values = Enumerable.Range(0, Count);
        var dic = values.ToFrozenInt32KeyDictionary(static x => x);

        dic.Count.Should().Be(Count);
        dic.TryGetValue(-1, out _).Should().BeFalse();
        foreach (var x in values)
        {
            dic.TryGetValue(x, out var result).Should().BeTrue();
            result.Should().Be(x);
        }
    }


    [Fact]
    public void StringKey()
    {
        var values
            = Enumerable.Range(0, Count)
            .Select(static _ => Guid.NewGuid().ToString())
            .ToArray();
        var dic = values.ToFrozenStringKeyDictionary(static x => x);

        dic.Count.Should().Be(Count);
        dic.TryGetValue(string.Empty, out _).Should().BeFalse();
        foreach (var x in values)
        {
            dic.TryGetValue(x, out var result).Should().BeTrue();
            result.Should().Be(x);
        }
    }
}
