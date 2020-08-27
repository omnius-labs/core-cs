using System.Linq;
using Sprache;

namespace Omnius.Core.RocketPack.DefinitionCompiler.Parsers.Extensions
{
    internal static class ParserExtensions
    {
        // 1行コメントを無視するためのパーサー
        private static Parser<string> SingleLineCommentParser { get; } =
            from leading in Parse.String("//")
            from item in Parse.AnyChar.Except(Parse.LineEnd.Return('\n')).XMany().Text()
            select item;

        // 複数行コメントを無視するためのパーサー
        private static Parser<string> MultiLineCommentParser { get; } =
            from leading in Parse.String("/*")
            from item in Parse.AnyChar.Except(Parse.String("*/")).Or(Parse.LineEnd.Return('\n')).XMany().Text()
            from trailing in Parse.String("*/")
            select item;

        private static Parser<string> CommentParser { get; } = SingleLineCommentParser.Or(MultiLineCommentParser);

        // コメントと空白行を無視する
        public static Parser<T> TokenWithSkipComment<T>(this Parser<T> parser)
        {
            return from leading in CommentParser.Or(Parse.WhiteSpace.Return("")).XMany()
                   from item in parser
                   from trailing in CommentParser.Or(Parse.WhiteSpace.Return("")).XMany()
                   select item;
        }
    }
}
