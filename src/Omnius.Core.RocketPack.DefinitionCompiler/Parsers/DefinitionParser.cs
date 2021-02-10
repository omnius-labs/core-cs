using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Omnius.Core.RocketPack.DefinitionCompiler.Models;
using Omnius.Core.RocketPack.DefinitionCompiler.Parsers.Extensions;
using Sprache;

namespace Omnius.Core.RocketPack.DefinitionCompiler.Parsers
{
    internal static class DefinitionParser
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        // 「"」で囲まれた文字列を抽出するパーサー
        private static readonly Parser<string> _stringLiteralParser =
            from leading in Parse.WhiteSpace.XMany()
            from openQuote in Parse.Char('\"')
            from fragments in Parse.Char('\\').Then(_ => Parse.AnyChar.Select(c => $"\\{c}")).Or(Parse.CharExcept("\\\"").XMany().Text()).XMany()
            from closeQuote in Parse.Char('\"')
            from trailing in Parse.WhiteSpace.XMany()
            select string.Join(string.Empty, fragments).Replace("\\\"", "\"");

        // 英数字と'_'の文字列を抽出するパーサー
        private static readonly Parser<string> _nameParser =
            from name in Parse.Char(x => (x >= '0' && x <= '9') || (x >= 'A' && x <= 'Z') || (x >= 'a' && x <= 'z') || x == '_', "Name").AtLeastOnce().Text()
            select name;

        // example: syntax v1.0;
        private static readonly Parser<string> _syntaxParser =
            from keyword in Parse.String("syntax").TokenWithSkipComment()
            from type in Parse.String("v1.0").TokenWithSkipComment().Text()
            from semicolon in Parse.Char(';').TokenWithSkipComment()
            select type;

        // example: option csharp_namespace "RocketPack.Messages";
        private static readonly Parser<OptionDefinition> _optionParser =
            from keyword in Parse.String("option").TokenWithSkipComment()
            from name in _nameParser.TokenWithSkipComment()
            from value in ExpressionParser.GetParser()
            from semicolon in Parse.Char(';').TokenWithSkipComment()
            select new OptionDefinition(name, value.Compile().Invoke());

        // example: using "RocketPack.Messages";
        private static readonly Parser<UsingDefinition> _usingParser =
            from keyword in Parse.String("using").TokenWithSkipComment()
            from value in _stringLiteralParser.TokenWithSkipComment()
            from semicolon in Parse.Char(';').TokenWithSkipComment()
            select new UsingDefinition(value);

        // example: namespace "RocketPack.Messages";
        private static readonly Parser<NamespaceDefinition> _namespaceParser =
            from keyword in Parse.String("namespace").TokenWithSkipComment()
            from value in _stringLiteralParser.TokenWithSkipComment()
            from semicolon in Parse.Char(';').TokenWithSkipComment()
            select new NamespaceDefinition(value);

        // example: [csharp_recyclable]
        private static readonly Parser<string> _attributeParser =
            from beginTag in Parse.Char('[').TokenWithSkipComment()
            from name in _nameParser.TokenWithSkipComment()
            from endTag in Parse.Char(']').TokenWithSkipComment()
            select name;

        // example: capacity: 1024,
        private static readonly Parser<(string key, object value)> _parametersElementParser =
            from key in _nameParser.TokenWithSkipComment()
            from colon in Parse.Char(':').TokenWithSkipComment()
            from value in ExpressionParser.GetParser()
            from comma in Parse.Char(',').Or(Parse.Return(',')).TokenWithSkipComment()
            select (key, value.Compile().Invoke());

        // example: (capacity: 1024, recyclable: true)
        private static readonly Parser<Dictionary<string, object>> _parametersParser =
            from beginTag in Parse.Char('(').TokenWithSkipComment()
            from elements in _parametersElementParser.XMany().TokenWithSkipComment()
            from endTag in Parse.Char(')').TokenWithSkipComment()
            select new Dictionary<string, object>(elements.Select(n => new KeyValuePair<string, object>(n.key, n.value)));

        private static readonly Parser<IntType> _intTypeParser =
            from isSigned in Parse.Char('u').Then(n => Parse.Return(false)).Or(Parse.Return(true))
            from type in Parse.String("int")
            from size in Parse.Decimal
            from isOptional in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
            select new IntType(isSigned, int.Parse(size), isOptional);

        private static readonly Parser<BoolType> _boolTypeParser =
            from type in Parse.String("bool").TokenWithSkipComment()
            from isOptional in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
            select new BoolType(isOptional);

