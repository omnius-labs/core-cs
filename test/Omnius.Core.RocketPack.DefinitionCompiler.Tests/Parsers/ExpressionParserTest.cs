using System.Linq;
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
                from value in ExpressionParser.GetParser().End()
                select value.Compile().Invoke();

            // long
            Assert.Equal((long)0, p.Parse("0"));

            // double
            Assert.Equal((double)0.1, p.Parse("0.1"));

            // bool
            Assert.Equal(true, p.Parse("true"));
            Assert.Equal(false, p.Parse("false"));

            // string
            Assert.Equal("test", p.Parse("\"test\""));
        }

        [Fact]
        public void ComputeLongTest()
        {
            var p =
                from value in ExpressionParser.GetParser().End()
                select value.Compile().Invoke();

            Assert.Equal((long)3, p.Parse("2 + 1"));
            Assert.Equal((long)1000, p.Parse("400 + 600"));

            Assert.Equal((long)0, p.Parse("2 - 2"));
            Assert.Equal((long)500, p.Parse("1000 - 500"));

            Assert.Equal((long)4, p.Parse("2 * 2"));
            Assert.Equal((long)400, p.Parse("200 * 2"));

            Assert.Equal((long)2, p.Parse("2 / 1"));
            Assert.Equal((long)40, p.Parse("200 / 5"));

            Assert.Equal((long)1, p.Parse("1 % 2"));
            Assert.Equal((long)50, p.Parse("50 % 100"));
        }

        [Fact]
        public void ComputeDoubleTest()
        {
            var p =
                from value in ExpressionParser.GetParser().End()
                select value.Compile().Invoke();

            Assert.Equal((double)0.2 + 0.1, p.Parse("0.2 + 0.1"));
            Assert.Equal((double)0.4 + 0.6, p.Parse("0.4 + 0.6"));

            Assert.Equal((double)0.2 - 0.2, p.Parse("0.2 - 0.2"));
            Assert.Equal((double)0.1 - 0.5, p.Parse("0.1 - 0.5"));

            Assert.Equal((double)0.2 * 0.2, p.Parse("0.2 * 0.2"));
            Assert.Equal((double)2.2 * 2.2, p.Parse("2.2 * 2.2"));

            Assert.Equal((double)0.2 / 0.1, p.Parse("0.2 / 0.1"));
            Assert.Equal((double)0.2 / 1.5, p.Parse("0.2 / 1.5"));

            Assert.Equal((double)0.1 % 0.2, p.Parse("0.1 % 0.2"));
            Assert.Equal((double)0.1 % 0.5, p.Parse("0.1 % 0.5"));

            // long + double
            Assert.Equal((long)2 + (double)1.5, p.Parse("2 + 1.5"));
        }
    }
}
