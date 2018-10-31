using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sprache;

namespace Omnix.Serialization.RocketPack.CodeGenerator
{
    public static class Extensions
    {
        // 1行コメントを無視するためのパーサー
        private static Parser<string> SingleLineCommentParser { get; } =
            from leading in Parse.String("//")
            from item in Parse.AnyChar.Except(Parse.LineEnd.Return('\n')).Many().Text()
            select item;

        // 複数行コメントを無視するためのパーサー
        private static Parser<string> MultiLineCommentParser { get; } =
            from leading in Parse.String("/*")
            from item in Parse.AnyChar.Except(Parse.String("*/")).Or(Parse.LineEnd.Return('\n')).Many().Text()
            from trailing in Parse.String("*/")
            select item;

        private static Parser<string> CommentParser { get; } = SingleLineCommentParser.Or(MultiLineCommentParser);

        // コメントと空白行を無視する
        public static Parser<T> TokenWithSkipComment<T>(this Parser<T> parser)
        {
            return from leading in CommentParser.Or(Parse.WhiteSpace.Return("")).Many()
                   from item in parser
                   from trailing in CommentParser.Or(Parse.WhiteSpace.Return("")).Many()
                   select item;
        }
    }

    public static class RocketFormatParser
    {
        /*
        // ---- Example ----
        syntax = v1;
        
        option csharp_using = "System";

        option csharp_namespace = "RocketPack.Messages";
        
        enum HelloEnum : uint8 {
            Yes = 0,
            No = 1,
        }
        
        message HelloMessage {
            x1: bool = 0,
            x2: int8 = 1,
            x3: int16 = 2,
            x4: int32 = 3,
            x5: int64 = 4,
            x6: uint8 = 5,
            x7: uint16 = 6,
            x8: uint32 = 7,
            x9: uint64 = 8,
            x10: float32 = 9,
            x11: float64 = 10,
            x12: string(128) = 11, // maximum bytes size is 128
            x13: timestamp = 12,
            x14: memory(256) = 13, // maximum bytes size is 256
            [Recyclable]
            x15: memory(256) = 14, // use recyclable memory
            x16: vector<string(128)>(16) = 15,
            x17: map<int8, memory(32)>(32) = 16,
            x18: HelloEnum = 17,
            x19: HelloEnum? = 18, // Nullable
        }
        
        small message HelloMessage2 {
            x1: HelloMessage,
            x2: HelloEnum,
            x3: HelloEnum?, // Error
        }
        */

        public static RocketFormatInfo ParseV1(string text)
        {
            var notWhiteSpace = Sprache.Parse.Char(x => !char.IsWhiteSpace(x), "not whitespace");

            // 「"」で囲まれた文字列を抽出するパーサー
            var stringLiteralParser =
                from leading in Parse.WhiteSpace.Many()
                from openQuote in Parse.Char('\"')
                from fragments in Parse.Char('\\').Then(_ => Parse.AnyChar.Select(c => $"\\{c}")).Or(Parse.CharExcept("\\\"").Many().Text()).Many()
                from closeQuote in Parse.Char('\"')
                from trailing in Parse.WhiteSpace.Many()
                select $"{string.Join(string.Empty, fragments)}";

            // 英数字と'_'の文字列を抽出するパーサー
            var nameParser =
                from name in Parse.Char(x => ('0' <= x && x <= '9') || ('A' <= x && x <= 'Z') || ('a' <= x && x <= 'z') || x == '_', "Name").AtLeastOnce().Text()
                select name;

            // example: syntax = v1
            var syntaxParser =
                from keyword in Parse.String("syntax").TokenWithSkipComment()
                from equal in Parse.Char('=').TokenWithSkipComment()
                from type in Parse.String("v1").TokenWithSkipComment()
                from semicolon in Parse.Char(';').Or(Parse.Return(';')).TokenWithSkipComment()
                select type;

            // example: option csharp_namespace = "RocketPack.Messages";
            var optionParser =
                from keyword in Parse.String("option").TokenWithSkipComment()
                from name in nameParser.TokenWithSkipComment().Text()
                from equal in Parse.Char('=').TokenWithSkipComment()
                from value in stringLiteralParser.TokenWithSkipComment()
                from semicolon in Parse.Char(';').Or(Parse.Return(';')).TokenWithSkipComment()
                select new OptionInfo() { Name = name, Value = value };

            // example: using "RocketPack.Messages";
            var usingParser =
                from keyword in Parse.String("using").TokenWithSkipComment()
                from value in stringLiteralParser.TokenWithSkipComment()
                from semicolon in Parse.Char(';').Or(Parse.Return(';')).TokenWithSkipComment()
                select new UsingInfo() { Path = value };

            // example: [Recyclable]
            var attributeParser =
                from beginTag in Parse.Char('[').TokenWithSkipComment()
                from name in Parse.CharExcept(']').AtLeastOnce().TokenWithSkipComment().Text()
                from endTag in Parse.Char(']').TokenWithSkipComment()
                select name;

            var intTypeParser =
                from isSigned in Parse.Char('u').Then(n => Parse.Return(false)).Or(Parse.Return(true))
                from type in Parse.String("int")
                from size in Parse.Decimal
                from isNullable in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false))
                select new IntTypeInfo() { IsSigned = isSigned, Size = int.Parse(size), IsNullable = isNullable };