        private static readonly Parser<FloatType> _floatTypeParser =
            from type in Parse.String("float").TokenWithSkipComment()
            from size in Parse.Decimal.TokenWithSkipComment()
            from isOptional in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
            select new FloatType(int.Parse(size), isOptional);

        private static readonly Parser<StringType> _stringTypeParser =
            from type in Parse.String("string").TokenWithSkipComment()
            from isOptional in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
            from parameters in _parametersParser.Or(Parse.Return(new Dictionary<string, object>())).TokenWithSkipComment()
            select new StringType(isOptional, parameters);

        private static readonly Parser<TimestampType> _timestampTypeParser =
            from type in Parse.String("timestamp")
            from isOptional in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
            select new TimestampType(isOptional);

        private static readonly Parser<BytesType> _memoryTypeParser =
            from type in Parse.String("bytes").TokenWithSkipComment()
            from isOptional in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
            from parameters in _parametersParser.Or(Parse.Return(new Dictionary<string, object>())).TokenWithSkipComment()
            select new BytesType(isOptional, parameters);

        private static readonly Parser<CustomType> _customTypeParser =
            from type in _nameParser.Text()
            from isOptional in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
            select new CustomType(type, isOptional);

        private static readonly Parser<VectorType> _vectorTypeParser =
            from type in Parse.String("vector").TokenWithSkipComment()
            from beginType in Parse.String("<").TokenWithSkipComment()
            from elementType in _boolTypeParser
                .Or<TypeBase>(_intTypeParser)
                .Or(_floatTypeParser)
                .Or(_stringTypeParser)
                .Or(_timestampTypeParser)
                .Or(_memoryTypeParser)
                .Or(_customTypeParser).TokenWithSkipComment()
            from endType in Parse.String(">").TokenWithSkipComment()
            from isOptional in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
            from parameters in _parametersParser.Or(Parse.Return(new Dictionary<string, object>())).TokenWithSkipComment()
            select new VectorType(elementType, isOptional, parameters);

        private static readonly Parser<MapType> _mapTypeParser =
            from type in Parse.String("map").TokenWithSkipComment()
            from beginType in Parse.Char('<').TokenWithSkipComment()
            from keyType in _boolTypeParser
                .Or<TypeBase>(_intTypeParser)
                .Or(_floatTypeParser)
                .Or(_stringTypeParser)
                .Or(_timestampTypeParser)
                .Or(_memoryTypeParser)
                .Or(_customTypeParser).TokenWithSkipComment()
            from comma in Parse.Char(',').TokenWithSkipComment()
            from valueType in _boolTypeParser
                .Or<TypeBase>(_intTypeParser)
                .Or(_floatTypeParser)
                .Or(_stringTypeParser)
                .Or(_timestampTypeParser)
                .Or(_memoryTypeParser)
                .Or(_customTypeParser).TokenWithSkipComment()
            from endType in Parse.Char('>').TokenWithSkipComment()
            from isOptional in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
            from parameters in _parametersParser.Or(Parse.Return(new Dictionary<string, object>())).TokenWithSkipComment()
            select new MapType(keyType, valueType, isOptional, parameters);

        private static readonly Parser<EnumElement> _enumElementParser =
            from attributes in _attributeParser.XMany().TokenWithSkipComment()
            from name in _nameParser.TokenWithSkipComment()
            from equal in Parse.Char('=').TokenWithSkipComment()
            from id in Parse.Decimal.TokenWithSkipComment()
            from comma in Parse.Char(',').TokenWithSkipComment()
            select new EnumElement(attributes.ToList(), name, int.Parse(id));

        private static readonly Parser<EnumDefinition> _enumDefinitionParser =
            (from attributes in _attributeParser.XMany().TokenWithSkipComment()
             from keyword in Parse.String("enum").TokenWithSkipComment()
             from name in _nameParser.TokenWithSkipComment()
             from colon in Parse.Char(':').TokenWithSkipComment()
             from type in _intTypeParser
             from beginTag in Parse.Char('{').TokenWithSkipComment()
             from enumProperties in _enumElementParser.Except(Parse.Char('}')).XMany().TokenWithSkipComment()
             from endTag in Parse.Char('}').TokenWithSkipComment()
             select new EnumDefinition(attributes.ToList(), name, type, enumProperties.ToList())).Named("enum");

        private static readonly Parser<ObjectElement> _objectElementParser =
            from attributes in _attributeParser.XMany().TokenWithSkipComment()
            from name in _nameParser.TokenWithSkipComment()
            from colon in Parse.Char(':').TokenWithSkipComment()
            from type in _boolTypeParser
                .Or<TypeBase>(_intTypeParser)
                .Or(_floatTypeParser)
                .Or(_stringTypeParser)
                .Or(_timestampTypeParser)
                .Or(_memoryTypeParser)
                .Or(_vectorTypeParser)
                .Or(_mapTypeParser)
                .Or(_customTypeParser).TokenWithSkipComment()
            from comma in Parse.Char(',').TokenWithSkipComment()
            select new ObjectElement(attributes.ToList(), name, type);

