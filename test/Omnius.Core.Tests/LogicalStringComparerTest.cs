using FluentAssertions;
using Omnius.Core.Internal;
using Xunit;

namespace Omnius.Core;

public class LogicalStringComparerTest
{
    [Fact]
    public void SimpleTest()
    {
        var list = new List<string>() {
            "a20.txt",
            "a10.txt",
            "a2.txt",
            "a1.txt",
        };

        list.Sort();
        list.Should().BeEquivalentTo(
            new[] {
                "a1.txt",
                "a10.txt",
                "a2.txt",
                "a20.txt",
            }, config => config.WithStrictOrdering());

        list.Sort(new LogicalStringComparer());
        list.Should().BeEquivalentTo(
            new[] {
                "a1.txt",
                "a2.txt",
                "a10.txt",
                "a20.txt",
            }, config => config.WithStrictOrdering());
    }
}
