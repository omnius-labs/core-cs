using System;
using System.Collections;
using Omnix.Base.Extensions;
using Xunit;

namespace Omnix.Base.Extensions
{
    public class StringExtensionTests
    {
        [Fact]
        public void ContainsTest()
        {
            var s1 = "AAABCCC";

            Assert.True(s1.Contains("aaabccc", StringComparison.OrdinalIgnoreCase));
            Assert.True(s1.Contains("abc", StringComparison.OrdinalIgnoreCase));
        }
    }
}
