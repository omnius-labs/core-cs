using System;
using System.Linq;
using System.Linq.Expressions;
using Sprache;

namespace Omnius.Core.RocketPack.DefinitionCompiler.Parsers
{
    // https://github.com/sprache/Sprache/blob/develop/samples/LinqyCalculator/ExpressionParser.cs
    internal static class ExpressionParser
    {
        private static Expression MakeBinary(string ops, Expression leftOp, Expression rightOp)
        {
            if (leftOp.Type == typeof(long) && rightOp.Type == typeof(double))
            {
                leftOp = Expression.Convert(leftOp, typeof(double));
            }

            if (leftOp.Type == typeof(double) && rightOp.Type == typeof(long))
            {
                rightOp = Expression.Convert(rightOp, typeof(double));
            }

            return ops switch
            {
                "+" => Expression.MakeBinary(ExpressionType.AddChecked, leftOp, rightOp),
                "-" => Expression.MakeBinary(ExpressionType.SubtractChecked, leftOp, rightOp),
                "*" => Expression.MakeBinary(ExpressionType.MultiplyChecked, leftOp, rightOp),
                "/" => Expression.MakeBinary(ExpressionType.Divide, leftOp, rightOp),
                "%" => Expression.MakeBinary(ExpressionType.Modulo, leftOp, rightOp),
                _ => throw new NotSupportedException($"{nameof(ops)}:({ops})"),
            };
        }

        private static readonly Parser<string> _add =
            from op in Parse.String("+").Token().Text()
            select op;

        private static readonly Parser<string> _subtract =
            from op in Parse.String("-").Token().Text()
            select op;

        private static readonly Parser<string> _multiply =
            from op in Parse.String("*").Token().Text()
            select op;

        private static readonly Parser<string> _divide =
            from op in Parse.String("/").Token().Text()
            select op;

        private static readonly Parser<string> _modulo =
            from op in Parse.String("%").Token().Text()
            select op;

        private static readonly Parser<Expression> _numberConstant =
            (from value in Parse.Number.Token()
             select Expression.Constant(long.Parse(value), typeof(long))).Named("number");

        private static readonly Parser<Expression> _floatConstant =
            (from integerPart in Parse.Number.Text()
             from point in Parse.Char('.').Token()
             from decimalPart in Parse.Number.Text()
             select Expression.Constant(double.Parse($"{integerPart}.{decimalPart}"), typeof(double))).Named("float");

        private static readonly Parser<Expression> _factor =
            (from lparen in Parse.Char('(').Token()
             from expr in Parse.Ref(() => _expr).Token()
             from rparen in Parse.Char(')').Token()
             select expr).Named("expression").Or(_floatConstant).Or(_numberConstant);

        private static readonly Parser<Expression> _operand =
            (from sign in Parse.Char('-').Token()
             from factor in _factor.Token()
             select Expression.Negate(factor)).XOr(_factor).Token();

        private static readonly Parser<Expression> _term = Parse.ChainOperator(_multiply.XOr(_divide).XOr(_modulo), _operand, MakeBinary);

        private static readonly Parser<Expression> _expr = Parse.ChainOperator(_add.XOr(_subtract), _term, MakeBinary).Select(n => Expression.Convert(n, typeof(object)));

        private static readonly Parser<Expression> _boolLiteral =
             (from value in Parse.String("true").XOr(Parse.String("false")).Text().Token()
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
            from fragments in Parse.Char('\\').Then(_ => Parse.AnyChar.Select(c => $"\\{c}")).XOr(Parse.CharExcept("\\\"").XMany().Text()).XMany()
            from closeQuote in Parse.Char('\"')
            from trailing in Parse.WhiteSpace.XMany()
            select Expression.Constant(string.Join(string.Empty, fragments).Replace("\\\"", "\""), typeof(object));

        private static readonly Parser<Expression<Func<object>>> _lambda =
            _expr.Or(_stringLiteral).Or(_boolLiteral).Select(body => Expression.Lambda<Func<object>>(body));

        public static Parser<Expression<Func<object>>> GetParser() => _lambda;
    }
}