            var boolTypeParser =
                from type in Parse.String("bool").TokenWithSkipComment()
                from isNullable in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
                select new BoolTypeInfo() { IsNullable = isNullable };

            var floatTypeParser =
                from type in Parse.String("float").TokenWithSkipComment()
                from size in Parse.Decimal.TokenWithSkipComment()
                from isNullable in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
                select new FloatTypeInfo() { Size = int.Parse(size), IsNullable = isNullable };

            var stringTypeParser =
                from type in Parse.String("string").TokenWithSkipComment()
                from beginParam in Parse.String("(").TokenWithSkipComment()
                from maxLength in Parse.Decimal.TokenWithSkipComment()
                from endParam in Parse.String(")").TokenWithSkipComment()
                from isNullable in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
                select new StringTypeInfo() { MaxLength = int.Parse(maxLength), IsNullable = isNullable };

            var timestampTypeParser =
                from type in Parse.String("timestamp")
                from isNullable in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false))
                select new TimestampTypeInfo() { IsNullable = isNullable };

            var memoryTypeParser =
                from type in Parse.String("memory").TokenWithSkipComment()
                from beginParam in Parse.String("(").TokenWithSkipComment()
                from maxLength in Parse.Decimal.TokenWithSkipComment()
                from endParam in Parse.String(")").TokenWithSkipComment()
                from isNullable in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
                select new MemoryTypeInfo() { MaxLength = int.Parse(maxLength), IsNullable = isNullable };

            var customTypeParser =
                from type in nameParser.Text()
                from isNullable in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
                select new CustomTypeInfo() { TypeName = type, IsNullable = isNullable };

            var vectorTypeParser =
                from type in Parse.String("vector").TokenWithSkipComment()
                from beginType in Parse.String("<").TokenWithSkipComment()
                from elementType in boolTypeParser
                    .Or<TypeInfo>(intTypeParser)
                    .Or(floatTypeParser)
                    .Or(stringTypeParser)
                    .Or(timestampTypeParser)
                    .Or(memoryTypeParser)
                    .Or(customTypeParser).TokenWithSkipComment()
                from endType in Parse.String(">").TokenWithSkipComment()
                from beginParam in Parse.String("(").TokenWithSkipComment()
                from maxLength in Parse.Decimal.TokenWithSkipComment()
                from endParam in Parse.String(")").TokenWithSkipComment()
                from isNullable in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
                select new ListTypeInfo() { ElementType = elementType, MaxLength = int.Parse(maxLength), IsNullable = isNullable };

            var mapTypeParser =
                from type in Parse.String("map").TokenWithSkipComment()
                from beginType in Parse.String("<").TokenWithSkipComment()
                from keyType in boolTypeParser
                    .Or<TypeInfo>(intTypeParser)
                    .Or(floatTypeParser)
                    .Or(stringTypeParser)
                    .Or(timestampTypeParser)
                    .Or(memoryTypeParser)
                    .Or(customTypeParser).TokenWithSkipComment()
                from comma_1 in Parse.Char(',').Or(Parse.Return(',')).TokenWithSkipComment()
                from valueType in boolTypeParser
                    .Or<TypeInfo>(intTypeParser)
                    .Or(floatTypeParser)
                    .Or(stringTypeParser)
                    .Or(timestampTypeParser)
                    .Or(memoryTypeParser)
                    .Or(customTypeParser).TokenWithSkipComment()
                from endType in Parse.String(">").TokenWithSkipComment()
                from beginParam in Parse.String("(").TokenWithSkipComment()
                from maxLength in Parse.Decimal.TokenWithSkipComment()
                from endParam in Parse.String(")").TokenWithSkipComment()
                from isNullable in Parse.Char('?').Then(n => Parse.Return(true)).Or(Parse.Return(false)).TokenWithSkipComment()
                select new MapTypeInfo() { KeyType = keyType, ValueType = valueType, MaxLength = int.Parse(maxLength), IsNullable = isNullable };

            var enumElementParser =
                from attributes in attributeParser.Many().TokenWithSkipComment()
                from name in nameParser.TokenWithSkipComment().Text()
                from equal in Parse.Char('=').TokenWithSkipComment()
                from id in Parse.Decimal.TokenWithSkipComment()
                from comma in Parse.Char(',').TokenWithSkipComment()
                select new EnumElementInfo() { Attributes = attributes.ToList(), Name = name, Id = int.Parse(id) };

            var enumParser =
                from attributes in attributeParser.Many().TokenWithSkipComment()
                from keyword in Parse.String("enum").TokenWithSkipComment()
                from name in nameParser.TokenWithSkipComment().Text()
                from colon in Parse.Char(':').TokenWithSkipComment()
                from type in intTypeParser
                from beginTag in Parse.Char('{').TokenWithSkipComment()
                from enumProperties in enumElementParser.Except(Parse.Char('}')).Many().TokenWithSkipComment()
                from endTag in Parse.Char('}').TokenWithSkipComment()
                select new EnumInfo() { Attributes = attributes.ToList(), Name = name, Type = type, Elements = enumProperties.ToList() };

            var smallMessageElementParser =
                from attributes in attributeParser.Many().TokenWithSkipComment()
                from name in nameParser.TokenWithSkipComment().Text()
                from colon in Parse.Char(':').Or(Parse.Return(':')).TokenWithSkipComment()
                from type in boolTypeParser
                    .Or<TypeInfo>(intTypeParser)
                    .Or(floatTypeParser)
                    .Or(stringTypeParser)
                    .Or(timestampTypeParser)
                    .Or(memoryTypeParser)
                    .Or(vectorTypeParser)
                    .Or(mapTypeParser)
                    .Or(customTypeParser).TokenWithSkipComment()
                from comma in Parse.Char(',').TokenWithSkipComment()
                select new MessageElementInfo() { Attributes = attributes.ToList(), Name = name, Type = type };

            var smallMessageParser =
                from attributes in attributeParser.Many().TokenWithSkipComment()
                from formatType in Parse.String("small").Then(n => Parse.Return(MessageFormatType.Small)).TokenWithSkipComment()
                from keyword in Parse.String("message").TokenWithSkipComment()
                from name in nameParser.TokenWithSkipComment().Text()
                from beginTag in Parse.Char('{').TokenWithSkipComment()
                from elements in smallMessageElementParser.Except(Parse.Char('}')).Many().TokenWithSkipComment()
                from endTag in Parse.Char('}').TokenWithSkipComment()
                select new MessageInfo { Attributes = attributes.ToList(), FormatType = formatType, Name = name, Elements = elements.ToList() };

            var mediumMessageElementParser =
                from attributes in attributeParser.Many().TokenWithSkipComment()
                from name in nameParser.TokenWithSkipComment().Text()
                from colon in Parse.Char(':').Or(Parse.Return(':')).TokenWithSkipComment()
                from type in boolTypeParser
                    .Or<TypeInfo>(intTypeParser)
                    .Or(floatTypeParser)
                    .Or(stringTypeParser)
                    .Or(timestampTypeParser)
                    .Or(memoryTypeParser)
                    .Or(vectorTypeParser)
                    .Or(mapTypeParser)
                    .Or(customTypeParser).TokenWithSkipComment()
                from equal in Parse.Char('=').Or(Parse.Return('=')).TokenWithSkipComment()
                from id in Parse.Decimal.TokenWithSkipComment()
                from comma in Parse.Char(',').TokenWithSkipComment()
                select new MessageElementInfo() { Attributes = attributes.ToList(), Name = name, Type = type, Id = int.Parse(id) };

            var mediumMessageParser =
                from attributes in attributeParser.Many().TokenWithSkipComment()
                from formatType in Parse.String("medium").Then(n => Parse.Return(MessageFormatType.Medium)).Or(Parse.Return(MessageFormatType.Medium))
                from keyword in Parse.String("message").TokenWithSkipComment()
                from name in nameParser.TokenWithSkipComment().Text()
                from beginTag in Parse.Char('{').TokenWithSkipComment()
                from elements in mediumMessageElementParser.Except(Parse.Char('}')).Many().TokenWithSkipComment()
                from endTag in Parse.Char('}').TokenWithSkipComment()
                select new MessageInfo { Attributes = attributes.ToList(), FormatType = formatType, Name = name, Elements = elements.ToList() };

            var formatParser =
                from syntax in syntaxParser.AtLeastOnce().TokenWithSkipComment()
                from headers in usingParser.Or<object>(optionParser).TokenWithSkipComment().Many()
                from contents in enumParser.Or<object>(smallMessageParser).Or(mediumMessageParser).TokenWithSkipComment().Many()
                select new RocketFormatInfo()
                {
                    Usings = headers.OfType<UsingInfo>().ToList(),
                    Options = headers.OfType<OptionInfo>().ToList(),
                    Enums = contents.OfType<EnumInfo>().ToList(),
                    Messages = contents.OfType<MessageInfo>().ToList(),
                };

            var result = formatParser.Parse(text);

            // パースされた結果をチェックする。
            {
                // Small形式のメッセージの要素は常にNull非許容。
                foreach (var messageInfo in result.Messages.Where(n => n.FormatType == MessageFormatType.Small))
                {
                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        elementInfo.Type.IsNullable = false;
                    }
                }

                // Medium形式のメッセージのMemoryタイプの属性に[Recyclable]が設定されている場合は、IsUseMemoryPoolフラグを立てる。
                foreach (var messageInfo in result.Messages.Where(n => n.FormatType == MessageFormatType.Medium))
                {
                    foreach (var elementInfo in messageInfo.Elements)
                    {
                        if (elementInfo.Type is MemoryTypeInfo memoryTypeInfo && elementInfo.Attributes.Contains("Recyclable"))
                        {
                            memoryTypeInfo.IsUseMemoryPool = true;
                        }
                    }
                }
            }

            return result;
        }
    }
}
