using Xunit;

namespace Omnius.Core;

public class StringExtensionTest
{
    [Fact]
    public void ContainsTest()
    {
        var s1 = "AAABCCC";

        Assert.True(s1.Contains("aaabccc", StringComparison.OrdinalIgnoreCase));
        Assert.True(s1.Contains("abc", StringComparison.OrdinalIgnoreCase));
    }
}
