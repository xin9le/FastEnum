using System.Linq;
using FastEnumUtility.Internals;
using FluentAssertions;
using Xunit;

namespace FastEnumUtility.Tests.Cases;



public class ReadOnlyArrayTest
{
    private const int LoopCount = 100;
    private readonly int[] SourceArray = Enumerable.Range(0, LoopCount).ToArray();


    [Fact]
    public void Count()
    {
        var roa = this.SourceArray.ToReadOnlyArray();
        roa.Count.Should().Be(this.SourceArray.Length);
    }


    [Fact]
    public void ForEach()
    {
        var sum = 0;
        foreach (var x in this.SourceArray.ToReadOnlyArray())
            sum += x;

        sum.Should().Be(this.SourceArray.Sum());
    }


    [Fact]
    public void SequenceEquals()
    {
        var roa = this.SourceArray.ToReadOnlyArray();
        roa.Should().BeEquivalentTo(this.SourceArray);
        roa.Should().ContainInOrder(this.SourceArray);
    }
}
