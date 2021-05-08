using Sprache;

namespace Omnius.Core.Net.Internal.Extensions
{
    internal static class ParserExtensions
    {
        // コメントと空白行を無視する
        public static Parser<T> TokenWithSkipSpace<T>(this Parser<T> parser)
        {
            return from leading in Parse.WhiteSpace.Many()
                   from item in parser
                   from trailing in Parse.WhiteSpace.Many()
                   select item;
        }
    }
}
