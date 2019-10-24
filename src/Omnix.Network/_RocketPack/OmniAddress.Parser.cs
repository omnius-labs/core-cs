using System;
using System.Collections.Generic;
using System.Linq;
using Omnix.Collections;
using Omnix.Network.Internal.Extensions;
using Sprache;

namespace Omnix.Network
{
    public partial class OmniAddress
    {
        internal static class Parser
        {
            private static readonly Parser<string> _stringLiteralParser =
                from name in Sprache.Parse.Char(x => ('0' <= x && x <= '9') || ('A' <= x && x <= 'Z') || ('a' <= x && x <= 'z') || x == '_', "Name").AtLeastOnce().Text()
                select name;

            private static readonly Parser<string> _quotedStringLiteralParser =
                from openQuote in Sprache.Parse.Char('\'')
                from fragments in Sprache.Parse.Char('\\').Then(_ => Sprache.Parse.AnyChar.Select(c => $"\\{c}")).Or(Sprache.Parse.CharExcept("\\\'").Many().Text()).Many()
                from closeQuote in Sprache.Parse.Char('\'')
                select string.Join(string.Empty, fragments).Replace("\\\'", "\'");

            private static readonly Parser<ConstantElement> _constantElementParser =
                from element in _stringLiteralParser.Or(_quotedStringLiteralParser).TokenWithSkipSpace()
                from comma in Sprache.Parse.Char(',').Optional().TokenWithSkipSpace()
                select new ConstantElement(element);

            private static readonly Parser<FunctionElement> _functionElementParser =
                from name in _stringLiteralParser.TokenWithSkipSpace()
                from beginTag in Sprache.Parse.Char('(').TokenWithSkipSpace()
                from arguments in _functionElementParser.Or<object>(_constantElementParser).Many().TokenWithSkipSpace()
                from endTag in Sprache.Parse.Char(')').TokenWithSkipSpace()
                from comma in Sprache.Parse.Char(',').Optional().TokenWithSkipSpace()
                select new FunctionElement(name, arguments.ToArray());

            public static FunctionElement? Parse(string text)
            {
                try
                {
                    return _functionElementParser.Parse(text);
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }

                return null;
            }
        }

        public sealed class FunctionElement
        {
            public FunctionElement(string name, object[] arguments)
            {
                this.Name = name;
                this.Arguments = new ReadOnlyListSlim<object>(arguments);
            }

            public string Name { get; }
            public IReadOnlyList<object> Arguments { get; }

            public override string ToString()
            {
                return $"{this.Name}({string.Join(',', this.Arguments)})";
            }
        }

        public sealed class ConstantElement
        {
            public ConstantElement(string text)
            {
                this.Text = text;
            }

            public string Text { get; }

            public override string ToString()
            {
                return this.Text;
            }
        }
    }
}
