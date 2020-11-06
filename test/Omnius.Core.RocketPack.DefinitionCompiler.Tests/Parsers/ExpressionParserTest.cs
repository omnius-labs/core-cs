using System;
using System.Linq;
using System.Linq.Expressions;
using Sprache;
using Xunit;

namespace Omnius.Core.RocketPack.DefinitionCompiler.Parsers
{
    public class ExpressionParserTest
    {
        [Fact]
        public void NormalTest()
        {
            var p =
                from value in ExpressionParser.GetParser()
                select value.Compile().Invoke();

            Assert.Equal((long)0, p.Parse("0"));
            Assert.Equal((long)1, p.Parse("0 + 1"));
            Assert.Equal((long)2, p.Parse("2 / 1"));
            Assert.Equal((long)0.5, p.Parse("1 / 2"));
            Assert.Equal((long)4, p.Parse("2 * 2"));
            Assert.Equal((long)1, p.Parse("1 % 2"));
            Assert.Equal((long)1, p.Parse("2 ^ 2"));
            Assert.Equal((double)0.5, p.Parse("0.5"));
            Assert.Equal(true, p.Parse("true"));
            Assert.Equal(false, p.Parse("false"));
            Assert.Equal("test", p.Parse("\"test\""));
        }
    }
}
