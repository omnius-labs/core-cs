using System;
using System.Linq;
using System.Linq.Expressions;
using Sprache;

namespace Omnius.Core.RocketPack.DefinitionCompiler.Parsers
{
    // https://github.com/sprache/Sprache/blob/develop/samples/LinqyCalculator/ExpressionParser.cs
    internal static class ExpressionParser
    {
        private static Parser<ExpressionType> Operator(string op, ExpressionType opType) => Parse.String(op).Token().Return(opType);

        private static readonly Parser<ExpressionType> _add = Operator("+", ExpressionType.AddChecked);
        private static readonly Parser<ExpressionType> _subtract = Operator("-", ExpressionType.SubtractChecked);
        private static readonly Parser<ExpressionType> _multiply = Operator("*", ExpressionType.MultiplyChecked);
        private static readonly Parser<ExpressionType> _divide = Operator("/", ExpressionType.Divide);
        private static readonly Parser<ExpressionType> _modulo = Operator("%", ExpressionType.Modulo);
        private static readonly Parser<ExpressionType> _power = Operator("^", ExpressionType.Power);

        private static readonly Parser<Expression> _numberConstant =
             (from value in Parse.Number.Token()
              select Expression.Constant(long.Parse(value), typeof(long)))
              .Named("number");

        private static readonly Parser<Expression> _factor =
             (from lparen in Parse.Char('(').Token()
              from expr in Parse.Ref(() => _expr).Token()
              from rparen in Parse.Char(')').Token()
              select expr).Named("expression")
              .XOr(_numberConstant);

        private static readonly Parser<Expression> _operand =
             (from sign in Parse.Char('-').Token()
              from factor in _factor.Token()
              select Expression.Negate(factor))
              .XOr(_factor).Token();

        private static readonly Parser<Expression> _innerTerm = Parse.ChainRightOperator(_power, _operand, Expression.MakeBinary);

        private static readonly Parser<Expression> _term = Parse.ChainOperator(_multiply.Or(_divide).Or(_modulo), _innerTerm, Expression.MakeBinary);

        private static readonly Parser<Expression> _expr = Parse.ChainOperator(_add.Or(_subtract), _term, Expression.MakeBinary).Select(n => Expression.Convert(n, typeof(object)));

        private static readonly Parser<Expression> _floatConstant =
             (from value in Parse.Decimal.Token()
              select Expression.Constant(double.Parse(value), typeof(double)))
              .Named("float");

        private static readonly Parser<Expression> _boolLiteral =
             (from value in Parse.String("true").Or(Parse.String("false")).Text().Token()
              select Expression.Constant(
                value switch
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

        private static readonly Parser<Expression<Func<object>>> _lambda =
            _expr.XOr(_floatConstant).XOr(_stringLiteral).XOr(_boolLiteral).End().Select(body => Expression.Lambda<Func<object>>(body));

        public static Parser<Expression<Func<object>>> GetParser() => _lambda;
    }
}