        private static readonly Parser<ObjectDefinition> _objectDefinitionParser =
            from attributes in _attributeParser.XMany().TokenWithSkipComment()
            from type in Parse.String("struct").TokenWithSkipComment().Return(MessageFormatType.Struct).Or(Parse.String("message").TokenWithSkipComment().Return(MessageFormatType.Message))
            from name in _nameParser.TokenWithSkipComment()
            from beginTag in Parse.Char('{').TokenWithSkipComment()
            from elements in _objectElementParser.Except(Parse.Char('}')).XMany().TokenWithSkipComment()
            from endTag in Parse.Char('}').TokenWithSkipComment()
            select new ObjectDefinition(attributes.ToList(), name, type, elements.ToList());

        private static readonly Parser<FuncElement> _funcElementParser =
            from attributes in _attributeParser.XMany().TokenWithSkipComment()
            from name in _nameParser.TokenWithSkipComment()
            from colon in Parse.Char(':').TokenWithSkipComment()
            from beginParam in Parse.Char('(').TokenWithSkipComment()
            from inType in _customTypeParser.Or(Parse.Return<CustomType?>(null))
            from endParam in Parse.Char(')').TokenWithSkipComment()
            from arrow in Parse.String("->").TokenWithSkipComment()
            from beginResult in Parse.Char('(').TokenWithSkipComment()
            from outType in _customTypeParser.Or(Parse.Return<CustomType?>(null))
            from endResult in Parse.Char(')').TokenWithSkipComment()
            from comma in Parse.Char(',').TokenWithSkipComment()
            select new FuncElement(attributes.ToList(), name, inType, outType);

        private static readonly Parser<ServiceDefinition> _serviceDefinitionParser =
            from attributes in _attributeParser.XMany().TokenWithSkipComment()
            from keyword in Parse.String("service").TokenWithSkipComment()
            from name in _nameParser.TokenWithSkipComment()
            from beginTag in Parse.Char('{').TokenWithSkipComment()
            from funcElements in _funcElementParser.Except(Parse.Char('}')).XMany().TokenWithSkipComment()
            from endTag in Parse.Char('}').TokenWithSkipComment()
            select new ServiceDefinition(attributes.ToList(), name, funcElements.ToList());

        private static readonly Parser<RocketPackDefinition> _rocketPackDefinitionParser =
            from syntax in _syntaxParser.Once().TokenWithSkipComment()
            from usings in _usingParser.XMany().TokenWithSkipComment()
            from @namespace in _namespaceParser.Once().TokenWithSkipComment()
            from options in _optionParser.XMany().TokenWithSkipComment()
            from contents in _enumDefinitionParser.Or<object>(_objectDefinitionParser).Or(_serviceDefinitionParser).XMany().TokenWithSkipComment().End()
            select new RocketPackDefinition(
                usings,
                @namespace.First(),
                options,
                contents.OfType<EnumDefinition>().ToList(),
                contents.OfType<ObjectDefinition>().ToList(),
                contents.OfType<ServiceDefinition>().ToList());

        private static string LoadDefinition(string path)
        {
            using var reader = new StreamReader(path);
            var text = reader.ReadToEnd();
            return text;
        }

        private static RocketPackDefinition ParseDefinition(string text)
        {
            var result = _rocketPackDefinitionParser.Parse(text);

            foreach (var item in result.Objects)
            {
                item.Namespace = result.Namespace.Value;
            }

            foreach (var item in result.Enums)
            {
                item.Namespace = result.Namespace.Value;
            }

            foreach (var item in result.Services)
            {
                item.Namespace = result.Namespace.Value;
            }

            return result;
        }

        private static void ValidateDefinition(RocketPackDefinition result)
        {
            // struct形式のメッセージはOptional型は認めない。
            foreach (var objectDefinition in result.Objects.Where(n => n.FormatType == MessageFormatType.Struct))
            {
                if (objectDefinition.Elements.Any(n => n.Type.IsOptional)) throw new Exception();
            }
        }

        public static RocketPackDefinition Load(string definitionFilePath)
        {
            try
            {
                // Load
                var text = LoadDefinition(definitionFilePath);

                // Parse
                var result = ParseDefinition(text);

                // Validate
                ValidateDefinition(result);

                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"Load Error: {definitionFilePath}");
                throw;
            }
        }
    }
}
