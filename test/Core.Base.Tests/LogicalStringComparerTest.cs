using Shouldly;
using Omnius.Core.Base.Internal;
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
        list.ShouldBe(
            new[] {
                "a1.txt",
                "a10.txt",
                "a2.txt",
                "a20.txt",
            });

        list.Sort(new LogicalStringComparer());
        list.ShouldBe(
            new[] {
                "a1.txt",
                "a2.txt",
                "a10.txt",
                "a20.txt",
            });
    }
}
