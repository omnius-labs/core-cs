using System;
using System.Linq;
using System.Linq.Expressions;
using Omnius.Core.RocketPack.DefinitionCompiler.Parsers.Extensions;
using Sprache;

namespace Omnius.Core.RocketPack.DefinitionCompiler.Parsers
{
    // https://github.com/sprache/Sprache/blob/develop/samples/LinqyCalculator/ExpressionParser.cs
    internal static class ExpressionParser
    {
        private static Parser<ExpressionType> Operator(string op, ExpressionType opType) => Parse.String(op).TokenWithSkipComment().Return(opType);

        private static readonly Parser<ExpressionType> _add = Operator("+", ExpressionType.AddChecked);
        private static readonly Parser<ExpressionType> _subtract = Operator("-", ExpressionType.SubtractChecked);
        private static readonly Parser<ExpressionType> _multiply = Operator("*", ExpressionType.MultiplyChecked);
        private static readonly Parser<ExpressionType> _divide = Operator("/", ExpressionType.Divide);
        private static readonly Parser<ExpressionType> _modulo = Operator("%", ExpressionType.Modulo);
        private static readonly Parser<ExpressionType> _power = Operator("^", ExpressionType.Power);

        private static readonly Parser<Expression> _numberConstant =
             (from value in Parse.Decimal.TokenWithSkipComment()
              select Expression.Constant(long.Parse(value), typeof(object)))
              .Named("number");

        private static readonly Parser<Expression> _boolLiteral =
             (from value in Parse.String("true").Or(Parse.String("false")).Text().TokenWithSkipComment()
              select Expression.Constant(value switch
              {
                  "true" => true,
                  "false" => false,
                  _ => throw new NotImplementedException(),
              }, typeof(object))).Named("boolean");

        private static readonly Parser<Expression> _stringLiteral =
            from leading in Parse.WhiteSpace.XMany()
            from openQuote in Parse.Char('\"')
            from fragments in Parse.Char('\\').Then(_ => Parse.AnyChar.Select(c => $"\\{c}")).Or(Parse.CharExcept("\\\"").XMany().Text()).XMany()
            from closeQuote in Parse.Char('\"')
            from trailing in Parse.WhiteSpace.XMany()
            select Expression.Constant(string.Join(string.Empty, fragments).Replace("\\\"", "\""), typeof(object));


        private static readonly Parser<Expression> _factor =
             (from lparen in Parse.Char('(').TokenWithSkipComment()
              from expr in Parse.Ref(() => _expr).TokenWithSkipComment()
              from rparen in Parse.Char(')').TokenWithSkipComment()
              select expr).Named("expression")
              .XOr(_numberConstant);

        private static readonly Parser<Expression> _operand =
             (from sign in Parse.Char('-').TokenWithSkipComment()
              from factor in _factor.TokenWithSkipComment()
              select Expression.Negate(factor))
              .XOr(_factor).Token();

        private static readonly Parser<Expression> _innerTerm = Parse.ChainRightOperator(_power, _operand, Expression.MakeBinary);

        private static readonly Parser<Expression> _term = Parse.ChainOperator(_multiply.Or(_divide).Or(_modulo), _innerTerm, Expression.MakeBinary);

        private static readonly Parser<Expression> _expr = Parse.ChainOperator(_add.Or(_subtract), _term, Expression.MakeBinary);

        private static readonly Parser<Expression<Func<object>>> _lambda = _expr.XOr(_stringLiteral).XOr(_boolLiteral)
             .Select(body => Expression.Lambda<Func<object>>(body));

        public static Parser<Expression<Func<object>>> GetParser()
        {
            return _lambda;
        }
    }
}
