using Xunit;

namespace Omnius.Core;

public class IEnumerableExtensionsTest
{
    [Fact]
    public void IEnumerableWithContextTest()
    {
        var emptyList = new List<string>();
        Assert.Equal(emptyList.WithContext().ToList(), Array.Empty<ElementWithContext<string>>());

        var len1List = new List<string>(new[] { "a" });
        Assert.Equal(len1List.WithContext().ToList(), new[] {
            new ElementWithContext<string>(null, "a", null, 0),
        });

        var len2List = new List<string>(new[] { "a", "b" });
        Assert.Equal(len2List.WithContext().ToList(), new[] {
            new ElementWithContext<string>(null, "a", "b", 0),
            new ElementWithContext<string>("a", "b", null, 1),
        });
    }
}
